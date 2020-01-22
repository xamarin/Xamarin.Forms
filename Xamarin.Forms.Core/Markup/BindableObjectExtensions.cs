﻿using System;

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
			bindable.SetBinding(
				targetProperty, 
				new Binding(path, mode, converter, converterParameter, stringFormat, source)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
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
			var converter = new FuncConverter<TSource, object, TDest>(convert, convertBack);
			bindable.SetBinding(
				targetProperty,
				new Binding(path, mode, converter, converterParameter, stringFormat, source)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
			return bindable;
		}

		/// <summary>Bind to a specified property with inline conversion and conversion parameter</summary>
		public static TBindable Bind<TBindable, TSource, TParam, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TParam, TDest> convert = null,
			Func<TDest, TParam, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			var converter = new FuncConverter<TSource, TParam, TDest>(convert, convertBack);
			bindable.SetBinding(
				targetProperty,
				new Binding(path, mode, converter, converterParameter, stringFormat, source)
				{
					TargetNullValue = targetNullValue,
					FallbackValue = fallbackValue
				});
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
				DefaultBindableProperties.GetFor(bindable),
				path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
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
			var converter = new FuncConverter<TSource, object, TDest>(convert, convertBack);
			bindable.Bind(
				DefaultBindableProperties.GetFor(bindable),
				path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
			);
			return bindable;
		}

		/// <summary>Bind to the default property with inline conversion and conversion parameter</summary>
		public static TBindable Bind<TBindable, TSource, TParam, TDest>(
			this TBindable bindable,
			string path = bindingContextPath,
			BindingMode mode = BindingMode.Default,
			Func<TSource, TParam, TDest> convert = null,
			Func<TDest, TParam, TSource> convertBack = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			var converter = new FuncConverter<TSource, TParam, TDest>(convert, convertBack);
			bindable.Bind(
				DefaultBindableProperties.GetFor(bindable),
				path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
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