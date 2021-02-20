using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IBorder
	{
		int CornerRadius { get; }
		Color BorderColor { get; }
		double BorderWidth { get; }
	}
}