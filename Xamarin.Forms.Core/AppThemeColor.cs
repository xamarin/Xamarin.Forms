namespace Xamarin.Forms
{
	public class AppThemeColor
	{
		Color _light;
		Color _dark;
		Color _default;
		bool _isLightSet;
		bool _isDarkSet;
		bool _isDefaultSet;

		public Color Light
		{
			get => _light;
			set
			{
				_light = value;
				_isLightSet = true;
			}
		}
		public Color Dark
		{
			get => _dark;
			set
			{
				_dark = value;
				_isDarkSet = true;
			}
		}
		public Color Default
		{
			get => _default;
			set
			{
				_default = value;
				_isDefaultSet = true;
			}
		}

		public static implicit operator Color(AppThemeColor appThemeColor)
		{
			switch (((IAppThemeProvider)Application.Current).RequestedTheme)
			{
				default:
				case AppTheme.Light:
					return appThemeColor._isLightSet ? appThemeColor.Light : (appThemeColor._isDefaultSet ? appThemeColor.Default : default);
				case AppTheme.Dark:
					return appThemeColor._isDarkSet ? appThemeColor.Dark : (appThemeColor._isDefaultSet ? appThemeColor.Default : default);
			}
		}
	}
}