using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class ActivityIndicatorExtensions
	{
		public static void UpdateIsRunning(this ProgressBar progressBar, IActivityIndicator activityIndicator) =>
			progressBar.Visibility = activityIndicator.IsRunning ? ViewStates.Visible : ViewStates.Invisible;

		public static void UpdateColor(this ProgressBar progressBar, IActivityIndicator activityIndicator)
		{
			Color color = activityIndicator.Color;

			if (!color.IsDefault)
				progressBar.IndeterminateDrawable?.SetColorFilter(color.ToNative(), FilterMode.SrcIn);
			else
				progressBar.IndeterminateDrawable?.ClearColorFilter();
		}
	}
}