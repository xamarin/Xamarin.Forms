
namespace Xamarin.Forms
{
	public interface IFastElementConfiguration<TElement> where TElement : Element
	{
		IPlatformElementConfiguration<T, TElement> On<T>() where T : IConfigPlatform;
	}
}
