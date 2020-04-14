using System;

namespace Xamarin.Forms
{
	public class OnAppTheme<T> : BindableObject
	{
		public OnAppTheme()
		{
			Application.Current.RequestedThemeChanged += RequestedThemeChanged;
		}

		public static readonly BindableProperty LightProperty = BindableProperty.Create(nameof(Light), typeof(T), typeof(OnAppTheme<T>), default(T), propertyChanged: (bo, __, ___) => UpdateActualValue(bo));

		public T Light
		{
			get => (T)GetValue(LightProperty);
			set => SetValue(LightProperty, value);
		}

		public static readonly BindableProperty DarkProperty = BindableProperty.Create(nameof(Dark), typeof(T), typeof(OnAppTheme<T>), default(T), propertyChanged: (bo, __, ___) => UpdateActualValue(bo));

		public T Dark
		{
			get => (T)GetValue(DarkProperty);
			set => SetValue(DarkProperty, value);
		}

		public static readonly BindableProperty DefaultProperty = BindableProperty.Create(nameof(Default), typeof(T), typeof(OnAppTheme<T>), default(T), propertyChanged: (bo, __, ___) => UpdateActualValue(bo));

		public T Default
		{
			get => (T)GetValue(DefaultProperty);
			set => SetValue(DefaultProperty, value);
		}

		public static implicit operator T(OnAppTheme<T> onAppTheme)
		{
			switch (Application.Current?.RequestedTheme)
			{
				default:
				case OSAppTheme.Light:
					return onAppTheme.IsSet(LightProperty) ? onAppTheme.Light : (onAppTheme.IsSet(DefaultProperty) ? onAppTheme.Default : default(T));
				case OSAppTheme.Dark:
					return onAppTheme.IsSet(DarkProperty) ? onAppTheme.Dark : (onAppTheme.IsSet(DefaultProperty) ? onAppTheme.Default : default(T));
			}
		}

		private T _value;
		public T Value
		{
			get => _value;
			private set
			{
				_value = value;
				OnPropertyChanged();
			}
		}

		static void UpdateActualValue(BindableObject bo)
		{
			var appThemeColor = bo as OnAppTheme<T>;
			switch (Application.Current?.RequestedTheme)
			{
				default:
				case OSAppTheme.Light:
					appThemeColor.Value = appThemeColor.IsSet(LightProperty) ? appThemeColor.Light : (appThemeColor.IsSet(DefaultProperty) ? appThemeColor.Default : default(T));
					break;
				case OSAppTheme.Dark:
					appThemeColor.Value = appThemeColor.IsSet(DarkProperty) ? appThemeColor.Dark : (appThemeColor.IsSet(DefaultProperty) ? appThemeColor.Default : default(T));
					break;
			}
		}

		void RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			UpdateActualValue(this);
		}
	}
}