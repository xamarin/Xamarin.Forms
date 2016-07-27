using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

#if __UNIFIED__
using Foundation;
using UIKit;

#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;

#endif

#if !__UNIFIED__
// Save ourselves a ton of ugly ifdefs below
using CGSize = System.Drawing.SizeF;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	public class NativeViewWrapper : View
	{
		public NativeViewWrapper(UIView nativeView, GetDesiredSizeDelegate getDesiredSizeDelegate = null, SizeThatFitsDelegate sizeThatFitsDelegate = null, LayoutSubviewsDelegate layoutSubViews = null)
		{
			GetDesiredSizeDelegate = getDesiredSizeDelegate;
			SizeThatFitsDelegate = sizeThatFitsDelegate;
			LayoutSubViews = layoutSubViews;
			NativeView = nativeView;
		}

		public GetDesiredSizeDelegate GetDesiredSizeDelegate { get; }

		public LayoutSubviewsDelegate LayoutSubViews { get; set; }

		public UIView NativeView { get; }

		public SizeThatFitsDelegate SizeThatFitsDelegate { get; set; }

		protected override void OnBindingContextChanged()
		{
			FormsNativeBindingExtensions.SetNativeBindingContext(NativeView?.GetViewAndDescedants(), BindingContext);
			base.OnBindingContextChanged();
		}
	}
}