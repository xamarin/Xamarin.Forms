
namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using Xamarin.Forms.PlatformConfiguration;
	using FormsElement = Forms.Page;

	public static class Page
	{
		public static bool AnyCutoutModeSet { get; private set; }

		public static readonly BindableProperty CutoutModeProperty =
			BindableProperty.Create("CutoutMode", typeof(CutoutMode), typeof(Page), CutoutMode.Default);

		public static bool IsDefaultCutoutMode(this IPlatformElementConfiguration<Android, FormsElement> config) =>
			GetCutoutMode(config.Element) == CutoutMode.Default;

		public static CutoutMode GetCutoutMode(BindableObject element) =>
			(CutoutMode)element.GetValue(CutoutModeProperty);

		public static void SetCutoutMode(BindableObject element, CutoutMode value)
		{
			AnyCutoutModeSet = true;
			element.SetValue(CutoutModeProperty, value);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> SetCutoutMode(this IPlatformElementConfiguration<Android, FormsElement> config, CutoutMode value)
		{
			SetCutoutMode(config.Element, value);
			return config;
		}
	}
}
