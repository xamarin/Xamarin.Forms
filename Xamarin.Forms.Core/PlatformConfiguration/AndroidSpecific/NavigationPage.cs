namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.NavigationPage;

	public static class NavigationPage
	{
		public static readonly BindableProperty IsAllowingStateLossProperty =
			BindableProperty.Create(nameof(IsAllowingStateLoss), typeof(bool),
			typeof(NavigationPage), false);

		public static bool GetIsAllowingStateLoss(BindableObject element)
		{
			return (bool)element.GetValue(IsAllowingStateLossProperty);
		}

		public static void SetIsAllowingStateLoss(BindableObject element, bool value)
		{
			element.SetValue(IsAllowingStateLossProperty, value);
		}

		public static bool IsAllowingStateLoss(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetIsAllowingStateLoss(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetIsAllowingStateLoss(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetIsAllowingStateLoss(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<Android, FormsElement> EnableIsAllowingStateLoss(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			SetIsAllowingStateLoss(config.Element, true);
			return config;
		}

		public static IPlatformElementConfiguration<Android, FormsElement> DisableIsAllowingStateLoss(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			SetIsAllowingStateLoss(config.Element, false);
			return config;
		}
	}
}