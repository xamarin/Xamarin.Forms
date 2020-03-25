namespace Xamarin.Forms
{
	public class OnAppTheme<T> : BindableObject
	{
		public static readonly BindableProperty LightProperty = BindableProperty.Create(nameof(Light), typeof(T), typeof(OnAppTheme<T>), default(T));

		public T Light
		{
			get => (T)GetValue(LightProperty);
			set => SetValue(LightProperty, value);
		}

		public static readonly BindableProperty DarkProperty = BindableProperty.Create(nameof(Dark), typeof(T), typeof(OnAppTheme<T>), default(T));

		public T Dark
		{
			get => (T)GetValue(DarkProperty);
			set => SetValue(DarkProperty, value);
		}

		public static readonly BindableProperty DefaultProperty = BindableProperty.Create(nameof(Default), typeof(T), typeof(OnAppTheme<T>), default(T));

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
				case AppTheme.Light:
					return onAppTheme.IsSet(LightProperty) ? onAppTheme.Light : (onAppTheme.IsSet(DefaultProperty) ? onAppTheme.Default : default(T));
				case AppTheme.Dark:
					return onAppTheme.IsSet(DarkProperty) ? onAppTheme.Dark : (onAppTheme.IsSet(DefaultProperty) ? onAppTheme.Default : default(T));
			}
		}
	}
}