using UIKit;
using Xamarin.Forms.Platform.iOS.Controls.Snackbar.Extensions;

namespace Xamarin.Forms.Platform.iOS.Controls.Snackbar.SnackbarViews
{
	class MessageSnackbarView : BaseSnackbarView
	{
		public MessageSnackbarView(SnackBar snackbar) : base(snackbar)
		{
		}

		public UILabel MessageLabel { get; set; }

		protected override void ConstrainChildren()
		{
			base.ConstrainChildren();

			MessageLabel.SafeLeadingAnchor().ConstraintEqualTo(this.SafeLeadingAnchor(), Snackbar.Layout.PaddingLeading)
				.Active = true;
			MessageLabel.SafeTrailingAnchor()
				.ConstraintEqualTo(this.SafeTrailingAnchor(), -Snackbar.Layout.PaddingTrailing).Active = true;
			MessageLabel.SafeBottomAnchor().ConstraintEqualTo(this.SafeBottomAnchor(), -Snackbar.Layout.PaddingBottom)
				.Active = true;
			MessageLabel.SafeTopAnchor().ConstraintEqualTo(this.SafeTopAnchor(), Snackbar.Layout.PaddingTop).Active =
				true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			MessageLabel = new UILabel
			{
				Text = Snackbar.Message,
				Lines = 0,
				AdjustsFontSizeToFitWidth = true,
				MinimumFontSize = 10f,
				TextAlignment = Snackbar.Appearance.MessageTextAlignment,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			AddSubview(MessageLabel);
		}
	}
}