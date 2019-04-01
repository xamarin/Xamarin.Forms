
namespace Xamarin.Forms
{
	// Don't make this generic covariant as it causes UWP performance to tank
	public interface IElementConfiguration<out TElement> where TElement : Element
	{
		IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform;
	}
}
