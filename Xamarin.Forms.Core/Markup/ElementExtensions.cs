using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms.Markup
{
	public static class ElementExtensions
	{
		const string bindingContextPropertyName = ".";

		public static TView Bind<TView>(
			this TView view,
			BindableProperty targetProperty,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TView : Element
		{
			if (source != null || converterParameter != null)
				view.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				view.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
			return view;
		}

		public static TView Bind<TView, TSource, TDest>(
			this TView view,
			BindableProperty targetProperty,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TView : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			if (source != null || converterParameter != null)
				view.SetBinding(targetProperty, new Binding(
					path: sourcePropertyName,
					mode: mode,
					converter: converter,
					converterParameter: converterParameter,
					stringFormat: stringFormat,
					source: source
				));
			else
				view.SetBinding(targetProperty, sourcePropertyName, mode, converter, stringFormat);
			return view;
		}

		public static TView Bind<TView>(
			this TView view,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TView : Element
		{
			view.Bind(
				targetProperty: DefaultBindableProperties.GetFor(view),
				sourcePropertyName: sourcePropertyName,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source
			);
			return view;
		}

		public static TView Bind<TView, TSource, TDest>(
			this TView view,
			string sourcePropertyName = bindingContextPropertyName,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null
		) where TView : Element
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			view.Bind(
				targetProperty: DefaultBindableProperties.GetFor(view),
				sourcePropertyName: sourcePropertyName,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source
			);
			return view;
		}

		public static TView Assign<TView, TAssignView>(this TView view, out TAssignView variable)
			where TView : Element, TAssignView
		{
			variable = view;
			return view;
		}

		public static TView Invoke<TView>(this TView view, Action<TView> action) where TView : Element
		{
			action?.Invoke(view);
			return view;
		}

		public static TElement Effects<TElement>(this TElement element, params Effect[] effects) where TElement : Element
		{
			for (int i = 0; i < effects.Length; i++)
				element.Effects.Add(effects[i]);
			return element;
		}

	}
}
