using System;

#if __UNIFIED__
using Foundation;

#else
using MonoTouch.Foundation;

#endif

namespace Xamarin.Forms.Platform.iOS
{
	class NativeViewPropertyListener : NSObject
	{
		readonly BindableProxy _nativeBindableController;

		public NativeViewPropertyListener(BindableProxy nativeViewBindableController)
		{
			_nativeBindableController = nativeViewBindableController;
		}

		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			_nativeBindableController.OnTargetPropertyChanged();
		}
	}
}

