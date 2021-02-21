using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SliderHandlerTests
	{
		[Fact(DisplayName = "[SliderHandler] [iOS] Value Initializes Correctly")]
		public async Task ValueInitializesCorrectly()
		{
			var slider = new SliderStub()
			{
				Maximum = 1,
				Minimum = 0,
				Value = 0.5
			};

			await ValidatePropertyInitValue(slider, () => slider.Value, GetNativeProgress, slider.Value);
		}

		[Fact(DisplayName = "[SliderHandler] [iOS] Maximum Initializes Correctly")]
		public async Task MaximumInitializesCorrectly()
		{
			var slider = new SliderStub()
			{
				Maximum = 1
			};

			await ValidatePropertyInitValue(slider, () => slider.Maximum, GetNativeMaximum, slider.Maximum);
		}

		UISlider GetNativeSlider(SliderHandler sliderHandler) =>
			(UISlider)sliderHandler.View;

		double GetNativeProgress(SliderHandler sliderHandler) =>
			GetNativeSlider(sliderHandler).Value;

		double GetNativeMinimum(SliderHandler sliderHandler) =>
			GetNativeSlider(sliderHandler).MinValue;

		double GetNativeMaximum(SliderHandler sliderHandler) =>
			GetNativeSlider(sliderHandler).MaxValue;

		async Task ValidateNativeThumbColor(ISlider slider, Color color)
		{
			var expected = await GetValueAsync(slider, handler => GetNativeSlider(handler).ThumbTintColor.ToColor());
			Assert.Equal(expected, color);
		}
	}
}
