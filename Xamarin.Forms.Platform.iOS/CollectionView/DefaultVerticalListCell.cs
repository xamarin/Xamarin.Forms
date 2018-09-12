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

		public void Constrain(nfloat constant)
		{
			Width.Constant = constant;
		}

		public void Constrain(CGSize constraint)
		{
			Width.Constant = constraint.Width;
			// TODO hartez 2018/09/12 10:38:51 We need to add an optional Height constraint here, in case they've set up a text-only list with UniformSize	
			// This might turn into a single class with a PrimaryConstraintDirection property or something
		}
	}
}