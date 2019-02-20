using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Material
{
	public static class DatePicker
	{
		public static readonly BindableProperty PlaceholderProperty = BindableProperty.CreateAttached("Placeholder", typeof(string), typeof(DatePicker), String.Empty);

		public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.CreateAttached("PlaceholderColor", typeof(Color), typeof(DatePicker), Color.Default);

		public static string GetPlaceholder(BindableObject bindable)
		{
			return (string)bindable.GetValue(PlaceholderProperty);
		}
		public static void SetPlaceholder(BindableObject bindable, string value)
		{
			bindable.SetValue(PlaceholderProperty, value);
		}


		public static Color GetPlaceholderColor(BindableObject bindable)
		{
			return (Color)bindable.GetValue(PlaceholderColorProperty);
		}
		public static void SetPlaceholderColor(BindableObject bindable, Color value)
		{
			bindable.SetValue(PlaceholderColorProperty, value);
		}
	}
}
