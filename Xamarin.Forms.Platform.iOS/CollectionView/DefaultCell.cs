using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal sealed class DefaultCell : UICollectionViewCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultCell");

		public UILabel Label { get; }

		[Export("initWithFrame:")]
		public DefaultCell(CGRect frame) : base(frame)
		{
			ContentView.BackgroundColor = UIColor.Clear;

			Label = new UILabel(Bounds)
			{
				TextColor = UIColor.Black
			};
			
			ContentView.AddSubview(Label);
		}
	}
}