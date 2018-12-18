using System;

namespace Xamarin.Forms
{
	public class CheckBoxCell : Cell
	{
		public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create("IsCheckedProperty", typeof(bool), typeof(CheckBoxCell), false, propertyChanged: (obj, oldValue, newValue) =>
		{
			((CheckBoxCell)obj).IsCheckedChanged?.Invoke(obj, new CheckedChangedEventArgs((bool)newValue));
		}, defaultBindingMode: BindingMode.TwoWay);

		public bool IsChecked
		{
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}

		public static readonly BindableProperty CheckedColorProperty = BindableProperty.Create(nameof(CheckedColor), typeof(Color), typeof(CheckBoxCell), Color.Default);

		public Color CheckedColor
		{
			get { return (Color)GetValue(CheckedColorProperty); }
			set { SetValue(CheckedColorProperty, value); }
		}

		public static readonly BindableProperty UncheckedColorProperty = BindableProperty.Create(nameof(UncheckedColor), typeof(Color), typeof(CheckBoxCell), Color.Default);

		public Color UncheckedColor
		{
			get { return (Color)GetValue(UncheckedColorProperty); }
			set { SetValue(UncheckedColorProperty, value); }
		}

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CheckBoxCell), default(string));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public event EventHandler<CheckedChangedEventArgs> IsCheckedChanged;
	}
}