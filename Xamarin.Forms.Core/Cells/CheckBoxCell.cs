using System;

namespace Xamarin.Forms
{
	public class CheckBoxCell : Cell
	{
		public static readonly BindableProperty OnProperty = BindableProperty.Create("On", typeof(bool), typeof(CheckBoxCell), false, propertyChanged: (obj, oldValue, newValue) =>
		{
			((CheckBoxCell)obj).OnChanged?.Invoke(obj, new CheckedChangedEventArgs((bool)newValue));
		}, defaultBindingMode: BindingMode.TwoWay);

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(CheckBoxCell), default(string));

		public bool On
		{
			get { return (bool)GetValue(OnProperty); }
			set { SetValue(OnProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public event EventHandler<CheckedChangedEventArgs> OnChanged;
	}
}