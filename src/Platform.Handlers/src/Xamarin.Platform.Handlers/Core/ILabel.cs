using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface ILabel : IView
	{
		string Text { get; }
		Color TextColor { get; }
	}
}