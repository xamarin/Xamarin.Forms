using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface ISwitch : IView
	{
		bool IsToggled { get; set; }
		Color OnColor { get; }
		Color ThumbColor { get; }
	}
}