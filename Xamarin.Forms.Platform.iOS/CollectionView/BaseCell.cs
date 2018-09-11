using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class BaseCell : UICollectionViewCell
	{
		[Export("initWithFrame:")]
		protected BaseCell(CGRect frame) : base(frame)
		{
			ContentView.BackgroundColor = UIColor.Clear;
		}

		protected void InitializeContentConstraints(UIView nativeView)
		{
			ContentView.AddSubview(nativeView);
			ContentView.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.TopAnchor.ConstraintEqualTo(nativeView.TopAnchor).Active = true;
			ContentView.BottomAnchor.ConstraintEqualTo(nativeView.BottomAnchor).Active = true;
			ContentView.LeadingAnchor.ConstraintEqualTo(nativeView.LeadingAnchor).Active = true;
			ContentView.TrailingAnchor.ConstraintEqualTo(nativeView.TrailingAnchor).Active = true;
		}
	}
}