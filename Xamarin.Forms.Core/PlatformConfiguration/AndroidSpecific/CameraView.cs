namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.CameraView;

	public static class CameraView
	{
		public static readonly BindableProperty SutterSoundProperty = BindableProperty.Create("SutterSound", typeof(bool), typeof(FormsElement), true);

		public static void SetSutterSound(BindableObject element, bool value)
			=> element.SetValue(SutterSoundProperty, value);

		public static bool GetSutterSound(this IPlatformElementConfiguration<Android, FormsElement> config)
			=> (bool)config.Element.GetValue(SutterSoundProperty);

		public static bool GetSutterSound(BindableObject element)
			=> (bool)element.GetValue(SutterSoundProperty);

		public static IPlatformElementConfiguration<Android, FormsElement> SetSutterSound(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			config.Element.SetValue(SutterSoundProperty, value);
			return config;
		}

		public static readonly BindableProperty MirrorFrontPreviewProperty = BindableProperty.Create("MirrorFrontPreview", typeof(bool), typeof(FormsElement), false);

		public static void SetMirrorFrontPreview(BindableObject element, bool value)
			=> element.SetValue(MirrorFrontPreviewProperty, value);

		public static bool GetMirrorFrontPreview(this IPlatformElementConfiguration<Android, FormsElement> config)
			=> (bool)config.Element.GetValue(MirrorFrontPreviewProperty);

		public static bool GetMirrorFrontPreview(BindableObject element)
			=> (bool)element.GetValue(MirrorFrontPreviewProperty);

		public static IPlatformElementConfiguration<Android, FormsElement> SetMirrorFrontPreview(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			config.Element.SetValue(MirrorFrontPreviewProperty, value);
			return config;
		}
	}
}