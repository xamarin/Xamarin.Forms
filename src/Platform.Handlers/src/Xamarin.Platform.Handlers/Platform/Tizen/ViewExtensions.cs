using ElmSharp;

namespace Xamarin.Platform
{
	public static class ViewExtensions
	{
		public static void UpdateIsEnabled(this EvasObject nativeView, IView view)
		{
			if (!(nativeView is Widget widget))
				return;

			widget.IsEnabled = view.IsEnabled;
		}

		public static void UpdateBackgroundColor(this EvasObject nativeView, IView view)
		{
			if (!(nativeView is Widget widget))
				return;

			widget.BackgroundColor = view.BackgroundColor.ToNative();
		}
	}
}
