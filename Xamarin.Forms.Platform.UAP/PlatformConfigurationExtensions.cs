namespace Xamarin.Forms.Platform.UWP
{
	public static class PlatformConfigurationExtensions
	{
		public static IPlatformElementConfiguration<PlatformConfiguration.Windows, T> OnThisPlatform<T>(this T element) 
			where T : Element, IFastElementConfiguration<T>
		{
			return (element).On<PlatformConfiguration.Windows>();
		}
	}
}
