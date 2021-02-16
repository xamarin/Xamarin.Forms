using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IButton : IText, IBorder, IPadding
	{
		LineBreakMode LineBreakMode { get; }
		ButtonContentLayout ContentLayout { get; }

		void Pressed();
		void Released();
		void Clicked();
	}
}