using CurrentPlatform = Xamarin.Forms.PlatformConfiguration.Tizen;

namespace Xamarin.Forms.Platform.Tizen
{
	public static class PlatformConfigurationExtensions
	{
		public static IPlatformElementConfiguration<CurrentPlatform, T> OnThisPlatform<T>(this T element)
			where T : Element, IFastElementConfiguration<T>
		{
			return (element).On<CurrentPlatform>();
		}
	}
}
