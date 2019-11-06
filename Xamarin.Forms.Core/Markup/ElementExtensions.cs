using System;

namespace Xamarin.Forms.Markup
{
	public static class ElementExtensions
	{
		const string bindingContextPropertyName = ".";

		public static TElement Bind<TElement>(
			this TElement element,
			BindableProperty targetProperty,
			string path = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TElement : Element
		{
			if (source != null || converterParameter != null || targetNullValue != null || fallbackValue != null)
				element.SetBinding(targetProperty, new Binding(
					path: path,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
			else
				element.SetBinding(targetProperty, path, mode, converter, stringFormat);
			return element;
		}

		public static TElement Bind<TElement, TSource, TDest>(
			this TElement element,
			BindableProperty targetProperty,
			string path = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TElement : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			if (source != null || converterParameter != null || targetNullValue != null || fallbackValue != null)
				element.SetBinding(targetProperty, new Binding(
					path: path,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
			else
				element.SetBinding(targetProperty, path, mode, converter, stringFormat);
			return element;
		}

		public static TElement Bind<TElement>(
			this TElement element,
			string path = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TElement : Element
		{
			element.Bind(
				targetProperty: DefaultBindableProperties.GetFor(element),
				path: path,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
			return element;
		}

		public static TElement Bind<TElement, TSource, TDest>(
			this TElement element,
			string path = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TElement : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			element.Bind(
				targetProperty: DefaultBindableProperties.GetFor(element),
				path: path,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
			return element;
		}

		public static TElement Assign<TElement, TVariable>(this TElement element, out TVariable variable)
			where TElement : Element, TVariable
		{
			variable = element;
			return element;
		}

		public static TElement Invoke<TElement>(this TElement element, Action<TElement> action) where TElement : Element
		{
			action?.Invoke(element);
			return element;
		}

		public static TElement Effects<TElement>(this TElement element, params Effect[] effects) where TElement : Element
		{
			for (int i = 0; i < effects.Length; i++)
				element.Effects.Add(effects[i]);
			return element;
		}
	}
}
