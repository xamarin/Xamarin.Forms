namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.Image;

	public static class Image
	{
		public static readonly BindableProperty ImageDirectoryProperty = BindableProperty.Create ("ImageDirectory", typeof (string), typeof (FormsElement), null);

		public static void SetImageDirectory(BindableObject element, string value)
		{
			element.SetValue (ImageDirectoryProperty, value);
		}

		public static string GetImageDirectory(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return (string) config.Element.GetValue (ImageDirectoryProperty);
		}

		public static string GetImageDirectory(BindableObject element)
		{
			return (string) element.GetValue (ImageDirectoryProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetImageDirectory(
			this IPlatformElementConfiguration<Windows, FormsElement> config, string value) 
		{
			config.Element.SetValue (ImageDirectoryProperty, value);
			return config;
		}
	}

}