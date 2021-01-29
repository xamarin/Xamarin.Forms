using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IBox : IView
	{
		Color Color { get; }

		CornerRadius CornerRadius { get; }
	}
}