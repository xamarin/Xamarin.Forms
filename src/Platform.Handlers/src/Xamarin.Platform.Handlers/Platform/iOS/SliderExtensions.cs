﻿using UIKit;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class SliderExtensions
	{
		public static void UpdateMinimum(this UISlider uiSlider, ISlider slider)
		{
			uiSlider.MaxValue = (float)slider.Maximum;
		}

		public static void UpdateMaximum(this UISlider uiSlider, ISlider slider)
		{
			uiSlider.MinValue = (float)slider.Minimum;
		}

		public static void UpdateValue(this UISlider uiSlider, ISlider slider)
		{
			if ((float)slider.Value != uiSlider.Value)
				uiSlider.Value = (float)slider.Value;
		}

		public static void UpdateMinimumTrackColor(this UISlider uiSlider, ISlider slider)
		{
			UpdateMinimumTrackColor(uiSlider, slider, null);
		}

		public static void UpdateMinimumTrackColor(this UISlider uiSlider, ISlider slider, UIColor? defaultMinTrackColor)
		{
			if (slider.MinimumTrackColor == Color.Default)
			{
				if (defaultMinTrackColor != null)
					uiSlider.MinimumTrackTintColor = defaultMinTrackColor;
			}
			else
				uiSlider.MinimumTrackTintColor = slider.MinimumTrackColor.ToNative();
		}

		public static void UpdateMaximumTrackColor(this UISlider uiSlider, ISlider slider)
		{
			UpdateMaximumTrackColor(uiSlider, slider, null);
		}

		public static void UpdateMaximumTrackColor(this UISlider uiSlider, ISlider slider, UIColor? defaultMaxTrackColor)
		{
			if (slider.MaximumTrackColor == Color.Default)
				uiSlider.MaximumTrackTintColor = defaultMaxTrackColor;
			else
				uiSlider.MaximumTrackTintColor = slider.MaximumTrackColor.ToNative();
		}

		public static void UpdateThumbColor(this UISlider uiSlider, ISlider slider)
		{
			UpdateThumbColor(uiSlider, slider, null);
		}

		public static void UpdateThumbColor(this UISlider uiSlider, ISlider slider, UIColor? defaultThumbColor)
		{
			if (slider.ThumbColor == Color.Default)
				uiSlider.ThumbTintColor = defaultThumbColor;
			else
				uiSlider.ThumbTintColor = slider.ThumbColor.ToNative();
		}
	}
}