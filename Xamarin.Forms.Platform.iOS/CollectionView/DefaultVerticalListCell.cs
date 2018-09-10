using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class DefaultVerticalListCell : DefaultCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultVerticalListCell");

		[Export("initWithFrame:")]
		public DefaultVerticalListCell(CGRect frame) : base(frame)
		{
			Width = Label.WidthAnchor.ConstraintEqualTo(Frame.Width);
			Width.Active = true;
		}

		NSLayoutConstraint Width { get; }

		public void SetConstrainedDimension(nfloat constant)
		{
			Width.Constant = constant;
		}
	}
}