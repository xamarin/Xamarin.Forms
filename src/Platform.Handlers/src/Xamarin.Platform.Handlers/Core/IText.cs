using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IText : IView
	{
		string Text { get; }

		Color TextColor { get; }
	}
}