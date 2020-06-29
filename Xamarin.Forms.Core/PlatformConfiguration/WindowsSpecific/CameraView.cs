namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.CameraView;

	public static class CameraView
	{
		public static readonly BindableProperty PhotoFolderProperty = BindableProperty.Create("PhotoFolder", typeof(string), typeof(FormsElement), "Pictures");

		public static void SetPhotoFolder(BindableObject element, string value)
			=> element.SetValue(PhotoFolderProperty, value);

		public static string GetPhotoFolder(this IPlatformElementConfiguration<Windows, FormsElement> config)
			=> (string)config.Element.GetValue(PhotoFolderProperty);

		public static string GetPhotoFolder(BindableObject element)
			=> (string)element.GetValue(PhotoFolderProperty);

		public static IPlatformElementConfiguration<Windows, FormsElement> SetPhotoFolder(this IPlatformElementConfiguration<Windows, FormsElement> config, string value)
		{
			config.Element.SetValue(PhotoFolderProperty, value);
			return config;
		}

		public static readonly BindableProperty VideoFolderProperty = BindableProperty.Create("VideoFolder", typeof(string), typeof(FormsElement), "Video");

		public static void SetVideoFolder(BindableObject element, string value)
			=> element.SetValue(VideoFolderProperty, value);

		public static string GetVideoFolder(this IPlatformElementConfiguration<Windows, FormsElement> config)
			=> (string)config.Element.GetValue(VideoFolderProperty);

		public static string GetVideoFolder(BindableObject element)
			=> (string)element.GetValue(VideoFolderProperty);

		public static IPlatformElementConfiguration<Windows, FormsElement> SetVideoFolder(this IPlatformElementConfiguration<Windows, FormsElement> config, string value)
		{
			config.Element.SetValue(VideoFolderProperty, value);
			return config;
		}
	}
}