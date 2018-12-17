using System;

namespace Xamarin.Forms
{
	public class CheckBoxCell : Cell
	{
		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsCheckedProperty", typeof(bool), typeof(CheckBoxCell), false, propertyChanged: (obj, oldValue, newValue) =>
		{
			((CheckBoxCell)obj).IsCheckedChanged?.Invoke(obj, new CheckedChangedEventArgs((bool)newValue));
		}, defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CheckBoxCell), default(string));

		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public event EventHandler<CheckedChangedEventArgs> IsCheckedChanged;
	}
}