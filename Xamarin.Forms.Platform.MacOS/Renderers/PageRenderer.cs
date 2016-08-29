using System;
using System.Collections.Generic;
using System.ComponentModel;
using AppKit;

namespace Xamarin.Forms.Platform.MacOS
{
	public class PageRenderer : NSViewController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _appeared;
		bool _disposed;
		EventTracker _events;
		VisualElementPackager _packager;
		VisualElementTracker _tracker;

		IPageController PageController => Element as IPageController;

		public PageRenderer()
		{
			View = new FormsNSView();
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			var platformEffect = effect as PlatformEffect;
			if (platformEffect != null)
				platformEffect.Container = View;
		}

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public NSView NativeView
		{
			get { return _disposed ? null : View; }
		}

		public void SetElement(VisualElement element)
		{
			VisualElement oldElement = Element;
			Element = element;
			UpdateTitle();

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			if (Element != null && !string.IsNullOrEmpty(Element.AutomationId))
				SetAutomationId(Element.AutomationId);


			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
		}

		public void SetElementSize(Size size)
		{
			Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
		}

		public NSViewController ViewController => _disposed ? null : this;

		public override void ViewDidAppear()
		{
			base.ViewDidAppear();

			if (_appeared || _disposed)
				return;

			_appeared = true;
			PageController.SendAppearing();
		}


		public override void ViewDidDisappear()
		{
			base.ViewDidDisappear();

			if (!_appeared || _disposed)
				return;

			_appeared = false;
			PageController.SendDisappearing();
		}

		public override void ViewWillDisappear()
		{
			base.ViewWillDisappear();

			//View.Window?.EndEditing(true);
		}

		public override void ViewWillAppear()
		{
			Init();
			base.ViewWillAppear();
		}

		public override void ViewDidLoad()
		{

			//Seems this is not being called
			base.ViewDidLoad();

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				Element.PropertyChanged -= OnHandlePropertyChanged;
				Platform.SetRenderer(Element, null);
				if (_appeared)
					PageController.SendDisappearing();

				_appeared = false;

				if (_events != null)
				{
					_events.Dispose();
					_events = null;
				}

				if (_packager != null)
				{
					_packager.Dispose();
					_packager = null;
				}

				if (_tracker != null)
				{
					_tracker.Dispose();
					_tracker = null;
				}

				Element = null;
				_disposed = true;
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			ElementChanged?.Invoke(this, e);
		}

		protected virtual void SetAutomationId(string id)
		{
			//if (NativeView != null)
			//	NativeView.AccessibilityIdentifier = id;
		}

		internal FormsNSView FormsNativeView => View as FormsNSView;

		void Init()
		{

			//var uiTapGestureRecognizer = new UITapGestureRecognizer(a => View.EndEditing(true));

			//uiTapGestureRecognizer.ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true;
			//uiTapGestureRecognizer.ShouldReceiveTouch = OnShouldReceiveTouch;
			//uiTapGestureRecognizer.DelaysTouchesBegan =
			//	uiTapGestureRecognizer.DelaysTouchesEnded = uiTapGestureRecognizer.CancelsTouchesInView = false;
			//View.AddGestureRecognizer(uiTapGestureRecognizer);

			UpdateBackground();

			_packager = new VisualElementPackager(this);
			_packager.Load();

			Element.PropertyChanged += OnHandlePropertyChanged;
			_tracker = new VisualElementTracker(this);

			_events = new EventTracker(this);
			_events.LoadEvents(View);
		}

		void OnHandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackground();
			else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
				UpdateBackground();
			else if (e.PropertyName == Page.TitleProperty.PropertyName)
				UpdateTitle();
		}

		//bool OnShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
		//{
		//	foreach (UIView v in ViewAndSuperviewsOfView(touch.View))
		//	{
		//		if (v is UITableView || v is UITableViewCell || v.CanBecomeFirstResponder)
		//			return false;
		//	}
		//	return true;
		//}

		void UpdateBackground()
		{
			string bgImage = ((Page)Element).BackgroundImage;
			if (!string.IsNullOrEmpty(bgImage))
			{
				FormsNativeView.BackgroundColor = NSColor.FromPatternImage(NSImage.ImageNamed(bgImage));
				return;
			}
			Color bgColor = Element.BackgroundColor;
			if (bgColor.IsDefault)
				FormsNativeView.BackgroundColor = NSColor.White;
			else
				FormsNativeView.BackgroundColor = bgColor.ToNSColor();
		}

		void UpdateTitle()
		{
			if (!string.IsNullOrWhiteSpace(((Page)Element).Title))
				Title = ((Page)Element).Title;
		}

		IEnumerable<NSView> ViewAndSuperviewsOfView(NSView view)
		{
			while (view != null)
			{
				yield return view;
				view = view.Superview;
			}
		}
	}
}