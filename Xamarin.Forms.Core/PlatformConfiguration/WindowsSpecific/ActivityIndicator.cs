
namespace Xamarin.Forms.PlatformConfiguration.WindowsSpecific
{
	using FormsElement = Forms.ActivityIndicator;
	public static class ActivityIndicator
	{
		public enum ActivityIndicatorType
		{
			Bar,
			Ring
		}
		public static readonly BindableProperty ActivityIndicatorStyleProperty = BindableProperty.CreateAttached("ActivityIndicatorStyle",
			typeof(ActivityIndicatorType),
			typeof(ActivityIndicator), ActivityIndicatorType.Bar);

		public static void SetActivityIndicatorStyle(BindableObject element, ActivityIndicatorType value)
		{
			element.SetValue(ActivityIndicatorStyleProperty, value);
		}

		public static ActivityIndicatorType GetActivityIndicatorStyle(this IPlatformElementConfiguration<Windows, FormsElement> config)
		{
			return GetActivityIndicatorStyle(config.Element);
		}

		public static ActivityIndicatorType GetActivityIndicatorStyle(BindableObject element)
		{
			return (ActivityIndicatorType)element.GetValue(ActivityIndicatorStyleProperty);
		}

		public static IPlatformElementConfiguration<Windows, FormsElement> SetActivityIndicatorStyle(
			this IPlatformElementConfiguration<Windows, FormsElement> config, ActivityIndicatorType value)
		{
			SetActivityIndicatorStyle(config.Element, value);
			return config;
		}
	}


}
