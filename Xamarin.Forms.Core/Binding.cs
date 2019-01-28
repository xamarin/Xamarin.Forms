using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public sealed class Binding : BindingBase
	{
		internal const string SelfPath = ".";
		IValueConverter _converter;
		object _converterParameter;

		BindingExpression _expression;
		string _path;
		object _source;
		string _updateSourceEventName;

		public Binding()
		{
		}

		public Binding(string path, BindingMode mode = BindingMode.Default, IValueConverter converter = null, object converterParameter = null, string stringFormat = null, object source = null)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentException("path can not be an empty string", nameof(path));

			Path = path;
			Converter = converter;
			ConverterParameter = converterParameter;
			Mode = mode;
			StringFormat = stringFormat;
			Source = source;
		}

		public IValueConverter Converter
		{
			get { return _converter; }
			set
			{
				ThrowIfApplied();

				_converter = value;
			}
		}

		public object ConverterParameter
		{
			get { return _converterParameter; }
			set
			{
				ThrowIfApplied();

				_converterParameter = value;
			}
		}

		public string Path
		{
			get { return _path; }
			set
			{
				ThrowIfApplied();

				_path = value;
				_expression = new BindingExpression(this, !string.IsNullOrWhiteSpace(value) ? value : SelfPath);
			}
		}

		public object Source
		{
			get { return _source; }
			set
			{
				ThrowIfApplied();
				_source = value;
				if ((value as RelativeBindingSource)?.Mode == RelativeBindingSourceMode.TemplatedParent)
					this.AllowChaining = true;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string UpdateSourceEventName {
			get { return _updateSourceEventName; }
			set {
				ThrowIfApplied();
				_updateSourceEventName = value;
			}
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Binding Create<TSource>(Expression<Func<TSource, object>> propertyGetter, BindingMode mode = BindingMode.Default, IValueConverter converter = null, object converterParameter = null,
											  string stringFormat = null)
		{
			if (propertyGetter == null)
				throw new ArgumentNullException(nameof(propertyGetter));

			return new Binding(GetBindingPath(propertyGetter), mode, converter, converterParameter, stringFormat);
		}

		internal override void Apply(bool fromTarget)
		{
			base.Apply(fromTarget);

			if (_expression == null)
				_expression = new BindingExpression(this, SelfPath);

			_expression.Apply(fromTarget);
		}

		internal override void Apply(object context, BindableObject bindObj, BindableProperty targetProperty, bool fromBindingContextChanged = false, bool fromAncestorChanged = false)
		{
			object src = _source;
			var isApplied = IsApplied;

			base.Apply(src ?? context, bindObj, targetProperty, fromBindingContextChanged: fromBindingContextChanged);

			if (isApplied && !ShouldReapply(fromBindingContextChanged, fromAncestorChanged))
				return;

			if ( this.Source is RelativeBindingSource )
				ResolveRelativeSource(bindObj, targetProperty);
			else
				CompleteApplyBinding(src ?? Context ?? context, bindObj, targetProperty);
		}

		private void CompleteApplyBinding(object bindingContext, BindableObject bindObj, BindableProperty targetProperty)
		{
			if (_expression == null && bindingContext != null)
				_expression = new BindingExpression(this, SelfPath);
			_expression.Apply(bindingContext, bindObj, targetProperty);
		}

		private async void ResolveRelativeSource(BindableObject bindObj, BindableProperty targetProperty)
		{
			RelativeBindingSource relSource = this.Source as RelativeBindingSource;
			object bindingContext = null;
			switch (relSource.Mode)
			{
				case RelativeBindingSourceMode.Self:
					bindingContext = bindObj;
					break;
				case RelativeBindingSourceMode.TemplatedParent:
					{
						var view = bindObj as Element;
						if (view == null)
							throw new InvalidOperationException();
						bindingContext = await TemplateUtilities.FindTemplatedParentAsync(view);
						break;
					}
				case RelativeBindingSourceMode.FindAncestor:
					{
						if (!(bindObj is Element elem))
							throw new InvalidOperationException();
						Element parent = await TemplateUtilities.GetRealParentAsync(elem);
						int currentLevel = 1;

						while (parent != null)
						{
							if (currentLevel >= relSource.AncestorLevel)
							{
								if (relSource.AncestorType.IsInstanceOfType(parent))
								{
									bindingContext = parent;
									break;
								}
								else if (relSource.AncestorType.IsInstanceOfType(parent.BindingContext))
								{
									bindingContext = parent.BindingContext;
									break;
								}
							}
							parent = await TemplateUtilities.GetRealParentAsync(parent);
							currentLevel++;
						}
						break;
					};
				default:
					throw new NotImplementedException();
			}

			CompleteApplyBinding(bindingContext, bindObj, targetProperty);
		}

		internal override BindingBase Clone()
		{
			return new Binding(Path, Mode) {
				Converter = Converter,
				ConverterParameter = ConverterParameter,
				StringFormat = StringFormat,
				Source = Source,
				UpdateSourceEventName = UpdateSourceEventName,
				TargetNullValue = TargetNullValue,
				FallbackValue = FallbackValue
			};
		}

		internal override object GetSourceValue(object value, Type targetPropertyType)
		{
			if (Converter != null)
				value = Converter.Convert(value, targetPropertyType, ConverterParameter, CultureInfo.CurrentUICulture);

			return base.GetSourceValue(value, targetPropertyType);
		}

		internal override object GetTargetValue(object value, Type sourcePropertyType)
		{
			if (Converter != null)
				value = Converter.ConvertBack(value, sourcePropertyType, ConverterParameter, CultureInfo.CurrentUICulture);

			return base.GetTargetValue(value, sourcePropertyType);
		}

		internal override void Unapply(bool fromBindingContextChanged = false, bool fromAncestorChange = false)
		{
			if ( this.IsApplied && !ShouldReapply(fromBindingContextChanged, fromAncestorChange))
				return;
			
			base.Unapply(fromBindingContextChanged: fromBindingContextChanged, fromAncestorChanged: fromAncestorChange);

			if (_expression != null)
				_expression.Unapply();
		}

		private bool ShouldReapply(bool fromBindingContextChanged, bool fromAncestorChange)
		{
			if ( this.Source is RelativeBindingSource relSource )
			{
				if (relSource.Mode == RelativeBindingSourceMode.Self ||
					 relSource.Mode == RelativeBindingSourceMode.TemplatedParent)
					// We never need to reapply bindings in these
					// relative source modes because the resolved binding
					// source will never change during the life of the
					// target.
					return false;
				else if (relSource.Mode == RelativeBindingSourceMode.FindAncestor)
					// Avoids a potentially expensive re-calculation
					// of the ancestor every time the BindingContext
					// changes.
					return !fromBindingContextChanged || fromAncestorChange;
				else
					return true;
			}
			else
			{
				return (Source == null || !fromBindingContextChanged) && !fromAncestorChange;
			}
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		static string GetBindingPath<TSource>(Expression<Func<TSource, object>> propertyGetter)
		{
			Expression expr = propertyGetter.Body;

			var unary = expr as UnaryExpression;
			if (unary != null)
				expr = unary.Operand;

			var builder = new StringBuilder();

			var indexed = false;

			var member = expr as MemberExpression;
			if (member == null)
			{
				var methodCall = expr as MethodCallExpression;
				if (methodCall != null)
				{
					if (methodCall.Arguments.Count == 0)
						throw new ArgumentException("Method calls are not allowed in binding expression");

					var arguments = new List<string>(methodCall.Arguments.Count);
					foreach (Expression arg in methodCall.Arguments)
					{
						if (arg.NodeType != ExpressionType.Constant)
							throw new ArgumentException("Only constants can be used as indexer arguments");

						object value = ((ConstantExpression)arg).Value;
						arguments.Add(value != null ? value.ToString() : "null");
					}

					Type declarerType = methodCall.Method.DeclaringType;
					DefaultMemberAttribute defaultMember = declarerType.GetTypeInfo().GetCustomAttributes(typeof(DefaultMemberAttribute), true).OfType<DefaultMemberAttribute>().FirstOrDefault();
					string indexerName = defaultMember != null ? defaultMember.MemberName : "Item";

					MethodInfo getterInfo =
						declarerType.GetProperties().Where(pi => pi.Name == indexerName && pi.CanRead && pi.GetMethod.IsPublic && !pi.GetMethod.IsStatic).Select(pi => pi.GetMethod).FirstOrDefault();
					if (getterInfo != null)
					{
						if (getterInfo == methodCall.Method)
						{
							indexed = true;
							builder.Append("[");

							var first = true;
							foreach (string argument in arguments)
							{
								if (!first)
									builder.Append(",");

								builder.Append(argument);
								first = false;
							}

							builder.Append("]");

							member = methodCall.Object as MemberExpression;
						}
						else
							throw new ArgumentException("Method calls are not allowed in binding expressions");
					}
					else
						throw new ArgumentException("Public indexer not found");
				}
				else
					throw new ArgumentException("Invalid expression type");
			}

			while (member != null)
			{
				var property = (PropertyInfo)member.Member;
				if (builder.Length != 0)
				{
					if (!indexed)
						builder.Insert(0, ".");
					else
						indexed = false;
				}

				builder.Insert(0, property.Name);

				//				member = member.Expression as MemberExpression ?? (member.Expression as UnaryExpression)?.Operand as MemberExpression;
				member = member.Expression as MemberExpression ?? (member.Expression is UnaryExpression ? (member.Expression as UnaryExpression).Operand as MemberExpression : null);
			}

			return builder.ToString();
		}
	}
}