using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class DefaultCell : UICollectionViewCell
	{
		public UILabel Label { get; }

		public abstract void UpdateConstrainedDimension(nfloat constant);

		[Export("initWithFrame:")]
		protected DefaultCell(CGRect frame) : base(frame)
		{
			Label = new UILabel(frame)
			{
				TextColor = UIColor.Black,
				Lines = 1,
				Font = UIFont.PreferredBody,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			ContentView.BackgroundColor = UIColor.Clear;
			ContentView.AddSubview(Label);
			ContentView.TranslatesAutoresizingMaskIntoConstraints = false;

			ContentView.TopAnchor.ConstraintEqualTo(Label.TopAnchor).Active = true;
			ContentView.BottomAnchor.ConstraintEqualTo(Label.BottomAnchor).Active = true;
			ContentView.LeadingAnchor.ConstraintEqualTo(Label.LeadingAnchor).Active = true;
			ContentView.TrailingAnchor.ConstraintEqualTo(Label.TrailingAnchor).Active = true;
		}
	}

	internal sealed class DefaultVerticalListCell : DefaultCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultVerticalListCell");

		[Export("initWithFrame:")]
		public DefaultVerticalListCell(CGRect frame) : base(frame)
		{
			Width = Label.WidthAnchor.ConstraintEqualTo(Frame.Width);
			Width.Active = true;
		}

		NSLayoutConstraint Width { get; }

		public override void UpdateConstrainedDimension(nfloat constant)
		{
			Width.Constant = constant;
		}
	}

	internal sealed class DefaultHorizontalListCell : DefaultCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultHorizontalListCell");

		[Export("initWithFrame:")]
		public DefaultHorizontalListCell(CGRect frame) : base(frame)
		{
			Height = Label.HeightAnchor.ConstraintEqualTo(Frame.Height);
			Height.Active = true;
		}

		NSLayoutConstraint Height { get; }

		public override void UpdateConstrainedDimension(nfloat constant)
		{
			Height.Constant = constant;
		}
	}
}