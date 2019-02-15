namespace Xamarin.Forms.Platform.Android
{
	public static class PlatformConfigurationExtensions
	{
		public static IPlatformElementConfiguration<PlatformConfiguration.Android, T> OnThisPlatform<T>(this T element) 
			where T : Element, IFastElementConfiguration<T>
		{
			return (element).On<PlatformConfiguration.Android>();
		}
	}
}