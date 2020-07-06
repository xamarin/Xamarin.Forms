namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.Label;

	public enum BreakStrategyFlags
	{
		Simple = 0,
		HighQuality = 1,
		Balanced = 2
	}

	public static class Label
	{
		public static readonly BindableProperty BreakStrategyProperty = BindableProperty.Create(nameof(BreakStrategy), typeof(BreakStrategyFlags), typeof(Label), BreakStrategyFlags.Simple);

		public static BreakStrategyFlags GetBreakStrategy(BindableObject element)
		{
			return (BreakStrategyFlags)element.GetValue(BreakStrategyProperty);
		}

		public static void SetBreakStrategy(BindableObject element, BreakStrategyFlags value)
		{
			element.SetValue(BreakStrategyProperty, value);
		}

		public static BreakStrategyFlags BreakStrategy(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetBreakStrategy(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetBreakStrategy(this IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, FormsElement> config, BreakStrategyFlags value)
		{
			SetBreakStrategy(config.Element, value);
			return config;
		}
	}
}