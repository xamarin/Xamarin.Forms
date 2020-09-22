using System;

#if __IOS__
using NativeColor = UIKit.UIColor;
using NativeControl = UIKit.UIControl;
using NativeView = UIKit.UIView;
#else
using NativeColor = CoreGraphics.CGColor;
using NativeControl = AppKit.NSControl;
using NativeView = AppKit.NSView;
#endif

namespace Xamarin.Platform.Handlers
{
	public static class ViewExtensions
	{
		public static void UpdateIsEnabled(this NativeView nativeView, IView view)
		{
			if (!(nativeView is NativeControl uiControl))
				return;

			uiControl.Enabled = view.IsEnabled;
		}

		public static void UpdateBackgroundColor(this NativeView nativeView, IView view)
		{
			var color = view.BackgroundColor;

			if (color != null && !color.IsDefault)
				nativeView.SetBackgroundColor(color.ToNative());
		}
	}
}