using System;
using System.Collections.Generic;
using static Xamarin.Forms.Core.Markup.Markup;

namespace Xamarin.Forms.Markup
{
	public static class BindableObjectMultiBindExtensions
	{
		/// <summary>Bind to a specified property with 2 bindings and an inline convertor</summary>
		public static TBindable Bind<TBindable, TSource1, TSource2, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			BindingBase binding1,
			BindingBase binding2,
			Func<ValueTuple<TSource1, TSource2>, TDest> convert = null,
			Func<TDest, ValueTuple<TSource1, TSource2>> convertBack = null,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default
		) where TBindable : BindableObject
		=> bindable.Bind(
			targetProperty,
			new List<BindingBase> { binding1, binding2 },
			new FuncMultiConverter<TSource1, TSource2, TDest>(convert, convertBack),
			null,
			mode,
			stringFormat,
			targetNullValue,
			fallbackValue
		);

		/// <summary>Bind to a specified property with 2 bindings, an inline convertor and a converter parameter</summary>
		public static TBindable Bind<TBindable, TSource1, TSource2, TParam, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			BindingBase binding1,
			BindingBase binding2,
			Func<ValueTuple<TSource1, TSource2>, TParam, TDest> convert = null,
			Func<TDest, TParam, ValueTuple<TSource1, TSource2>> convertBack = null,
			TParam converterParameter = default,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default
		) where TBindable : BindableObject
		=> bindable.Bind(
			targetProperty,
			new List<BindingBase> { binding1, binding2 },
			new FuncMultiConverterWithParam<TSource1, TSource2, TDest, TParam>(convert, convertBack),
			converterParameter,
			mode,
			stringFormat,
			targetNullValue,
			fallbackValue
		);

		/// <summary>Bind to a specified property with 3 bindings and an inline convertor</summary>
		public static TBindable Bind<TBindable, TSource1, TSource2, TSource3, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			BindingBase binding1,
			BindingBase binding2,
			BindingBase binding3,
			Func<ValueTuple<TSource1, TSource2, TSource3>, TDest> convert = null,
			Func<TDest, ValueTuple<TSource1, TSource2, TSource3>> convertBack = null,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default
		) where TBindable : BindableObject
		=> bindable.Bind(
			targetProperty,
			new List<BindingBase> { binding1, binding2, binding3 },
			new FuncMultiConverter<TSource1, TSource2, TSource3, TDest>(convert, convertBack),
			null,
			mode,
			stringFormat,
			targetNullValue,
			fallbackValue
		);

		/// <summary>Bind to a specified property with 3 bindings, an inline convertor and a convertor parameter</summary>
		public static TBindable Bind<TBindable, TSource1, TSource2, TSource3, TParam, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			BindingBase binding1,
			BindingBase binding2,
			BindingBase binding3,
			Func<ValueTuple<TSource1, TSource2, TSource3>, TParam, TDest> convert = null,
			Func<TDest, TParam, ValueTuple<TSource1, TSource2, TSource3>> convertBack = null,
			TParam converterParameter = default,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default
		) where TBindable : BindableObject
		=> bindable.Bind(
			targetProperty,
			new List<BindingBase> { binding1, binding2, binding3 },
			new FuncMultiConverterWithParam<TSource1, TSource2, TSource3, TDest, TParam>(convert, convertBack),
			converterParameter,
			mode,
			stringFormat,
			targetNullValue,
			fallbackValue
		);

		/// <summary>Bind to a specified property with 4 bindings and an inline convertor</summary>
		public static TBindable Bind<TBindable, TSource1, TSource2, TSource3, TSource4, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			BindingBase binding1,
			BindingBase binding2,
			BindingBase binding3,
			BindingBase binding4,
			Func<ValueTuple<TSource1, TSource2, TSource3, TSource4>, TDest> convert = null,
			Func<TDest, ValueTuple<TSource1, TSource2, TSource3, TSource4>> convertBack = null,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default
		) where TBindable : BindableObject
		=> bindable.Bind(
			targetProperty,
			new List<BindingBase> { binding1, binding2, binding3, binding4 },
			new FuncMultiConverter<TSource1, TSource2, TSource3, TSource4, TDest>(convert, convertBack),
			null,
			mode,
			stringFormat,
			targetNullValue,
			fallbackValue
		);

		/// <summary>Bind to a specified property with 4 bindings, an inline convertor and a converter parameter</summary>
		public static TBindable Bind<TBindable, TSource1, TSource2, TSource3, TSource4, TParam, TDest>(
			this TBindable bindable,
			BindableProperty targetProperty,
			BindingBase binding1,
			BindingBase binding2,
			BindingBase binding3,
			BindingBase binding4,
			Func<ValueTuple<TSource1, TSource2, TSource3, TSource4>, TParam, TDest> convert = null,
			Func<TDest, TParam, ValueTuple<TSource1, TSource2, TSource3, TSource4>> convertBack = null,
			TParam converterParameter = default,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default
		) where TBindable : BindableObject
		=> bindable.Bind(
			targetProperty,
			new List<BindingBase> { binding1, binding2, binding3, binding4 },
			new FuncMultiConverterWithParam<TSource1, TSource2, TSource3, TSource4, TDest, TParam>(convert, convertBack),
			converterParameter,
			mode,
			stringFormat,
			targetNullValue,
			fallbackValue
		);

		/// <summary>Bind to a specified property with multiple bindings and a multi convertor</summary>
		public static TBindable Bind<TBindable>(
			this TBindable bindable,
			BindableProperty targetProperty,
			IList<BindingBase> bindings,
			IMultiValueConverter converter,
			object converterParameter = default,
			BindingMode mode = BindingMode.Default,
			string stringFormat = null,
			object targetNullValue = null,
			object fallbackValue = null
		) where TBindable : BindableObject
		{
			VerifyExperimental();
			bindable.SetBinding(targetProperty, new MultiBinding {
				Bindings = bindings,
				Converter = converter,
				ConverterParameter = converterParameter,
				Mode = mode,
				StringFormat = stringFormat,
				TargetNullValue = targetNullValue,
				FallbackValue = fallbackValue
			});
			return bindable;
		}
	}
}