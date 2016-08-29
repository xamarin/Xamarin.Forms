using System;

using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
using System.Collections.Generic;
using AppKit;
using System.Collections.Specialized;
using System.ComponentModel;

using Xamarin.Forms.Internals;
using Foundation;
using ObjCRuntime;

namespace Xamarin.Forms.Platform.MacOS
{
	public class CarouselPageRenderer : NSViewController, IVisualElementRenderer
	{
		bool _appeared;
		Dictionary<Page, NSView> _containerMap;
		bool _disposed;
		EventTracker _events;
		bool _ignoreNativeScrolling;
		FormsScrollView _scrollView;
		VisualElementTracker _tracker;

		public CarouselPageRenderer()
		{
			View = _scrollView = new FormsScrollView
			{
				DocumentView = new FormsNSView { BackgroundColor = NSColor.Clear },
				HasVerticalScroller = false,
				HasHorizontalScroller = true,
				VerticalScrollElasticity = NSScrollElasticity.None,
				HorizontalScrollElasticity = NSScrollElasticity.None
			};

			_scrollView.ScrollChanged += FormsScrollViewScrollChanged;
		}

		IElementController ElementController => Element as IElementController;

		protected CarouselPage Carousel
		{
			get { return (CarouselPage)Element; }
		}

		IPageController PageController => (IPageController)Element;

		protected int SelectedIndex
		{
			get { return (int)(_scrollView.ContentView.Bounds.Location.X / _scrollView.Frame.Width); }
			set { ScrollToPage(value); }
		}

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public NSView NativeView
		{
			get { return View; }
		}

		public void SetElement(VisualElement element)
		{
			VisualElement oldElement = Element;
			Element = element;
			_containerMap = new Dictionary<Page, NSView>();

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

		}

		public void SetElementSize(Size size)
		{
			Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
		}

		public NSViewController ViewController
		{
			get { return this; }
		}

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

		public override void ViewDidLayout()
		{
			base.ViewDidLayout();
			View.Frame = View.Superview.Bounds;
			_scrollView.Frame = View.Bounds;
		}

		public override void ViewWillAppear()
		{
			base.ViewWillAppear();
			Init();
		}

		public void Init()
		{

			_tracker = new VisualElementTracker(this);
			_events = new EventTracker(this);
			_events.LoadEvents(View);

			UpdateBackground();


			for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
			{
				Element element = ElementController.LogicalChildren[i];
				var child = element as ContentPage;
				if (child != null)
					InsertPage(child, i);
			}

			PositionChildren();
			UpdateCurrentPage(false);

			Carousel.PropertyChanged += OnPropertyChanged;
			Carousel.PagesChanged += OnPagesChanged;
		}

		public override void ViewWillDisappear()
		{
			base.ViewWillDisappear();
			if (_scrollView != null)
			{
				_scrollView.ScrollChanged -= FormsScrollViewScrollChanged;
			}
			if (Carousel != null)
			{
				Carousel.PropertyChanged -= OnPropertyChanged;
				Carousel.PagesChanged -= OnPagesChanged;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				if (Carousel != null)
				{
					Carousel.PropertyChanged -= OnPropertyChanged;
					Carousel.PagesChanged -= OnPagesChanged;
				}

				Platform.SetRenderer(Element, null);

				Clear();

				if (_scrollView != null)
				{
					_scrollView.ScrollChanged -= FormsScrollViewScrollChanged;
					_scrollView.RemoveFromSuperview();
					_scrollView = null;
				}

				if (_appeared)
				{
					_appeared = false;
					PageController?.SendDisappearing();
				}

				if (_events != null)
				{
					_events.Dispose();
					_events = null;
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
			EventHandler<VisualElementChangedEventArgs> changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		void FormsScrollViewScrollChanged(object sender, FormsScrollViewScrollChangedEventArgs e)
		{
			if (e.CurrentScrollPoint.X % _scrollView.Bounds.Width != 0)
				return;

			Carousel.CurrentPage = (ContentPage)ElementController.LogicalChildren[SelectedIndex];
		}

		void Clear()
		{
			foreach (KeyValuePair<Page, NSView> kvp in _containerMap)
			{
				kvp.Value.RemoveFromSuperview();
				IVisualElementRenderer renderer = Platform.GetRenderer(kvp.Key);
				if (renderer != null)
				{
					renderer.ViewController.RemoveFromParentViewController();
					renderer.NativeView.RemoveFromSuperview();
					Platform.SetRenderer(kvp.Key, null);
				}
			}
			_containerMap.Clear();
		}

		void InsertPage(ContentPage page, int index)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer(page);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(page);
				Platform.SetRenderer(page, renderer);
			}

			NSView container = new PageContainer(page);
			container.AddSubview(renderer.NativeView);
			_containerMap[page] = container;

			AddChildViewController(renderer.ViewController);
			(_scrollView.DocumentView as NSView).AddSubview(container);

			if ((index == 0 && SelectedIndex == 0) || (index < SelectedIndex))
				ScrollToPage(SelectedIndex + 1, false);
		}

		void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_ignoreNativeScrolling = true;

			NotifyCollectionChangedAction action = e.Apply((o, i, c) => InsertPage((ContentPage)o, i), (o, i) => RemovePage((ContentPage)o, i), Reset);
			PositionChildren();

			_ignoreNativeScrolling = false;

			if (action == NotifyCollectionChangedAction.Reset)
			{
				int index = Carousel.CurrentPage != null ? CarouselPage.GetIndex(Carousel.CurrentPage) : 0;
				if (index < 0)
					index = 0;

				ScrollToPage(index);
			}
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentPage")
				UpdateCurrentPage();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackground();
			else if (e.PropertyName == Page.BackgroundImageProperty.PropertyName)
				UpdateBackground();
		}

