using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal abstract class DefaultCell : UICollectionViewCell
	{
		public UILabel Label { get; }

		public abstract void UpdateConstrainedDimension(nfloat constant);

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

			InitializeConstraints();
		}

		void InitializeConstraints()
		{
			Label.TranslatesAutoresizingMaskIntoConstraints = false;
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
			Width = Label.WidthAnchor.ConstraintEqualTo(Frame.Width - 2);
			Width.Active = true;
		}

		NSLayoutConstraint Width { get; }

		public override void UpdateConstrainedDimension(nfloat constant)
		{
			// TODO hartez 2018/06/10 14:54:51 Can we drop the "- 2" now? Or at least make it a constant. (with an explanation)
			Width.Constant = constant - 2;
		}
	}

	internal sealed class DefaultHorizontalListCell : DefaultCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.DefaultHorizontalListCell");

		[Export("initWithFrame:")]
		public DefaultHorizontalListCell(CGRect frame) : base(frame)
		{
			Height = Label.HeightAnchor.ConstraintEqualTo(Frame.Height - 2);
			Height.Active = true;
		}

		NSLayoutConstraint Height { get; }

		public override void UpdateConstrainedDimension(nfloat constant)
		{
			Height.Constant = constant - 2;
		}
	}
}