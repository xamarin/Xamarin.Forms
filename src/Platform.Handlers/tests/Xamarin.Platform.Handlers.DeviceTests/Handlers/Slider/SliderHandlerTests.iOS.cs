
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SliderHandlerTests
	{
		double GetNativeProgress(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.Value;

		double GetNativeMaximum(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.MaxValue;
	}
}
