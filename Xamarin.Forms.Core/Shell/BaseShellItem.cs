using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public class BaseShellItem : NavigableElement
	{
		#region PropertyKeys

		internal static readonly BindablePropertyKey IsCheckedPropertyKey = BindableProperty.CreateReadOnly(nameof(IsChecked), typeof(bool), typeof(BaseShellItem), false);

		#endregion PropertyKeys

		public static readonly BindableProperty FlyoutIconProperty =
			BindableProperty.Create(nameof(FlyoutIcon), typeof(ImageSource), typeof(BaseShellItem), null, BindingMode.OneTime);

		public static readonly BindableProperty IconProperty =
			BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(BaseShellItem), null, BindingMode.OneWay,
				propertyChanged: OnIconChanged);

		public static readonly BindableProperty IsCheckedProperty = IsCheckedPropertyKey.BindableProperty;

		public static readonly BindableProperty IsEnabledProperty =
			BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(BaseShellItem), true, BindingMode.OneWay);

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(BaseShellItem), null, BindingMode.OneTime);

		public ImageSource FlyoutIcon
		{
			get { return (ImageSource)GetValue(FlyoutIconProperty); }
			set { SetValue(FlyoutIconProperty, value); }
		}

		public ImageSource Icon
		{
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public bool IsChecked => (bool)GetValue(IsCheckedProperty);

		public bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		public string Route
		{
			get { return Routing.GetRoute(this); }
			set { Routing.SetRoute(this, value); }
		}

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		private static void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (newValue == null || bindable.IsSet(FlyoutIconProperty))
				return;

			var shellItem = (BaseShellItem)bindable;
			shellItem.FlyoutIcon = (ImageSource)newValue;
		}
	}
}