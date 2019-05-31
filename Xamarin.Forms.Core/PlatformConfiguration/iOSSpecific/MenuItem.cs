namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.MenuItem;

	public static class MenuItem
	{
		public static readonly BindableProperty ContextActionDisplayProperty = BindableProperty.Create(nameof(ContextActionDisplay), typeof(ContextActionDisplay), typeof(MenuItem), ContextActionDisplay.Text);

		public static ContextActionDisplay GetContextActionDisplay(BindableObject element)
			=> (ContextActionDisplay)element.GetValue(ContextActionDisplayProperty);

		public static void SetContextActionDisplay(BindableObject element, ContextActionDisplay value)
			=> element.SetValue(ContextActionDisplayProperty, value);

		public static ContextActionDisplay DefaultContextActionCellDisplay(this IPlatformElementConfiguration<iOS, FormsElement> config)
			=> GetContextActionDisplay(config.Element);

		public static ContextActionDisplay GetContextActionDisplay(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetContextActionDisplay(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetContextActionDisplay(this IPlatformElementConfiguration<iOS, FormsElement> config, ContextActionDisplay value)
		{
			SetContextActionDisplay(config.Element, value);
			return config;
		}
	}

	public enum ContextActionDisplay
	{
		Text, 
		Icon
	}
}
