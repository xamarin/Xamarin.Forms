
namespace Xamarin.Forms.PlatformConfiguration
{
	public interface IPlatformElementConfiguration<out TPlatform, out TElement> : IConfigElement<TElement>
			where TPlatform : IConfigPlatform
	 		where TElement : Element
	{
	}
}
