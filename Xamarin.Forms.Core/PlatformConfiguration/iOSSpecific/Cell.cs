namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.Cell;

	public static class Cell
	{
		public static readonly BindableProperty DefaultBackgroundColorProperty = BindableProperty.Create(nameof(DefaultBackgroundColor), typeof(Color), typeof(Cell), Color.Default);

		public static readonly BindableProperty ContextActionDisplayProperty = BindableProperty.Create(nameof(ContextActionDisplay), typeof(ContextActionDisplay), typeof(Cell), ContextActionDisplay.Text);

		public static Color GetDefaultBackgroundColor(BindableObject element)
			=> (Color) element.GetValue(DefaultBackgroundColorProperty);

		public static void SetDefaultBackgroundColor(BindableObject element, Color value)
			=> element.SetValue(DefaultBackgroundColorProperty, value);

		public static Color DefaultBackgroundColor(this IPlatformElementConfiguration<iOS, FormsElement> config)
			=> GetDefaultBackgroundColor(config.Element);

		public static IPlatformElementConfiguration<iOS, FormsElement> SetDefaultBackgroundColor(this IPlatformElementConfiguration<iOS, FormsElement> config, Color value)
		{
			SetDefaultBackgroundColor(config.Element, value);
			return config;
		}

		public static ContextActionDisplay GetContextActionDisplay(BindableObject element)
		   => (ContextActionDisplay)element.GetValue(ContextActionDisplayProperty);

		public static void SetContextActionDisplay(BindableObject element, ContextActionDisplay value)
		   => element.SetValue(ContextActionDisplayProperty, value);

		public static ContextActionDisplay DefaultContextActionCellDisplay(this IPlatformElementConfiguration<iOS, FormsElement> config)
		   => GetContextActionDisplay(config.Element);

		public static IPlatformElementConfiguration<iOS, FormsElement> SetContextActionDisplay(this IPlatformElementConfiguration<iOS, FormsElement> config, ContextActionDisplay value)
		{
			SetContextActionDisplay(config.Element, value);
			return config;
		}

		public enum ContextActionDisplay
		{
			Text,
			Icon
		}
	}
}
