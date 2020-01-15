using System;

namespace Xamarin.Forms.Markup
{
	public static class BindableObjectExtensions
	{
		const string bindingContextPath = Binding.SelfPath;

		/// <summary>Bind to a specified property</summary>
		public static TBindable Bind<TBindable>(
			this TBindable bindable,
			BindableProperty targetProperty,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			if (source != null || converterParameter != null || targetNullValue != null || fallbackValue != null)
				bindable.SetBinding(targetProperty, new Binding(
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
				bindable.SetBinding(targetProperty, path, mode, converter, stringFormat);
			return bindable;
		}

		/// <summary>Bind to a specified property with inline conversion</summary>
		public static TBindable Bind<TBindable, TSource, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			if (source != null || converterParameter != null || targetNullValue != null || fallbackValue != null)
				bindable.SetBinding(targetProperty, new Binding(
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
				bindable.SetBinding(targetProperty, path, mode, converter, stringFormat);
			return bindable;
		}

		/// <summary>Bind to the default property</summary>
		public static TBindable Bind<TBindable>(
			this TBindable bindable,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			bindable.Bind(
				targetProperty: DefaultBindableProperties.GetFor(bindable),
				path: path,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
			return bindable;
		}

		/// <summary>Bind to the default property with inline conversion</summary>
		public static TBindable Bind<TBindable, TSource, TDest>(
			this TBindable bindable,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TDest> convert = null,
			Func<TDest, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			var converter = new FuncConverter<TSource, TDest>(convert, convertBack);
			bindable.Bind(
				targetProperty: DefaultBindableProperties.GetFor(bindable),
				path: path,
				mode: mode,
				converter: converter,
				converterParameter: converterParameter,
				stringFormat: stringFormat,
				source: source,
				targetNullValue: targetNullValue,
				fallbackValue: fallbackValue
			);
			return bindable;
		}

		public static TBindable Assign<TBindable, TVariable>(this TBindable bindable, out TVariable variable)
			where TBindable : BindableObject, TVariable
		{
			variable = bindable;
			return bindable;
		}

		public static TBindable Invoke<TBindable>(this TBindable bindable, Action<TBindable> action) where TBindable : BindableObject
		{
			action?.Invoke(bindable);
			return bindable;
		}
	}
}
