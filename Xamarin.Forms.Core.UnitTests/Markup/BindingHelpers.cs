using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	internal static class BindingHelpers
	{
		static MethodInfo getContextMethodInfo;
		static FieldInfo bindingFieldInfo;

		internal static void AssertBindingExists(
			BindableObject bindable,
			BindableProperty targetProperty,
			string path = ".",
			BindingMode mode = BindingMode.Default,
			bool assertConverterInstanceIsAnyNotNull = false,
			IValueConverter converter = null,
			object converterParameter = null,
			string stringFormat = null,
			object source = null,
			object targetNullValue = default,
			object fallbackValue = default,
			Action<IValueConverter> assertConvert = null
		) => AssertBindingExists<object, object>(
				bindable, targetProperty, path, mode, assertConverterInstanceIsAnyNotNull, converter, converterParameter,
				stringFormat, source, targetNullValue, fallbackValue, assertConvert
		);

		internal static void AssertBindingExists<TDest>(
			BindableObject bindable,
			BindableProperty targetProperty,
			string path = ".",
			BindingMode mode = BindingMode.Default,
			bool assertConverterInstanceIsAnyNotNull = false,
			IValueConverter converter = null,
			string stringFormat = null,
			object source = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default,
			Action<IValueConverter> assertConvert = null
		) => AssertBindingExists<TDest, object>(
				bindable, targetProperty, path, mode, assertConverterInstanceIsAnyNotNull, converter, null,
				stringFormat, source, targetNullValue, fallbackValue, assertConvert
		);

		internal static void AssertBindingExists<TDest, TParam>(
			BindableObject bindable,
			BindableProperty targetProperty,
			string path = ".",
			BindingMode mode = BindingMode.Default,
			bool assertConverterInstanceIsAnyNotNull = false,
			IValueConverter converter = null,
			TParam converterParameter = default,
			string stringFormat = null,
			object source = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default,
			Action<IValueConverter> assertConvert = null
		)
		{
			var binding = BindingHelpers.GetBinding(bindable, targetProperty);
			Assert.That(binding, Is.Not.Null);
			Assert.That(binding.Path, Is.EqualTo(path));
			Assert.That(binding.Mode, Is.EqualTo(mode));
			if (assertConverterInstanceIsAnyNotNull)
				Assert.That(binding.Converter, Is.Not.Null);
			else
				Assert.That(binding.Converter, Is.EqualTo(converter));
			Assert.That(binding.ConverterParameter, Is.EqualTo(converterParameter));
			Assert.That(binding.StringFormat, Is.EqualTo(stringFormat));
			Assert.That(binding.Source, Is.EqualTo(source));
			Assert.That(binding.TargetNullValue, Is.EqualTo(targetNullValue));
			Assert.That(binding.FallbackValue, Is.EqualTo(fallbackValue));

			assertConvert?.Invoke(binding.Converter);
		}

		internal static void AssertBindingExists<TDest>(
			BindableObject bindable,
			BindableProperty targetProperty,
			IList<BindingBase> bindings,
			IMultiValueConverter converter = null,
			BindingMode mode = BindingMode.Default,
			bool assertConverterInstanceIsAnyNotNull = false,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default,
			Action<IMultiValueConverter> assertConvert = null
		) => AssertBindingExists<TDest, object>(
			bindable, targetProperty, bindings, converter, null, mode, assertConverterInstanceIsAnyNotNull,
			stringFormat, targetNullValue, fallbackValue, assertConvert);


		internal static void AssertBindingExists<TDest, TParam>(
			BindableObject bindable,
			BindableProperty targetProperty,
			IList<BindingBase> bindings,
			IMultiValueConverter converter = null,
			TParam converterParameter = default,
			BindingMode mode = BindingMode.Default,
			bool assertConverterInstanceIsAnyNotNull = false,
			string stringFormat = null,
			TDest targetNullValue = default,
			TDest fallbackValue = default,
			Action<IMultiValueConverter> assertConvert = null
		)
		{
			var binding = BindingHelpers.GetMultiBinding(bindable, targetProperty);
			Assert.That(binding, Is.Not.Null);
			Assert.That(binding.Bindings.SequenceEqual(bindings), Is.True);
			Assert.That(binding.Mode, Is.EqualTo(mode));
			if (assertConverterInstanceIsAnyNotNull)
				Assert.That(binding.Converter, Is.Not.Null);
			else
				Assert.That(binding.Converter, Is.EqualTo(converter));
			Assert.That(binding.ConverterParameter, Is.EqualTo(converterParameter));
			Assert.That(binding.StringFormat, Is.EqualTo(stringFormat));
			Assert.That(binding.TargetNullValue, Is.EqualTo(targetNullValue));
			Assert.That(binding.FallbackValue, Is.EqualTo(fallbackValue));

			assertConvert?.Invoke(binding.Converter);
		}

		internal static Binding GetBinding(BindableObject bindable, BindableProperty property) => GetBindingBase(bindable, property) as Binding;

		internal static MultiBinding GetMultiBinding(BindableObject bindable, BindableProperty property) => GetBindingBase(bindable, property) as MultiBinding;

		/// <remarks>
		/// Note that we are only testing whether the Markup helpers create the correct bindings,
		/// we are not testing the binding mechanism itself; this is why it is justified to access
		/// private binding API's here for testing.
		/// </remarks>
		internal static BindingBase GetBindingBase(BindableObject bindable, BindableProperty property)
		{
			// return bindable.GetContext(property)?.Binding as Binding;
			// Both BindableObject.GetContext and BindableObject.BindablePropertyContext are private; 
			// use reflection instead of above line.

			if (getContextMethodInfo == null)
				getContextMethodInfo = typeof(BindableObject).GetMethod("GetContext", BindingFlags.NonPublic | BindingFlags.Instance);

			var context = getContextMethodInfo?.Invoke(bindable, new object[] { property });
			if (context == null)
				return null;

			if (bindingFieldInfo == null)
				bindingFieldInfo = context?.GetType().GetField("Binding");

			return bindingFieldInfo?.GetValue(context) as BindingBase;
		}

		internal static IValueConverter AssertConvert<TValue, TConvertedValue>(this IValueConverter converter, TValue value, object parameter, TConvertedValue expectedConvertedValue, bool twoWay = false, bool backOnly = false, CultureInfo culture = null)
		{
			Assert.That(converter?.Convert(value, typeof(object), parameter, culture), Is.EqualTo(backOnly ? default(TConvertedValue) : expectedConvertedValue));
			Assert.That(converter?.ConvertBack(expectedConvertedValue, typeof(object), parameter, culture), Is.EqualTo(twoWay || backOnly ? value : default(TValue)));
			return converter;
		}

		internal static IValueConverter AssertConvert<TValue, TConvertedValue>(this IValueConverter converter, TValue value, TConvertedValue expectedConvertedValue, bool twoWay = false, bool backOnly = false, CultureInfo culture = null)
			=> AssertConvert(converter, value, null, expectedConvertedValue, twoWay: twoWay, backOnly: backOnly, culture: culture);

		internal static IMultiValueConverter AssertConvert<TConvertedValue>(this IMultiValueConverter converter, object[] values, object parameter, TConvertedValue expectedConvertedValue, bool twoWay = false, bool backOnly = false, CultureInfo culture = null)
		{
			Assert.That(converter?.Convert(values, typeof(TConvertedValue), parameter, culture), Is.EqualTo(backOnly ? BindableProperty.UnsetValue : expectedConvertedValue));
			var convertedBackValues = converter?.ConvertBack(expectedConvertedValue, null, parameter, culture);
			if (twoWay || backOnly)
			{
				Assert.That(convertedBackValues.Length, Is.EqualTo(values.Length));
				for (int i = 0; i < values.Length; i++)
					Assert.That(convertedBackValues[i], Is.EqualTo(values[i]));
			}
			else
				Assert.That(convertedBackValues, Is.Null);
			return converter;
		}

		internal static IMultiValueConverter AssertConvert<TConvertedValue>(this IMultiValueConverter converter, object[] values, TConvertedValue expectedConvertedValue, bool twoWay = false, bool backOnly = false, CultureInfo culture = null)
		=> AssertConvert<TConvertedValue>(converter, values, null, expectedConvertedValue, twoWay, backOnly, culture);
	}
}
