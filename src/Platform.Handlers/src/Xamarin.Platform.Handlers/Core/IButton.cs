using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IButton : IView, IText
	{
		string Text { get; }
		Color TextColor { get; }

		void Pressed();
		void Released();
		void Clicked();
	}
}