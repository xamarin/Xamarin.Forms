using AppKit;

namespace Xamarin.Forms.Platform.macOS.Controls.Snackbar.SnackbarViews
{
	class MessageSnackbarView : BaseSnackbarView
	{
		public MessageSnackbarView(SnackBar snackbar) : base(snackbar)
		{
		}

		public NSTextField MessageLabel { get; set; }

		protected override void ConstrainChildren()
		{
			base.ConstrainChildren();

			MessageLabel.LeadingAnchor.ConstraintEqualToAnchor(LeadingAnchor, Snackbar.Layout.PaddingLeading).Active = true;
			MessageLabel.TrailingAnchor.ConstraintEqualToAnchor(TrailingAnchor, -Snackbar.Layout.PaddingTrailing).Active = true;
			MessageLabel.BottomAnchor.ConstraintEqualToAnchor(BottomAnchor, -Snackbar.Layout.PaddingBottom).Active = true;
			MessageLabel.TopAnchor.ConstraintEqualToAnchor(TopAnchor, Snackbar.Layout.PaddingTop).Active = true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			MessageLabel = new NSTextField
			{
				StringValue = Snackbar.Message,
				Selectable = false,
				BackgroundColor = Snackbar.Appearance.Color,
				Alignment = Snackbar.Appearance.MessageTextAlignment,
				LineBreakMode = Snackbar.Appearance.DismissButtonLineBreakMode,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			AddSubview(MessageLabel);
		}
	}
}