		void PositionChildren()
		{
			nfloat x = 0;
			RectangleF bounds = View.Bounds;
			foreach (ContentPage child in ((CarouselPage)Element).Children)
			{
				NSView container = _containerMap[child];

				container.Frame = new RectangleF(x, bounds.Y, bounds.Width, bounds.Height);
				x += bounds.Width;
			}

			var contentSize = new SizeF(bounds.Width * ((CarouselPage)Element).Children.Count, bounds.Height);
			(_scrollView.DocumentView as NSView).Frame = new RectangleF(0, Element.Height - contentSize.Height, contentSize.Width, contentSize.Height);

			_scrollView.CustompageScroll = bounds.Width;

		}

		void RemovePage(ContentPage page, int index)
		{
			NSView container = _containerMap[page];
			container.RemoveFromSuperview();
			_containerMap.Remove(page);

			IVisualElementRenderer renderer = Platform.GetRenderer(page);
			if (renderer == null)
				return;

			renderer.ViewController.RemoveFromParentViewController();
			renderer.NativeView.RemoveFromSuperview();
		}

		void Reset()
		{
			Clear();

			for (var i = 0; i < ElementController.LogicalChildren.Count; i++)
			{
				Element element = ElementController.LogicalChildren[i];
				var child = element as ContentPage;
				if (child != null)
					InsertPage(child, i);
			}
		}

		async void ScrollToPage(int index, bool animated = true)
		{
			if (_scrollView.ContentView.Bounds.Location.X == index * _scrollView.Frame.Width)
				return;

			var scrollPoint = new PointF(index * _scrollView.Frame.Width, 0);

			await _scrollView.ScrollToPositionAsync(scrollPoint, animated, 0.2);
		}


		void UpdateBackground()
		{
			string bgImage = ((Page)Element).BackgroundImage;
			if (!string.IsNullOrEmpty(bgImage))
			{
				_scrollView.BackgroundColor = NSColor.FromPatternImage(NSImage.ImageNamed(bgImage));
				return;
			}
			Color bgColor = Element.BackgroundColor;
			if (bgColor.IsDefault)
				_scrollView.BackgroundColor = NSColor.White;
			else
				_scrollView.BackgroundColor = bgColor.ToNSColor();
		}

		void UpdateCurrentPage(bool animated = true)
		{
			ContentPage current = Carousel.CurrentPage;
			if (current != null)
				ScrollToPage(CarouselPage.GetIndex(current), animated);
		}


		class PageContainer : NSView
		{
			public PageContainer(VisualElement element)
			{
				Element = element;
			}

			public VisualElement Element { get; }

			public override void Layout()
			{
				base.Layout();

				if (Subviews.Length > 0)
					Subviews[0].Frame = new RectangleF(0, 0, (float)Element.Width, (float)Element.Height);
			}
		}
	}
}
