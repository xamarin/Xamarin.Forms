using ElmSharp;
using Xamarin.Platform.Tizen;

namespace Xamarin.Platform
{
	public static class SliderExtensions
	{
		public static void UpdateMinimum(this Slider nativeSlider, ISlider slider)
		{
			nativeSlider.Minimum = slider.Minimum;
		}

		public static void UpdateMaximum(this Slider nativeSlider, ISlider slider)
		{
			nativeSlider.Maximum = slider.Maximum;
		}

		public static void UpdateValue(this Slider nativeSlider, ISlider slider)
		{
			nativeSlider.Value = slider.Value;
		}

		public static void UpdateMinimumTrackColor(this Slider nativeSlider, ISlider slider)
		{
			UpdateMinimumTrackColor(nativeSlider, slider, null);
		}

		public static void UpdateMinimumTrackColor(this Slider nativeSlider, ISlider slider, Color? defaultMinTrackColor)
		{
			if (slider.MinimumTrackColor.IsDefault)
			{
				if (defaultMinTrackColor != null)
					nativeSlider.SetBarColor(defaultMinTrackColor.Value);
			}
			else
				nativeSlider.SetBarColor(slider.MinimumTrackColor.ToNative());
		}

		public static void UpdateMaximumTrackColor(this Slider nativeSlider, ISlider slider)
		{
			UpdateMaximumTrackColor(nativeSlider, slider, null);
		}

		public static void UpdateMaximumTrackColor(this Slider nativeSlider, ISlider slider, Color? defaultMaxTrackColor)
		{
			if (slider.MaximumTrackColor.IsDefault)
			{
				if (defaultMaxTrackColor != null)
					nativeSlider.SetBackgroundColor(defaultMaxTrackColor.Value);
			}
			else
			{
				nativeSlider.SetBackgroundColor(slider.MaximumTrackColor.ToNative());
			}
		}

		public static void UpdateThumbColor(this Slider nativeSlider, ISlider slider)
		{
			UpdateThumbColor(nativeSlider, slider, null);
		}

		public static void UpdateThumbColor(this Slider nativeSlider, ISlider slider, Color? defaultThumbColor)
		{
			if (slider.ThumbColor.IsDefault)
			{
				if (defaultThumbColor != null)
					nativeSlider.SetHandlerColor(defaultThumbColor.Value);
			}
			else
			{
				nativeSlider.SetHandlerColor(slider.ThumbColor.ToNative());
			}
		}
	}
}