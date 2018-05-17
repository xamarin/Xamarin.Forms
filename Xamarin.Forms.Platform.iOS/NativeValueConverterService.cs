using System;
using Xamarin.Forms.Xaml.Internals;
using Xamarin.Forms.Internals;
#if __MOBILE__
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Xamarin.Forms.Platform.iOS.NativeValueConverterService))]
namespace Xamarin.Forms.Platform.iOS
#else
using UIView = AppKit.NSView;

[assembly: Xamarin.Forms.Dependency(typeof(Xamarin.Forms.Platform.MacOS.NativeValueConverterService))]

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	class NativeValueConverterService : INativeValueConverterService
	{
		public bool ConvertTo(object value, Type toType, out object nativeValue)
		{
			nativeValue = null;
			switch (value)
			{
				case UIView nativeView when toType.IsAssignableFrom(typeof(View)):
					nativeValue = nativeView.ToView();
					return true;

				case UIViewController nativeViewController when toType.IsAssignableFrom(typeof(Page)):
					nativeValue = nativeViewController.ToPage();
					return true;
			}
			return false;
		}
	}
}