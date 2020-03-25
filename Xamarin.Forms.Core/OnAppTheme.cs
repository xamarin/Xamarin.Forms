namespace Xamarin.Forms
{
	public class OnAppTheme<T>
	{
		T _light;
		T _dark;
		T _default;
		bool _isLightSet;
		bool _isDarkSet;
		bool _isDefaultSet;

		public T Light
		{
			get => _light;
			set
			{
				_light = value;
				_isLightSet = true;
			}
		}
		public T Dark
		{
			get => _dark;
			set
			{
				_dark = value;
				_isDarkSet = true;
			}
		}
		public T Default
		{
			get => _default;
			set
			{
				_default = value;
				_isDefaultSet = true;
			}
		}

		public static implicit operator T(OnAppTheme<T> onAppTheme)
		{
			switch (Application.Current?.RequestedTheme)
			{
				default:
				case AppTheme.Light:
					return onAppTheme._isLightSet ? onAppTheme.Light : (onAppTheme._isDefaultSet ? onAppTheme.Default : default(T));
				case AppTheme.Dark:
					return onAppTheme._isDarkSet ? onAppTheme.Dark : (onAppTheme._isDefaultSet ? onAppTheme.Default : default(T));
			}
		}
	}
}