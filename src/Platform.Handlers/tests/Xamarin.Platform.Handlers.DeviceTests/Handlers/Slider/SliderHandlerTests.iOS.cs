
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SliderHandlerTests
	{
		double GetNativeProgress(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.Value;

		double GetNativeMaximum(SliderHandler sliderHandler) =>
			sliderHandler.TypedNativeView.MaxValue;


		async Task ValidateNativeThumbColor(ISlider slider, Color color)
		{
			var expected = await GetValueAsync(slider, handler => handler.TypedNativeView.ThumbTintColor.ToColor());
			Assert.Equal(expected, color);
		}
	}
}
