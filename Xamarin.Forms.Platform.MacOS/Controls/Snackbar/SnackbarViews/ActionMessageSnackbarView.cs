using System;
using AppKit;

namespace Xamarin.Forms.Platform.macOS.Controls.Snackbar.SnackbarViews
{
	class ActionMessageSnackbarView : MessageSnackbarView
	{
		readonly SnackBar _snackbar;

		public ActionMessageSnackbarView(SnackBar snackbar) : base(snackbar)
		{
			_snackbar = snackbar;
		}

		public NSButton ActionButton { get; set; }

		protected virtual nfloat ActionButtonMaxWidth
		{
			// Gets the maximum width of the action button. Possible values 0 to 1.
			get => 1f;
		}

		public override void RemoveFromSuperview()
		{
			base.RemoveFromSuperview();

			if (ActionButton != null)
			{
				ActionButton.Activated -= DismissButton_TouchUpInside;
			}
		}

		protected override void ConstrainChildren()
		{
			MessageLabel.TrailingAnchor.ConstraintLessThanOrEqualToAnchor(ActionButton.LeadingAnchor, -Snackbar.Layout.Spacing).Active = true;
			MessageLabel.LeadingAnchor.ConstraintEqualToAnchor(this.LeadingAnchor, Snackbar.Layout.PaddingLeading).Active = true;
			MessageLabel.BottomAnchor.ConstraintEqualToAnchor(this.BottomAnchor, -Snackbar.Layout.PaddingBottom).Active = true;
			MessageLabel.TopAnchor.ConstraintEqualToAnchor(this.TopAnchor, Snackbar.Layout.PaddingTop).Active =	true;

			ActionButton.TrailingAnchor.ConstraintEqualToAnchor(TrailingAnchor, -Snackbar.Layout.PaddingTrailing).Active = true;
			ActionButton.CenterYAnchor.ConstraintEqualToAnchor(CenterYAnchor).Active = true;
			// The following constraint makes sure that button is not wider than specified amount of available width
			ActionButton.WidthAnchor.ConstraintLessThanOrEqualToAnchor(WidthAnchor, ActionButtonMaxWidth, 0f).Active = true;

			ActionButton.SetContentCompressionResistancePriority(
				MessageLabel.GetContentCompressionResistancePriority(NSLayoutConstraintOrientation.Horizontal) + 1,
				NSLayoutConstraintOrientation.Horizontal);
		}

		protected override void Initialize()
		{
			base.Initialize();

			ActionButton = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false };
			ActionButton.Title = Snackbar.ActionButtonText;
			ActionButton.LineBreakMode = Snackbar.Appearance.DismissButtonLineBreakMode;
			ActionButton.Activated += DismissButton_TouchUpInside;
			AddSubview(ActionButton);
		}

		async void DismissButton_TouchUpInside(object sender, EventArgs e)
		{
			await _snackbar.Action();
			Dismiss();
		}
	}
}