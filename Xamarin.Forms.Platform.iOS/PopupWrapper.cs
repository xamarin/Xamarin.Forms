using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class PopupWrapper<T> : UIViewController
	{
		readonly IVisualElementRenderer _renderer;

		public static UIViewController Wrap(Popup<T> popup)
		{
			var wrapper = new PopupWrapper<T>(popup);

			if (Device.Idiom != TargetIdiom.Phone || !popup.IsLightDismissEnabled)
			{
				return wrapper;
			}

			// There's no Light Dismiss on iPhone; we'll need to add a 'Cancel' button
			// TODO hartez 2017/01/19 10:22:30 We don't have a way to change 'Cancel' to 'Done' right now, and it only makes sense for iPhone; maybe that setting is a platform specific? 	
			// For cases where the popover is informational (rather than asking for a value), "Done" makes more sense
			wrapper.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, args) =>
			{
				wrapper.DismissModalViewController(true);
				popup.LightDismiss();
			});

			var controller = new UINavigationController(wrapper);
			controller.NavigationBar.Translucent = false;

			return controller;
		}

		PopupWrapper(Popup<T> popup)
		{
			var view = popup.View;

			var contentPage = new ContentPage { Content = view };
			
			_renderer = Platform.CreateRenderer(contentPage);
			Platform.SetRenderer(contentPage, _renderer);
			contentPage.Parent = Application.Current.MainPage;

			InitializeViewController(popup);
		}

		void InitializeViewController(Popup<T> popup)
		{
			Title = popup.Title;

			View.BackgroundColor = UIColor.White;
			View.AddSubview(_renderer.ViewController.View);
			AddChildViewController(_renderer.ViewController);

			_renderer.ViewController.DidMoveToParentViewController(this);

			ModalInPopover = !popup.IsLightDismissEnabled;
			ModalPresentationStyle = UIModalPresentationStyle.Popover;

			if (!popup.Size.IsZero)
			{
				PreferredContentSize = new CGSize(popup.Size.Width, popup.Size.Height);
			}
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			_renderer?.SetElementSize(new Size(View.Bounds.Width, View.Bounds.Height));
		}

		public override void ViewWillAppear(bool animated)
		{
			View.BackgroundColor = UIColor.White;
			base.ViewWillAppear(animated);
		}
	}
}