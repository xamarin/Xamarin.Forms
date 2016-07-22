
namespace Xamarin.Forms.PlatformConfiguration
{
	public interface IElementConfiguration<out TElement> where TElement : Element
	{
		IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform;
	}
}
