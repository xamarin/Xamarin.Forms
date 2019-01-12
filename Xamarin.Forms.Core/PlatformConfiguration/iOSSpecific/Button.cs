namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Button;

	public static class Button
	{
		#region BorderAdjustsPadding
		public static readonly BindableProperty BorderAdjustsPaddingProperty = BindableProperty.Create("BorderAdjustsPadding", typeof(bool), typeof(Button), false);

		public static bool GetBorderAdjustsPadding(BindableObject element) =>
			(bool)element.GetValue(BorderAdjustsPaddingProperty);

		public static void SetBorderAdjustsPadding(BindableObject element, bool value) =>
			element.SetValue(BorderAdjustsPaddingProperty, value);

		public static bool GetBorderAdjustsPadding(this IPlatformElementConfiguration<iOS, FormsElement> config) =>
			GetBorderAdjustsPadding(config.Element);

		public static IPlatformElementConfiguration<iOS, FormsElement> SetBorderAdjustsPadding(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetBorderAdjustsPadding(config.Element, value);
			return config;
		}
		#endregion
	}
}
