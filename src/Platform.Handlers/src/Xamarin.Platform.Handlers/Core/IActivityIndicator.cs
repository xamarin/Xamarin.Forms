using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IActivityIndicator : IView
	{
		bool IsRunning { get; }
		Color Color { get; }
	}
}