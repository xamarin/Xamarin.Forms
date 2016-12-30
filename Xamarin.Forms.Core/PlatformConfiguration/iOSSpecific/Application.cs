namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Application;

	public static class Application
	{
		public static readonly BindableProperty PageOverlayColorWhenPushingModalProperty = BindableProperty.Create("PageOverlayColorWhenPushingModal", typeof(Color?), typeof(Application));

		public static Color? GetPageOverlayColorWhenPushingModal(BindableObject element)
		{
			return (Color?)element.GetValue(PageOverlayColorWhenPushingModalProperty);
		}

		public static void SetPageOverlayColorWhenPushingModal(BindableObject element, Color? value)
		{
			element.SetValue(PageOverlayColorWhenPushingModalProperty, value);
		}

		public static Color? PageOverlayColorWhenPushingModal(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetPageOverlayColorWhenPushingModal(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetPageOverlayColorWhenPushingModal(this IPlatformElementConfiguration<iOS, FormsElement> config, Color? value)
		{
			SetPageOverlayColorWhenPushingModal(config.Element, value);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> EnablePageOverlayColorWhenPushingModal(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			var pageOverlayColor = GetPageOverlayColorWhenPushingModal(config.Element);
			SetPageOverlayColorWhenPushingModal(config.Element, pageOverlayColor ?? Color.White);
			return config;
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> DisablePageOverlayColorWhenPushingModal(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			SetPageOverlayColorWhenPushingModal(config.Element, null);
			return config;
		}
	}
}