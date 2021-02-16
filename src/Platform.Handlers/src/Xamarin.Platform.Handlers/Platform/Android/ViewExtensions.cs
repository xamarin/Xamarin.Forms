using Android.Graphics.Drawables;
using AView = Android.Views.View;

namespace Xamarin.Platform
{
	public static class ViewExtensions
	{
		public static void UpdateIsEnabled(this AView nativeView, IView view)
		{
			if (nativeView != null)
				nativeView.Enabled = view.IsEnabled;
		}

		public static void UpdateBackgroundColor(this AView nativeView, IView view)
		{
			var backgroundColor = view.BackgroundColor;
			if (!backgroundColor.IsDefault)
				nativeView?.SetBackgroundColor(backgroundColor.ToNative());
		}

		public static void SetBackground(this AView view, Drawable drawable)
		{
			view.Background = drawable;
		}
	}
}