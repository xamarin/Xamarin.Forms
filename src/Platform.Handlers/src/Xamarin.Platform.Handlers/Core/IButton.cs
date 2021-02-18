using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IButton : IView
	{
		string Text { get; }
		Color TextColor { get; }

		void Pressed();
		void Released();
		void Clicked();
	}
}