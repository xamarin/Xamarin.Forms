
namespace Xamarin.Forms
{
	public interface IElementConfiguration<TElement> where TElement : Element
	{
		IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform;
	}
}
