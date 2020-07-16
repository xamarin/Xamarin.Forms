using System;
using System.ComponentModel;
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	public class PopupRenderer : UIViewController, IVisualElementRenderer
	{
		public IVisualElementRenderer Control { get; private set; }
		public BasePopup Element { get; private set; }
		VisualElement IVisualElementRenderer.Element { get => Element; }
		public UIView NativeView { get => base.View; }
		public UIViewController ViewController { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		bool _isDisposed;

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

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));
			
			if (!(element is BasePopup popup))
				throw new ArgumentNullException("Element is not of type " + typeof(BasePopup), nameof(element));

			var oldElement = Element;
			Element = popup;
			CreateControl();

			Performance.Start(out string reference);

			if (oldElement != null)
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			
			element.PropertyChanged += OnElementPropertyChanged;
			
			OnElementChanged(new ElementChangedEventArgs<BasePopup>(oldElement, Element));
			Element?.SendViewInitialized(NativeView);

			Performance.Stop(reference);
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<BasePopup> e)
		{
			if (e.NewElement != null && !_isDisposed)
			{
				ModalInPopover = true;
				ModalPresentationStyle = UIModalPresentationStyle.Popover;

				SetViewController();
				SetPresentationController();
				SetEvents();
				SetSize();
				SetLayout();
				SetBackgroundColor();
				SetView();
				AddToCurrentPageViewController();
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == BasePopup.VerticalOptionsProperty.PropertyName || 
				args.PropertyName == BasePopup.HorizontalOptionsProperty.PropertyName)
			{
				SetLayout();
			}
			else if (args.PropertyName == BasePopup.SizeProperty.PropertyName)
			{
				SetSize();
			}
			else if (args.PropertyName == BasePopup.ColorProperty.PropertyName)
			{
				SetBackgroundColor();
			}

			ElementPropertyChanged?.Invoke(this, args);
		}

		void CreateControl()
		{
			var view = Element.View;
			var contentPage = new ContentPage { Content = view, Padding = new Thickness(25) };

			Control = Platform.CreateRenderer(contentPage);
			Platform.SetRenderer(contentPage, Control);
			contentPage.Parent = Application.Current.MainPage;
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
			// TODO - This is currently not working because the background of the foreground object is not set to transparent
			base.View.BackgroundColor = Element.Color.ToUIColor();
			((UIPopoverPresentationController)PresentationController).BackgroundColor = Element.Color.ToUIColor();
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

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;
			if (disposing)
			{
				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					if (iOS.Platform.GetRenderer(Element) == this)
						Element.ClearValue(iOS.Platform.RendererProperty);

					Element = null;
				}
			}

			base.Dispose(disposing);
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