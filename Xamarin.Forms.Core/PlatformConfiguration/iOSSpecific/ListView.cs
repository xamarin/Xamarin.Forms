namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.ListView;

	public static class ListView
	{
		public static readonly BindableProperty IsUsingDynamicViewCellsOnlyProperty = BindableProperty.Create(nameof(IsUsingDynamicViewCellsOnly), typeof(bool), typeof(ListView), false);

		public static bool GetIsUsingDynamicViewCellsOnly(BindableObject element)
		{
			return (bool)element.GetValue(IsUsingDynamicViewCellsOnlyProperty);
		}

		public static void SetIsUsingDynamicViewCellsOnly(BindableObject element, bool value)
		{
			element.SetValue(IsUsingDynamicViewCellsOnlyProperty, value);
		}

		public static bool IsUsingDynamicViewCellsOnly(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetIsUsingDynamicViewCellsOnly(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetIsUsingDynamicViewCellsOnly(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetIsUsingDynamicViewCellsOnly(config.Element, value);
			return config;
		}
	}
}