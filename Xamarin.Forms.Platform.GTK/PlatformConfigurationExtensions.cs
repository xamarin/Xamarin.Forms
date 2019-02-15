namespace Xamarin.Forms.Platform.GTK
{
	public static class PlatformConfigurationExtensions
	{
		public static IPlatformElementConfiguration<PlatformConfiguration.GTK, T> OnThisPlatform<T>(this T element)
			where T : Element, IFastElementConfiguration<T>
		{
			return (element).On<PlatformConfiguration.GTK>();
		}
	}
}
