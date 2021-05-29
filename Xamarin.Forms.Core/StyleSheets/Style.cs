using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.StyleSheets
{
	sealed class Style
	{
		Style()
		{
		}

		public IDictionary<string, string> Declarations { get; set; } = new Dictionary<string, string>();
		Dictionary<KeyValuePair<string, string>, object> convertedValues = new Dictionary<KeyValuePair<string, string>, object>();

		public static Style Parse(CssReader reader, char stopChar = '\0')
		{
			Style style = new Style();
			string propertyName = null, propertyValue = null;

			int p;
			reader.SkipWhiteSpaces();
			bool readingName = true;
			while ((p = reader.Peek()) > 0)
			{
				switch (unchecked((char)p))
				{
					case ':':
						reader.Read();
						readingName = false;
						reader.SkipWhiteSpaces();
						break;
					case ';':
						reader.Read();
						if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
							style.Declarations.Add(propertyName, propertyValue);
						propertyName = propertyValue = null;
						readingName = true;
						reader.SkipWhiteSpaces();
						break;
					default:
						if ((char)p == stopChar)
							return style;

						if (readingName)
						{
							propertyName = reader.ReadIdent();
							if (propertyName == null)
								throw new Exception();
						}
						else
							propertyValue = reader.ReadUntil(stopChar, ';', ':');
						break;
				}
			}
			return style;
		}

		public void Apply(VisualElement styleable, bool inheriting = false)
		{
			if (styleable == null)
				throw new ArgumentNullException(nameof(styleable));

			foreach (var decl in Declarations)
			{
				var property = ((IStylable)styleable).GetProperty(decl.Key, inheriting);
				if (property == null)
					continue;
				if (string.Equals(decl.Value, "initial", StringComparison.OrdinalIgnoreCase))
					styleable.ClearValue(property, fromStyle: true);
				else
				{
					object value;
					if (!convertedValues.TryGetValue(decl, out value))
						convertedValues[decl] = (value = Convert(styleable, decl.Value, property));
					styleable.SetValue(property, value, fromStyle: true);
				}
			}

			foreach (var child in styleable.LogicalChildrenInternal)
			{
				switch (child)
				{
					case VisualElement ve:
						Apply(ve, inheriting: true);

						break;
					case Cell cell:
						Apply(cell, inheriting: true);

						break;
					default:
						break;
				}
			}
		}

		public void Apply(Cell styleable, bool inheriting = false)
		{
			if (styleable == null)
				throw new ArgumentNullException(nameof(styleable));

			foreach (var decl in Declarations)
			{
				var property = ((IStylable)styleable).GetProperty(decl.Key, inheriting);
				if (property == null)
					continue;
				if (string.Equals(decl.Value, "initial", StringComparison.OrdinalIgnoreCase))
					styleable.ClearValue(property, fromStyle: true);
				else
				{
					object value;
					if (!convertedValues.TryGetValue(decl, out value))
						convertedValues[decl] = (value = Convert(styleable, decl.Value, property));
					styleable.SetValue(property, value, fromStyle: true);
				}
			}

			foreach (var child in styleable.LogicalChildrenInternal)
			{
				switch (child)
				{
					case VisualElement ve:
						Apply(ve, inheriting: true);

						break;
					case Cell cell:
						Apply(cell, inheriting: true);

						break;
					default:
						break;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static object Convert(object target, object value, BindableProperty property)
		{
			var serviceProvider = new StyleSheetServiceProvider(target, property);
			Func<MemberInfo> minforetriever =
				() =>
				{
					MemberInfo minfo = null;
					try
					{
						minfo = property.DeclaringType.GetRuntimeProperty(property.PropertyName);
					}
					catch (AmbiguousMatchException e)
					{
						throw new XamlParseException($"Multiple properties with name '{property.DeclaringType}.{property.PropertyName}' found.", serviceProvider, innerException: e);
					}
					if (minfo != null)
						return minfo;
					try
					{
						return property.DeclaringType.GetRuntimeMethod("Get" + property.PropertyName, new[] { typeof(BindableObject) });
					}
					catch (AmbiguousMatchException e)
					{
						throw new XamlParseException($"Multiple methods with name '{property.DeclaringType}.Get{property.PropertyName}' found.", serviceProvider, innerException: e);
					}
				};
			var ret = value.ConvertTo(property.ReturnType, minforetriever, serviceProvider, out Exception exception);
			if (exception != null)
				throw exception;
			return ret;
		}

		public void UnApply(IStylable styleable)
		{
			throw new NotImplementedException();
		}
	}
}