﻿using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Widget;
using Xunit;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SliderHandlerTests
	{
		SeekBar GetNativeSlider(SliderHandler sliderHandler) =>
			(SeekBar)sliderHandler.View;

		double GetNativeProgress(SliderHandler sliderHandler) =>
			GetNativeSlider(sliderHandler).Progress;

		double GetNativeMinimum(SliderHandler sliderHandler)
		{
			if (NativeVersion.Supports(NativeApis.SeekBarSetMin))
			{
				return GetNativeSlider(sliderHandler).Min;
			}

			return 0;
		}

		double GetNativeMaximum(SliderHandler sliderHandler) =>
			GetNativeSlider(sliderHandler).Max;

		Task ValidateNativeThumbColor(ISlider slider, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeSlider(CreateHandler(slider)).AssertContainsColor(color);
			});
		}

		[Fact(DisplayName = "[SliderHandler] Maximum Value Initializes Correctly")]
		public async Task MaximumInitializesCorrectly()
		{
			var xplatMaximum = 1;
			var slider = new SliderStub()
			{
				Maximum = xplatMaximum
			};

			double expectedValue = SliderExtensions.NativeMaxValue;

			var values = await GetValueAsync(slider, (handler) =>
			{
				return new
				{
					ViewValue = slider.Maximum,
					NativeViewValue = GetNativeMaximum(handler)
				};
			});

			Assert.Equal(xplatMaximum, values.ViewValue);
			Assert.Equal(expectedValue, values.NativeViewValue);
		}

		[Fact(DisplayName = "[SliderHandler] Value Initializes Correctly")]
		public async Task ValueInitializesCorrectly()
		{
			var xplatValue = 0.5;
			var slider = new SliderStub()
			{
				Maximum = 1,
				Minimum = 0,
				Value = xplatValue
			};

			double expectedValue = SliderExtensions.NativeMaxValue / 2;

			var values = await GetValueAsync(slider, (handler) =>
			{
				return new
				{
					ViewValue = slider.Value,
					NativeViewValue = GetNativeProgress(handler)
				};
			});

			Assert.Equal(xplatValue, values.ViewValue);
			Assert.Equal(expectedValue, values.NativeViewValue);
		}
	}
}
