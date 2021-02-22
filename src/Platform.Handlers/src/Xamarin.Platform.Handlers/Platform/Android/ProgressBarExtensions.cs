using Android.Content.Res;
using Android.OS;
using Xamarin.Forms;
using AProgressBar = Android.Widget.ProgressBar;

namespace Xamarin.Platform
{
	public static class ProgressBar
	{
		public const int Maximum = 10000;
	}

	public static class ProgressBarExtensions
	{
		public static void UpdateProgress(this AProgressBar nativeProgressBar, IProgress progress)
		{
			nativeProgressBar.Progress = (int)(progress.Progress * ProgressBar.Maximum);
		}

		public static void UpdateProgressColor(this AProgressBar nativeProgressBar, IProgress progress)
		{
			Color color = progress.ProgressColor;

			if (color.IsDefault)
			{
				(nativeProgressBar.Indeterminate ? nativeProgressBar.IndeterminateDrawable :
					nativeProgressBar.ProgressDrawable)?.ClearColorFilter();
			}
			else
			{
				if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
				{
					(nativeProgressBar.Indeterminate ? nativeProgressBar.IndeterminateDrawable :
						nativeProgressBar.ProgressDrawable)?.SetColorFilter(color, FilterMode.SrcIn);
				}
				else
				{
					var tintList = ColorStateList.ValueOf(color.ToNative());
					if (nativeProgressBar.Indeterminate)
						nativeProgressBar.IndeterminateTintList = tintList;
					else
						nativeProgressBar.ProgressTintList = tintList;
				}
			}
		}
	}
}