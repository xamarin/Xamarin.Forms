using System.Drawing;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SliderHandlerTests
	{
		double GetNativeProgress(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.Progress;

		double GetNativeMaximum(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.Max;
	}
}
