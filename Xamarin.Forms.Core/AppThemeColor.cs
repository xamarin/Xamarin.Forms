using System;

namespace Xamarin.Forms
{
	public class AppThemeColor : BindableObject, IDisposable
	{
		public AppThemeColor()
		{
			Application.Current.RequestedThemeChanged += RequestedThemeChanged;
		}

		public static readonly BindableProperty LightProperty = BindableProperty.Create(nameof(Light), typeof(Color), typeof(AppThemeColor), default(Color), propertyChanged: (bo, __, ___) => UpdateActualValue(bo));

		public Color Light
		{
			get => (Color)GetValue(LightProperty);
			set => SetValue(LightProperty, value);
		}

		public static readonly BindableProperty DarkProperty = BindableProperty.Create(nameof(Dark), typeof(Color), typeof(AppThemeColor), default(Color), propertyChanged: (bo, __, ___) => UpdateActualValue(bo));

		public Color Dark
		{
			get => (Color)GetValue(DarkProperty);
			set => SetValue(DarkProperty, value);
		}

		public static readonly BindableProperty DefaultProperty = BindableProperty.Create(nameof(Default), typeof(Color), typeof(AppThemeColor), default(Color), propertyChanged: (bo, __, ___) => UpdateActualValue(bo));

		public Color Default
		{
			get => (Color)GetValue(DefaultProperty);
			set => SetValue(DefaultProperty, value);
		}

		private Color _actualValue;
		public Color ActualValue
		{
			get => _actualValue;
			private set
			{
				_actualValue = value;
				OnPropertyChanged();
			}
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

		public void Dispose()
		{
			Application.Current.RequestedThemeChanged -= RequestedThemeChanged;
		}

		static void UpdateActualValue(BindableObject bo)
		{
			var appThemeColor = bo as AppThemeColor;
			switch (Application.Current?.RequestedTheme)
			{
				default:
				case AppTheme.Light:
					appThemeColor.ActualValue = appThemeColor.IsSet(LightProperty) ? appThemeColor.Light : (appThemeColor.IsSet(DefaultProperty) ? appThemeColor.Default : default(Color));
					break;
				case AppTheme.Dark:
					appThemeColor.ActualValue = appThemeColor.IsSet(DarkProperty) ? appThemeColor.Dark : (appThemeColor.IsSet(DefaultProperty) ? appThemeColor.Default : default(Color));
					break;
			}
		}

		void RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			UpdateActualValue(this);
		}
	}
}