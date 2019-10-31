using System;

namespace Xamarin.Forms.Markup
{
	public static class ElementExtensions
	{
		const string bindingContextPropertyName = ".";

		public static TElement Bind<TElement>(
			this TElement element,
			BindableProperty targetProperty,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TElement : Element
		{
			if (source != null || converterParameter != null)
				element.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				element.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
			return element;
		}

		public static TElement Bind<TElement, TSource, TDest>(
			this TElement element,
			BindableProperty targetProperty,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TElement : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			if (source != null || converterParameter != null)
				element.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				element.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
			return element;
		}

		public static TElement Bind<TElement>(
			this TElement element,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TElement : Element
		{
			element.Bind(
				targetProperty: DefaultBindableProperties.GetFor(element),
				sourcePropertyName: sourcePropertyName,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source
			);
			return element;
		}

		public static TElement Bind<TElement, TSource, TDest>(
			this TElement element,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TElement : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			element.Bind(
				targetProperty: DefaultBindableProperties.GetFor(element),
				sourcePropertyName: sourcePropertyName,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source
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
