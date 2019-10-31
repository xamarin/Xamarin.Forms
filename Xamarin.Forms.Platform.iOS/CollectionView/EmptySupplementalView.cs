using System;
using CoreGraphics;
using Foundation;

namespace Xamarin.Forms.Platform.iOS
{
	internal class EmptySupplementalView : ItemsViewCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.EmptySupplementalView");

		[Export("initWithFrame:")]
		public EmptySupplementalView(CGRect frame) : base(frame)
		{
		}

		public override void ConstrainTo(nfloat constant)
		{
		}

		public override void ConstrainTo(CGSize constraint)
		{ 
		}

		public override CGSize Measure()
		{
			return CGSize.Empty;
		}
	}
}