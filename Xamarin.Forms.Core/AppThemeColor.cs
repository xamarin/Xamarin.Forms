namespace Xamarin.Forms
{
	public class AppThemeColor : BindableObject
	{
		public static readonly BindableProperty LightProperty = BindableProperty.Create(nameof(Light), typeof(Color), typeof(AppThemeColor), default(Color));

		public Color Light
		{
			get => (Color)GetValue(LightProperty);
			set => SetValue(LightProperty, value);
		}

		public static readonly BindableProperty DarkProperty = BindableProperty.Create(nameof(Dark), typeof(Color), typeof(AppThemeColor), default(Color));

		public Color Dark
		{
			get => (Color)GetValue(DarkProperty);
			set => SetValue(DarkProperty, value);
		}

		public static readonly BindableProperty DefaultProperty = BindableProperty.Create(nameof(Default), typeof(Color), typeof(AppThemeColor), default(Color));

		public Color Default
		{
			get => (Color)GetValue(DefaultProperty);
			set => SetValue(DefaultProperty, value);
		}

		public static implicit operator Color(AppThemeColor appThemeColor)
		{
			switch (Application.Current?.RequestedTheme)
			{
				default:
				case AppTheme.Light:
					return appThemeColor.IsSet(LightProperty) ? appThemeColor.Light : (appThemeColor.IsSet(DefaultProperty) ? appThemeColor.Default : default(Color));
				case AppTheme.Dark:
					return appThemeColor.IsSet(DarkProperty) ? appThemeColor.Dark : (appThemeColor.IsSet(DefaultProperty) ? appThemeColor.Default : default(Color));
			}
		}
	}
}