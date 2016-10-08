using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class ModalWrapper : UIViewController
	{
		IVisualElementRenderer _modal;
		internal bool Appeared { get; private set; }

		bool _disposed;

		internal ModalWrapper(IVisualElementRenderer modal)
		{
			_modal = modal;

			View.BackgroundColor = UIColor.White;
			View.AddSubview(modal.ViewController.View);
			AddChildViewController(modal.ViewController);

			modal.ViewController.DidMoveToParentViewController(this);
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			if ((ChildViewControllers != null) && (ChildViewControllers.Length > 0))
			{
				return ChildViewControllers[0].GetSupportedInterfaceOrientations();
			}

			return base.GetSupportedInterfaceOrientations();
		}

		public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation()
		{
			if ((ChildViewControllers != null) && (ChildViewControllers.Length > 0))
			{
				return ChildViewControllers[0].PreferredInterfaceOrientationForPresentation();
			}
			return base.PreferredInterfaceOrientationForPresentation();
		}

		public override bool ShouldAutorotate()
		{
			if ((ChildViewControllers != null) && (ChildViewControllers.Length > 0))
			{
				return ChildViewControllers[0].ShouldAutorotate();
			}
			return base.ShouldAutorotate();
		}

		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			if ((ChildViewControllers != null) && (ChildViewControllers.Length > 0))
			{
				return ChildViewControllers[0].ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
			}
			return base.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
		}

		public override bool ShouldAutomaticallyForwardRotationMethods => true;

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			_modal?.SetElementSize(new Size(View.Bounds.Width, View.Bounds.Height));
		}

		public override void ViewWillAppear(bool animated)
		{
			View.BackgroundColor = UIColor.White;
			base.ViewWillAppear(animated);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (Appeared || _disposed)
				return;

			Appeared = true;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			if (!Appeared || _disposed)
				return;

			Appeared = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				Appeared = false;
				_modal = null;
			}
			base.Dispose(disposing);
		}
	}
}