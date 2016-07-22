
namespace Xamarin.Forms.PlatformConfiguration
{
	public interface IConfigElement<out T> where T : Element
	{
		T Element { get; }
	}
}
