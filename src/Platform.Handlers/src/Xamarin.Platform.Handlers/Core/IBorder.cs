using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IBorder : IView
	{
		int CornerRadius { get; }
		Color BorderColor { get; }
		double BorderWidth { get; }
	}
}