using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class PopupRenderer : UIViewController, IVisualElementRenderer
	{
		public IVisualElementRenderer Control { get; private set; }
		public Popup Element { get; private set; }
		VisualElement IVisualElementRenderer.Element { get => Element; }
		public UIView NativeView { get => View; }
		public UIViewController ViewController { get => this; }
		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public void SetElementSize(Size size)
		{
			Control?.SetElementSize(size);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			SetElementSize(new Size(View.Bounds.Width, View.Bounds.Height));
		}

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;
			Element = (Popup)element;

			OnElementChanged(new ElementChangedEventArgs<Popup>(oldElement, Element));
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Popup> e)
		{
			if (Control == null)
				CreateControl();
			else if (Element != null)
			{
				SetSize();
				SetBackgroundColor();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		void CreateControl()
		{
			var view = Element.View;
			var contentPage = new ContentPage { Content = view, Padding = new Thickness(25) };

			Control = Platform.CreateRenderer(contentPage);
			Platform.SetRenderer(contentPage, Control);
			contentPage.Parent = Application.Current.MainPage;

			ModalInPopover = true;
			ModalPresentationStyle = UIModalPresentationStyle.Popover;

			SetSize();
			SetBackgroundColor();
			SetView();
			SetPresentationController();
			AddToCurrentPageViewController();
		}

		void SetSize()
		{
			if (!Element.Size.IsZero)
			{
				PreferredContentSize = new CGSize(Element.Size.Width, Element.Size.Height);
				((UIPopoverPresentationController)PresentationController).SourceRect = new CGRect(0, 0, Element.Size.Width, Element.Size.Height);
			}
		}

		void SetBackgroundColor()
		{
			View.BackgroundColor = Element.BackgroundColor.ToUIColor();
			((UIPopoverPresentationController)PresentationController).BackgroundColor = Element.BackgroundColor.ToUIColor();
		}

		void SetView()
		{
			View.AddSubview(Control.ViewController.View);
			View.Bounds = Control.ViewController.View.Bounds;
			AddChildViewController(Control.ViewController);
		}

		void SetPresentationController()
		{
			var currentPageRenderer = Platform.GetRenderer(Application.Current.MainPage);
			
			((UIPopoverPresentationController)PresentationController).SourceView = currentPageRenderer.ViewController.View;
			((UIPopoverPresentationController)PresentationController).PermittedArrowDirections = UIPopoverArrowDirection.Up;
			((UIPopoverPresentationController)PresentationController).Delegate = new PopoverDelegate();
		}

		void AddToCurrentPageViewController()
		{
			var currentPageRenderer = Platform.GetRenderer(Application.Current.MainPage);
			currentPageRenderer.ViewController.PresentViewController(this, true, null);
		}

		private class PopoverDelegate : UIPopoverPresentationControllerDelegate
		{
			public override UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController)
			{
				return UIModalPresentationStyle.None;
			}
		}
	}
}