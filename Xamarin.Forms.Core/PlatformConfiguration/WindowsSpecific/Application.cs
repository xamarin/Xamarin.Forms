namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.Application;

	public static class Application
	{
		public static readonly BindableProperty ImageSearchDirectoryProperty =
			BindableProperty.Create("ImageSearchDirectory", typeof(string), typeof(FormsElement), string.Empty);

		public static void SetImageSearchDirectory(BindableObject element, string value)
		{
			element.SetValue(ImageSearchDirectoryProperty, value);
		}

		public static string GetImageSearchDirectory(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (string)config.Element.GetValue(ImageSearchDirectoryProperty);
		}

		public static string GetImageSearchDirectory(BindableObject element)
		{
			return (string)element.GetValue(ImageSearchDirectoryProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetImageSearchDirectory(
			this IPlatformElementConfiguration<Windows, FormsElement> config, string value)
		{
			config.Element.SetValue(ImageSearchDirectoryProperty, value);
			return config;
		}
	}
}