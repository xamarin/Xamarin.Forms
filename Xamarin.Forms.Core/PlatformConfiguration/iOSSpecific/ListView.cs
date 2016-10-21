namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Forms.ListView;

	public static class ListView
	{
		public static readonly BindableProperty IsUsingAutoSizedViewCellsOnlyProperty = BindableProperty.Create(nameof(IsUsingAutoSizedViewCellsOnly), typeof(bool), typeof(ListView), false);

		public static bool GetIsUsingAutoSizedViewCellsOnly(BindableObject element)
		{
			return (bool)element.GetValue(IsUsingAutoSizedViewCellsOnlyProperty);
		}

		public static void SetIsUsingAutoSizedViewCellsOnly(BindableObject element, bool value)
		{
			element.SetValue(IsUsingAutoSizedViewCellsOnlyProperty, value);
		}

		public static bool IsUsingAutoSizedViewCellsOnly(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetIsUsingAutoSizedViewCellsOnly(config.Element);
		}

		public static IPlatformElementConfiguration<iOS, FormsElement> SetIsUsingAutoSizedViewCellsOnly(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetIsUsingAutoSizedViewCellsOnly(config.Element, value);
			return config;
		}
	}
}