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

		public void Constrain(nfloat constant)
		{
			Height.Constant = constant;
		}

		public void Constrain(CGSize constraint)
		{
			Height.Constant = constraint.Height;
			// TODO hartez 2018/09/12 10:38:51 We need to add an optional width constraint here, in case they've set up a text-only list with UniformSize	
		}
	}
}