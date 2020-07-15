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
		public UIView NativeView { get => base.View; }
		public UIViewController ViewController { get; private set; }
		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		[Internals.Preserve(Conditional = true)]
		public PopupRenderer()
		{

		}

		public void SetElementSize(Size size)
		{
			Control?.SetElementSize(size);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			SetElementSize(new Size(base.View.Bounds.Width, base.View.Bounds.Height));
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

			SetViewController();
			SetEvents();
			SetSize();
			SetLayout();
			SetBackgroundColor();
			SetView();
			SetPresentationController();
			AddToCurrentPageViewController();
		}

		void SetViewController()
		{
			var currentPageRenderer = Platform.GetRenderer(Application.Current.MainPage);
			ViewController = currentPageRenderer.ViewController;
		}

		void SetEvents()
		{
			Element.Dismissed += OnDismissed;
		}

		void SetSize()
		{
			if (!Element.Size.IsZero)
			{
				PreferredContentSize = new CGSize(Element.Size.Width, Element.Size.Height);
			}
		}

		void SetLayout()
		{
			((UIPopoverPresentationController)PresentationController).SourceRect = new CGRect(0, 0, PreferredContentSize.Width, PreferredContentSize.Height);

			nfloat originX = 0;
			nfloat originY = 0;

			switch (Element.VerticalOptions.Alignment)
			{
				case LayoutAlignment.End:
					originY = UIScreen.MainScreen.Bounds.Height - PreferredContentSize.Height;
					break;
				case LayoutAlignment.Center:
					originY = (UIScreen.MainScreen.Bounds.Height / 2) - (PreferredContentSize.Height / 2);
					break;
			}

			switch (Element.HorizontalOptions.Alignment)
			{
				case LayoutAlignment.End:
					originX = UIScreen.MainScreen.Bounds.Width;
					break;
				case LayoutAlignment.Center:
					originX = UIScreen.MainScreen.Bounds.Width / 2;
					break;
			}

			PopoverPresentationController.SourceRect = new CGRect(originX, originY, 0, 0);
		}

		void SetBackgroundColor()
		{
			base.View.BackgroundColor = Element.BackgroundColor.ToUIColor();
			((UIPopoverPresentationController)PresentationController).BackgroundColor = Element.BackgroundColor.ToUIColor();
		}

		void SetView()
		{
			base.View.AddSubview(Control.ViewController.View);
			base.View.Bounds = new CGRect(0, 0, PreferredContentSize.Width, PreferredContentSize.Height);// //Control.ViewController.View.Bounds;
			AddChildViewController(Control.ViewController);
		}

		void SetPresentationController()
		{
			((UIPopoverPresentationController)PresentationController).SourceView = ViewController.View;
			((UIPopoverPresentationController)PresentationController).PermittedArrowDirections = UIPopoverArrowDirection.Up;
			((UIPopoverPresentationController)PresentationController).Delegate = new PopoverDelegate();
		}

		void AddToCurrentPageViewController()
		{
			ViewController.PresentViewController(this, true, null);
		}

		// REVIEW - Is there a better way to handle this other than 'async void'
		async void OnDismissed(object sender, PopupDismissedEventArgs e)
		{
			if (!Forms.IsiOS9OrNewer)
				await ViewController.DismissViewControllerAsync(true);
			else
				ViewController.DismissViewController(true, null);
		}

		class PopoverDelegate : UIPopoverPresentationControllerDelegate
		{
			public override UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController)
			{
				return UIModalPresentationStyle.None;
			}
		}
	}
}