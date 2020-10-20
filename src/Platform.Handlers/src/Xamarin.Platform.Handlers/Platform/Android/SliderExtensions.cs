﻿using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class SliderExtensions
	{
		public static void UpdateMinimum(this SeekBar seekBar, ISlider slider)
		{
			if (NativeVersion.Supports(NativeApis.SeekBarSetMin))
			{
				seekBar.Min = (int)slider.Minimum;
			}
		}

		public static void UpdateMaximum(this SeekBar seekBar, ISlider slider) =>
			seekBar.Max = (int)slider.Maximum;

		public static void UpdateValue(this SeekBar seekBar, ISlider slider)
		{
			var min = slider.Minimum;
			var max = slider.Maximum;
			var value = slider.Value;

			seekBar.Progress = (int)((value - min) / (max - min) * 1000.0);
		}

		public static void UpdateMinimumTrackColor(this SeekBar seekBar, ISlider slider) =>
			UpdateMinimumTrackColor(seekBar, slider, null, null);

		public static void UpdateMinimumTrackColor(this SeekBar seekBar, ISlider slider, ColorStateList? defaultProgressTintList, PorterDuff.Mode? defaultProgressTintMode)
		{
			if (slider.MinimumTrackColor == Forms.Color.Default)
			{
				if (defaultProgressTintList != null)
					seekBar.ProgressTintList = defaultProgressTintList;

				if (defaultProgressTintMode != null)
					seekBar.ProgressTintMode = defaultProgressTintMode;
			}
			else
			{
				seekBar.ProgressTintList = ColorStateList.ValueOf(slider.MinimumTrackColor.ToNative());
				seekBar.ProgressTintMode = PorterDuff.Mode.SrcIn;
			}
		}

		public static void UpdateMaximumTrackColor(this SeekBar seekBar, ISlider slider) =>
			UpdateMaximumTrackColor(seekBar, slider, null, null);

		public static void UpdateMaximumTrackColor(this SeekBar seekBar, ISlider slider, ColorStateList? defaultProgressBackgroundTintList, PorterDuff.Mode? defaultProgressBackgroundTintMode)
		{
			if (slider.MaximumTrackColor == Forms.Color.Default)
			{
				if (defaultProgressBackgroundTintList != null)
					seekBar.ProgressBackgroundTintList = defaultProgressBackgroundTintList;

				if (defaultProgressBackgroundTintMode != null)
					seekBar.ProgressBackgroundTintMode = defaultProgressBackgroundTintMode;
			}
			else
			{
				seekBar.ProgressBackgroundTintList = ColorStateList.ValueOf(slider.MaximumTrackColor.ToNative());
				seekBar.ProgressBackgroundTintMode = PorterDuff.Mode.SrcIn;
			}
		}

		public static void UpdateThumbColor(this SeekBar seekBar, ISlider slider) =>
			UpdateThumbColor(seekBar, slider);

		public static void UpdateThumbColor(this SeekBar seekBar, ISlider slider, ColorFilter? defaultThumbColorFilter) =>
			seekBar.Thumb?.SetColorFilter(slider.ThumbColor, FilterMode.SrcIn, defaultThumbColorFilter);
	}
}