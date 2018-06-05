using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal abstract class DefaultCell : UICollectionViewCell
	{
		public UILabel Label { get; }

		[Export("initWithFrame:")]
		protected DefaultCell(CGRect frame) : base(frame)
		{
			ContentView.BackgroundColor = UIColor.Clear;

			Label = new UILabel(frame)
			{
				TextColor = UIColor.Black,
				Lines = 1,
				Font = UIFont.PreferredBody
			};
			
			ContentView.AddSubview(Label);

			ContentView.BackgroundColor = UIColor.Yellow;

			InitializeConstraints();
		}

		void InitializeConstraints()
		{
			Label.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentView.TranslatesAutoresizingMaskIntoConstraints = false;

			var views = new NSMutableDictionary
			{
				{ new NSString("label"), Label }, { new NSString("contentView"), ContentView }
			};

			// TODO hartez 2018/06/04 14:28:08 Replace these with NSConstraint.Create or Anchor constraints (save some parsing cycles)	
			var pinContentToTop = NSLayoutConstraint.FromVisualFormat("V:|[contentView]",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);

			var pinContentToLeading = NSLayoutConstraint.FromVisualFormat("H:|[contentView]",
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);

			AddConstraints(pinContentToLeading);
			AddConstraints(pinContentToTop);

			InitializeOrientationConstraints(views);
		}

		protected abstract void InitializeOrientationConstraints(NSDictionary views);
	}

	internal sealed class DefaultVerticalListCell : DefaultCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultVerticalListCell");

		[Export("initWithFrame:")]
		public DefaultVerticalListCell(CGRect frame) : base(frame)
		{
		}

		protected override void InitializeOrientationConstraints(NSDictionary views)
		{
			// TODO hartez 2018/06/04 14:28:08 Replace these with NSConstraint.Create or Anchor constraints (save some parsing cycles)	
			var sizeToLabelVertical = NSLayoutConstraint.FromVisualFormat("V:|-[label]-|",
				NSLayoutFormatOptions.SpacingEdgeToEdge, null, views);

			var pinLabelToLeading = NSLayoutConstraint.FromVisualFormat("H:|-[label]", 
				NSLayoutFormatOptions.DirectionLeadingToTrailing, null, views);

			ContentView.AddConstraints(pinLabelToLeading);
			ContentView.AddConstraints(sizeToLabelVertical);
		}
	}

	// TODO hartez 2018/06/04 17:23:25 Why does navigating back from the test page for this cause a crash?	
	internal sealed class DefaultHorizontalListCell : DefaultCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultHorizontalListCell");

		[Export("initWithFrame:")]
		public DefaultHorizontalListCell(CGRect frame) : base(frame)
		{
		}

		protected override void InitializeOrientationConstraints(NSDictionary views)
		{
			// TODO hartez 2018/06/04 14:28:08 Replace these with NSConstraint.Create or Anchor constraints (save some parsing cycles)	
			var sizeToLabelHorizontal = NSLayoutConstraint.FromVisualFormat("H:|-[label]-|",
				NSLayoutFormatOptions.SpacingEdgeToEdge, null, views);

			ContentView.AddConstraints(sizeToLabelHorizontal);

			Label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
		}
	}
}