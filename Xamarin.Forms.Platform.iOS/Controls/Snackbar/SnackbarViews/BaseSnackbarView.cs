using UIKit;
using Xamarin.Forms.Platform.iOS.Controls.Snackbar.Extensions;

namespace Xamarin.Forms.Platform.iOS.Controls.Snackbar.SnackbarViews
{
	class BaseSnackbarView : UIView
	{
		public BaseSnackbarView(SnackBar snackbar)
		{
			Snackbar = snackbar;
		}

		public NSLayoutConstraint BottomConstraint { get; protected set; }

		public NSLayoutConstraint CenterXConstraint { get; protected set; }

		public NSLayoutConstraint CenterYConstraint { get; protected set; }

		public NSLayoutConstraint LeadingConstraint { get; protected set; }

		public virtual UIView ParentView
		{
			get => Snackbar.ParentController != null
				? Snackbar.ParentController.View
				: UIApplication.SharedApplication.KeyWindow;
		}

		public NSLayoutConstraint TopConstraint { get; protected set; }

		public NSLayoutConstraint TrailingConstraint { get; protected set; }

		protected SnackBar Snackbar { get; }

		public virtual void Dismiss()
		{
			RemoveFromSuperview();
		}

		public virtual void Setup()
		{
			Initialize();
			ConstrainInParent();
			ConstrainChildren();
		}

		protected virtual void ConstrainChildren()
		{
		}

		protected virtual void ConstrainInParent()
		{
			BottomConstraint = this.SafeBottomAnchor()
				.ConstraintEqualTo(GetBottomAnchor(), -Snackbar.Layout.MarginBottom);
			BottomConstraint.Active = true;

			TopConstraint = this.SafeTopAnchor()
				.ConstraintGreaterThanOrEqualTo(GetTopAnchor(), Snackbar.Layout.MarginTop);
			TopConstraint.Active = true;

			LeadingConstraint = this.SafeLeadingAnchor()
				.ConstraintGreaterThanOrEqualTo(ParentView.SafeLeadingAnchor(), Snackbar.Layout.MarginLeading);
			TrailingConstraint = this.SafeTrailingAnchor()
				.ConstraintLessThanOrEqualTo(ParentView.SafeTrailingAnchor(), -Snackbar.Layout.MarginTrailing);
			CenterXConstraint = this.SafeCenterXAnchor().ConstraintEqualTo(ParentView.SafeCenterXAnchor());

			NSLayoutConstraint.ActivateConstraints(
				new[] { LeadingConstraint, TrailingConstraint, CenterXConstraint });
		}

		protected virtual NSLayoutYAxisAnchor GetBottomAnchor()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0) || Snackbar.ParentController == null)
			{
				return ParentView.SafeBottomAnchor();
			}

			return Snackbar.ParentController.BottomLayoutGuide.GetTopAnchor();
		}

		protected virtual NSLayoutYAxisAnchor GetCenterYAnchor()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0) || Snackbar.ParentController == null)
			{
				return ParentView.SafeCenterYAnchor();
			}

			return Snackbar.ParentController.View.CenterYAnchor;
		}

		protected virtual NSLayoutYAxisAnchor GetTopAnchor()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0) || Snackbar.ParentController == null)
			{
				return ParentView.SafeTopAnchor();
			}

			return Snackbar.ParentController.TopLayoutGuide.GetBottomAnchor();
		}

		protected virtual void Initialize()
		{
			BackgroundColor = Snackbar.Appearance.Color;
			Layer.CornerRadius = Snackbar.Appearance.CornerRadius;
			TranslatesAutoresizingMaskIntoConstraints = false;
		}
	}
}