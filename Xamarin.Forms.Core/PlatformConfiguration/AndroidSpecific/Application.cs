namespace Xamarin.Forms.PlatformConfiguration.AndroidSpecific
{
	using FormsElement = Forms.Application;

	public enum WindowSoftInputModeAdjust
	{
		Pan,
		Resize
	}

	public static class Application
	{
		public static readonly BindableProperty WindowSoftInputModeAdjustProperty = BindableProperty.Create(nameof(WindowSoftInputModeAdjust), typeof(WindowSoftInputModeAdjust), typeof(Application), WindowSoftInputModeAdjust.Pan);

		public static WindowSoftInputModeAdjust GetWindowSoftInputModeAdjust(BindableObject element)
		{
			return (WindowSoftInputModeAdjust)element.GetValue(WindowSoftInputModeAdjustProperty);
		}

		public static void SetWindowSoftInputModeAdjust(BindableObject element, WindowSoftInputModeAdjust value)
		{
			element.SetValue(WindowSoftInputModeAdjustProperty, value);
		}

		public static WindowSoftInputModeAdjust GetWindowSoftInputModeAdjust(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetWindowSoftInputModeAdjust(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> UseWindowSoftInputModeAdjust(this IPlatformElementConfiguration<Android, FormsElement> config, WindowSoftInputModeAdjust value)
		{
			SetWindowSoftInputModeAdjust(config.Element, value);
			return config;
		}

		public static readonly BindableProperty ShouldHandleWebViewStateOnLifecycleChangeProperty = BindableProperty.Create(nameof(ShouldHandleWebViewStateOnLifecycleChange), typeof(bool), typeof(Application), false);

		public static bool GetShouldHandleWebViewStateOnLifecycleChange(BindableObject element)
		{
			return (bool)element.GetValue(ShouldHandleWebViewStateOnLifecycleChangeProperty);
		}

		public static void SetShouldHandleWebViewStateOnLifecycleChange(BindableObject element, bool value)
		{
			element.SetValue(ShouldHandleWebViewStateOnLifecycleChangeProperty, value);
		}

		public static bool GetShouldHandleWebViewStateOnLifecycleChange(this IPlatformElementConfiguration<Android, FormsElement> config)
		{
			return GetShouldHandleWebViewStateOnLifecycleChange(config.Element);
		}

		public static IPlatformElementConfiguration<Android, FormsElement> ShouldHandleWebViewStateOnLifecycleChange(this IPlatformElementConfiguration<Android, FormsElement> config, bool value)
		{
			SetShouldHandleWebViewStateOnLifecycleChange(config.Element, value);
			return config;
		}
	}
}
