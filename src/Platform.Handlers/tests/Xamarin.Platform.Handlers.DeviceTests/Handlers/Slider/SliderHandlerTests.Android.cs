using Xamarin.Forms;
using System.Threading.Tasks;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SliderHandlerTests
	{
		double GetNativeProgress(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.Progress;

		double GetNativeMaximum(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.Max;


		Task ValidateNativeThumbColor(ISlider slider, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				CreateHandler(slider).TypedNativeView.AssertContainsColor(color);
			});
		}
	}
}
