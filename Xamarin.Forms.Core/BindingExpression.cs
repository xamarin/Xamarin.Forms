﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms
{
	internal class BindingExpression
	{
		internal const string PropertyNotFoundErrorMessage = "'{0}' property not found on '{1}', target property: '{2}.{3}'";

		readonly List<BindingExpressionPart> _parts = new List<BindingExpressionPart>();

		BindableProperty _targetProperty;
		WeakReference<object> _weakSource;
		WeakReference<BindableObject> _weakTarget;

		internal BindingExpression(BindingBase binding, string path)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			if (path == null)
				throw new ArgumentNullException("path");

			Binding = binding;
			Path = path;

			ParsePath();
		}

		internal BindingBase Binding { get; }

		internal string Path { get; }

		/// <summary>
		///     Applies the binding expression to a previously set source and target.
		/// </summary>
		internal void Apply(bool fromTarget = false)
		{
			if (_weakSource == null || _weakTarget == null)
				return;

			BindableObject target;
			if (!_weakTarget.TryGetTarget(out target))
			{
				Unapply();
				return;
			}

			object source;
			if (_weakSource.TryGetTarget(out source) && _targetProperty != null)
				ApplyCore(source, target, _targetProperty, fromTarget);
		}

		/// <summary>
		///     Applies the binding expression to a new source or target.
		/// </summary>
		internal void Apply(object sourceObject, BindableObject target, BindableProperty property)
		{
			_targetProperty = property;

			BindableObject prevTarget;
			if (_weakTarget != null && _weakTarget.TryGetTarget(out prevTarget) && !ReferenceEquals(prevTarget, target))
				throw new InvalidOperationException("Binding instances can not be reused");

			object previousSource;
			if (_weakSource != null && _weakSource.TryGetTarget(out previousSource) && !ReferenceEquals(previousSource, sourceObject))
				throw new InvalidOperationException("Binding instances can not be reused");

			_weakSource = new WeakReference<object>(sourceObject);
			_weakTarget = new WeakReference<BindableObject>(target);

			ApplyCore(sourceObject, target, property);
		}

		internal void Unapply()
		{
			object sourceObject;
			if (_weakSource != null && _weakSource.TryGetTarget(out sourceObject))
			{
				for (var i = 0; i < _parts.Count - 1; i++)
				{
					BindingExpressionPart part = _parts[i];

					if (!part.IsSelf)
					{
						part.TryGetValue(sourceObject, out sourceObject);
					}

					part.Unsubscribe();
				}
			}

			_weakSource = null;
			_weakTarget = null;
		}

		/// <summary>
		///     Applies the binding expression to a previously set source or target.
		/// </summary>
		void ApplyCore(object sourceObject, BindableObject target, BindableProperty property, bool fromTarget = false)
		{
			BindingMode mode = Binding.GetRealizedMode(_targetProperty);
			if (mode == BindingMode.OneWay && fromTarget)
				return;

			bool needsGetter = (mode == BindingMode.TwoWay && !fromTarget) || mode == BindingMode.OneWay;
			bool needsSetter = !needsGetter && ((mode == BindingMode.TwoWay && fromTarget) || mode == BindingMode.OneWayToSource);

			object current = sourceObject;
			object previous = null;
			BindingExpressionPart part = null;

			for (var i = 0; i < _parts.Count; i++)
			{
				part = _parts[i];
				bool isLast = i + 1 == _parts.Count;

				if (!part.IsSelf && current != null)
				{
					// Allow the object instance itself to provide its own TypeInfo 
					var reflectable = current as IReflectableType;
					TypeInfo currentType = reflectable != null ? reflectable.GetTypeInfo() : current.GetType().GetTypeInfo();
					if (part.LastGetter == null || !part.LastGetter.DeclaringType.GetTypeInfo().IsAssignableFrom(currentType))
						SetupPart(currentType, part);

					if (!isLast)
						part.TryGetValue(current, out current);
				}

				if (!part.IsSelf && current != null)
				{
					if ((needsGetter && part.LastGetter == null) || (needsSetter && part.NextPart == null && part.LastSetter == null))
					{
						Log.Warning("Binding", PropertyNotFoundErrorMessage, part.Content, current, target.GetType(), property.PropertyName);
						break;
					}
				}

				if (mode == BindingMode.OneWay || mode == BindingMode.TwoWay)
				{
					var inpc = current as INotifyPropertyChanged;
					if (inpc != null && !ReferenceEquals(current, previous))
					{
						part.Subscribe(inpc);
					}
				}

				previous = current;
			}

			Debug.Assert(part != null, "There should always be at least the self part in the expression.");

			if (needsGetter)
			{
				object value = property.DefaultValue;
				if (part.TryGetValue(current, out value) || part.IsSelf)
				{
					value = Binding.GetSourceValue(value, property.ReturnType);
				}
				else
					value = property.DefaultValue;

				if (!TryConvert(part, ref value, property.ReturnType, true))
				{
					Log.Warning("Binding", "{0} can not be converted to type '{1}'", value, property.ReturnType);
					return;
				}

				target.SetValueCore(property, value, BindableObject.SetValueFlags.ClearDynamicResource, BindableObject.SetValuePrivateFlags.Default | BindableObject.SetValuePrivateFlags.Converted);
			}
			else if (needsSetter && part.LastSetter != null && current != null)
			{
				object value = Binding.GetTargetValue(target.GetValue(property), part.SetterType);

				if (!TryConvert(part, ref value, part.SetterType, false))
				{
					Log.Warning("Binding", "{0} can not be converted to type '{1}'", value, part.SetterType);
					return;
				}

				object[] args;
				if (part.IsIndexer)
				{
					args = new object[part.Arguments.Length + 1];
					part.Arguments.CopyTo(args, 0);
					args[args.Length - 1] = value;
				}
				else if (part.IsBindablePropertySetter)
				{
					args = new[] { part.BindablePropertyField, value };
				}
				else
				{
					args = new[] { value };
				}

				part.LastSetter.Invoke(current, args);
			}
		}

		IEnumerable<BindingExpressionPart> GetPart(string part)
		{
			part = part.Trim();
			if (part == string.Empty)
				throw new FormatException("Path contains an empty part");

			BindingExpressionPart indexer = null;

			int lbIndex = part.IndexOf('[');
			if (lbIndex != -1)
			{
				int rbIndex = part.LastIndexOf(']');
				if (rbIndex == -1)
					throw new FormatException("Indexer did not contain closing bracket");

				int argLength = rbIndex - lbIndex - 1;
				if (argLength == 0)
					throw new FormatException("Indexer did not contain arguments");

				string argString = part.Substring(lbIndex + 1, argLength);
				indexer = new BindingExpressionPart(this, argString, true);

				part = part.Substring(0, lbIndex);
				part = part.Trim();
			}

			if (part.Length > 0)
				yield return new BindingExpressionPart(this, part);
			if (indexer != null)
				yield return indexer;
		}

		void ParsePath()
		{
			string p = Path.Trim();

			var last = new BindingExpressionPart(this, ".");
			_parts.Add(last);

			if (p[0] == '.')
			{
				if (p.Length == 1)
					return;

				p = p.Substring(1);
			}

			string[] pathParts = p.Split('.');
			for (var i = 0; i < pathParts.Length; i++)
			{
				foreach (BindingExpressionPart part in GetPart(pathParts[i]))
				{
					last.NextPart = part;
					_parts.Add(part);
					last = part;
				}
			}
		}

		void SetupPart(TypeInfo sourceType, BindingExpressionPart part)
		{
			part.Arguments = null;
			part.LastGetter = null;
			part.LastSetter = null;

			PropertyInfo property = null;
			if (part.IsIndexer)
			{
				if (sourceType.IsArray)
				{
					int index;
					if (!int.TryParse(part.Content, out index))
						Log.Warning("Binding", "{0} could not be parsed as an index for a {1}", part.Content, sourceType);
					else
						part.Arguments = new object[] { index };

					part.LastGetter = sourceType.GetDeclaredMethod("Get");
					part.LastSetter = sourceType.GetDeclaredMethod("Set");
					part.SetterType = sourceType.GetElementType();
				}

				DefaultMemberAttribute defaultMember = sourceType.GetCustomAttributes(typeof(DefaultMemberAttribute), true).OfType<DefaultMemberAttribute>().FirstOrDefault();
				string indexerName = defaultMember != null ? defaultMember.MemberName : "Item";

				part.IndexerName = indexerName;

				property = sourceType.GetDeclaredProperty(indexerName);
				if (property == null)
					property = sourceType.BaseType.GetProperty(indexerName);

				if (property != null)
				{
					ParameterInfo parameter = property.GetIndexParameters().FirstOrDefault();
					if (parameter != null)
					{
						try
						{
							object arg = Convert.ChangeType(part.Content, parameter.ParameterType, CultureInfo.InvariantCulture);
							part.Arguments = new[] { arg };
						}
						catch (FormatException)
						{
						}
						catch (InvalidCastException)
						{
						}
						catch (OverflowException)
						{
						}
					}
				}
			}
			else
			{
				property = sourceType.GetDeclaredProperty(part.Content);
				if (property == null)
					property = sourceType.BaseType.GetProperty(part.Content);
			}

			if (property != null)
			{
				if (property.CanRead && property.GetMethod.IsPublic && !property.GetMethod.IsStatic)
					part.LastGetter = property.GetMethod;
				if (property.CanWrite && property.SetMethod.IsPublic && !property.SetMethod.IsStatic)
				{
					part.LastSetter = property.SetMethod;
					part.SetterType = part.LastSetter.GetParameters().Last().ParameterType;

					if (Binding.AllowChaining)
					{
						FieldInfo bindablePropertyField = sourceType.GetDeclaredField(part.Content + "Property");
						if (bindablePropertyField != null && bindablePropertyField.FieldType == typeof(BindableProperty) && sourceType.ImplementedInterfaces.Contains(typeof(IElementController)))
						{
							MethodInfo setValueMethod = null;
							foreach (MethodInfo m in sourceType.AsType().GetRuntimeMethods())
							{
								if (m.Name.EndsWith("IElementController.SetValueFromRenderer"))
								{
									ParameterInfo[] parameters = m.GetParameters();
									if (parameters.Length == 2 && parameters[0].ParameterType == typeof(BindableProperty))
									{
										setValueMethod = m;
										break;
									}
								}
							}
							if (setValueMethod != null)
							{
								part.LastSetter = setValueMethod;
								part.IsBindablePropertySetter = true;
								part.BindablePropertyField = bindablePropertyField.GetValue(null);
							}
						}
					}
				}
			}
		}

		bool TryConvert(BindingExpressionPart part, ref object value, Type convertTo, bool toTarget)
		{
			if (value == null)
				return true;
			if ((toTarget && _targetProperty.TryConvert(ref value)) || (!toTarget && convertTo.IsInstanceOfType(value)))
				return true;

			object original = value;
			try
			{
				value = Convert.ChangeType(value, convertTo, CultureInfo.InvariantCulture);
				return true;
			}
			catch (InvalidCastException)
			{
				value = original;
				return false;
			}
			catch (FormatException)
			{
				value = original;
				return false;
			}
			catch (OverflowException)
			{
				value = original;
				return false;
			}
		}

		class BindingPair
		{
			public BindingPair(BindingExpressionPart part, object source, bool isLast)
			{
				Part = part;
				Source = source;
				IsLast = isLast;
			}

			public bool IsLast { get; set; }

			public BindingExpressionPart Part { get; private set; }

			public object Source { get; private set; }
		}

		class WeakPropertyChangedProxy
		{
			WeakReference _source, _listener;
			internal WeakReference Source => _source;

			public WeakPropertyChangedProxy(INotifyPropertyChanged source, PropertyChangedEventHandler listener)
			{
				source.PropertyChanged += OnPropertyChanged;
				_source = new WeakReference(source);
				_listener = new WeakReference(listener);
			}

			public void Unsubscribe()
			{
				if (_source != null)
				{
					var source = _source.Target as INotifyPropertyChanged;
					if (source != null)
					{
						source.PropertyChanged -= OnPropertyChanged;
					}
					_source = null;
					_listener = null;
				}
			}

			private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (_listener != null)
				{
					var handler = _listener.Target as PropertyChangedEventHandler;
					if (handler != null)
					{
						handler(sender, e);
					}
					else
					{
						Unsubscribe();
					}
				}
				else
				{
					Unsubscribe();
				}
			}
		}

		class BindingExpressionPart
		{
			readonly BindingExpression _expression;
			readonly PropertyChangedEventHandler _changeHandler;
			WeakPropertyChangedProxy _listener;

			public BindingExpressionPart(BindingExpression expression, string content, bool isIndexer = false)
			{
				_expression = expression;
				IsSelf = content == Forms.Binding.SelfPath;
				Content = content;
				IsIndexer = isIndexer;

				_changeHandler = PropertyChanged;
			}

			public void Subscribe(INotifyPropertyChanged handler)
			{
				if (ReferenceEquals(handler, _listener?.Source?.Target))
				{
					// Already subscribed
					return;
				}

				// Clear out the old subscription if necessary
				Unsubscribe();

				_listener = new WeakPropertyChangedProxy(handler, _changeHandler);
			}

			public void Unsubscribe()
			{
				var listener = _listener;
				if (listener != null)
				{
					listener.Unsubscribe();
					_listener = null;
				}
			}

			public object[] Arguments { get; set; }

			public object BindablePropertyField { get; set; }

			public string Content { get; }

			public string IndexerName { get; set; }

			public bool IsBindablePropertySetter { get; set; }

			public bool IsIndexer { get; }

			public bool IsSelf { get; }

			public MethodInfo LastGetter { get; set; }

			public MethodInfo LastSetter { get; set; }

			public BindingExpressionPart NextPart { get; set; }

			public Type SetterType { get; set; }

			public void PropertyChanged(object sender, PropertyChangedEventArgs args)
			{
				BindingExpressionPart part = NextPart ?? this;

				string name = args.PropertyName;

				if (!string.IsNullOrEmpty(name))
				{
					if (part.IsIndexer)
					{
						if (name.Contains("["))
						{
							if (name != string.Format("{0}[{1}]", part.IndexerName, part.Content))
								return;
						}
						else if (name != part.IndexerName)
							return;
					}
					else if (name != part.Content)
					{
						return;
					}
				}

				Device.BeginInvokeOnMainThread(() => _expression.Apply());
			}

			public bool TryGetValue(object source, out object value)
			{
				value = source;

				if (LastGetter != null && value != null)
				{
					if (IsIndexer)
					{
						try
						{
							value = LastGetter.Invoke(value, Arguments);
						}
						catch (TargetInvocationException ex)
						{
							if (!(ex.InnerException is KeyNotFoundException))
								throw;
							value = null;
						}
						return true;
					}
					value = LastGetter.Invoke(value, Arguments);
					return true;
				}

				return false;
			}
		}
	}
}