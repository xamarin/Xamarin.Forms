using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class DefaultHorizontalListCell : DefaultCell, IConstrainedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultHorizontalListCell");

		[Export("initWithFrame:")]
		public DefaultHorizontalListCell(CGRect frame) : base(frame)
		{
			Height = Label.HeightAnchor.ConstraintEqualTo(Frame.Height);
			Height.Active = true;
		}

		NSLayoutConstraint Height { get; }

		public void SetConstrainedDimension(nfloat constant)
		{
			Height.Constant = constant;
		}
	}
}