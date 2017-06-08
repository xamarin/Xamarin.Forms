using System;
using System.ComponentModel;
using ElmSharp;

namespace Xamarin.Forms.Platform.Tizen
{
	/// <summary>
	/// This class provides a Renderer for a ScrollView widget.
	/// </summary>
	public class ScrollViewRenderer : ViewRenderer<ScrollView, Scroller>
	{
		EvasObject _content;

		/// <summary>
		/// Initializes a new instance of the <see cref="Xamarin.Forms.Platform.Tizen.ScrollViewRenderer"/> class.
		/// </summary>
		public ScrollViewRenderer()
		{
			RegisterPropertyHandler("Content", FillContent);
		}

		/// <summary>
		/// Handles the element change event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
		{
			if (Control == null)
			{
				var scrollView = new Scroller(Forms.Context.MainWindow);
				SetNativeControl(scrollView);
			}

			if (e.OldElement != null)
			{
				Control.Scrolled -= ScrollViewScrolledHandler;
				(e.OldElement as IScrollViewController).ScrollToRequested -= ScrollRequestHandler;
			}

			if (e.NewElement != null)
			{
				Control.Scrolled += ScrollViewScrolledHandler;
				(e.NewElement as IScrollViewController).ScrollToRequested += ScrollRequestHandler;
			}

			UpdateAll();

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (null != Element)
			{
				(Element as IScrollViewController).ScrollToRequested -= ScrollRequestHandler;
			}

			base.Dispose(disposing);
		}

		void FillContent()
		{
			if (_content != null)
			{
				Control.SetContent(null, true);
			}

			_content = Platform.GetOrCreateRenderer(Element.Content).NativeView;

			if (_content != null)
			{
				Control.SetContent(_content, true);
				UpdateContentSize();
			}
		}

		void UpdateAll()
		{
			UpdateOrientation();
		}

		void UpdateOrientation()
		{
			switch (Element.Orientation)
			{
				case ScrollOrientation.Horizontal:
					Control.ScrollBlock = ScrollBlock.Vertical;
					Control.HorizontalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Auto;
					Control.VerticalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Invisible;
					break;
				case ScrollOrientation.Vertical:
					Control.ScrollBlock = ScrollBlock.Horizontal;
					Control.HorizontalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Invisible;
					Control.VerticalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Auto;
					break;
				default:
					Control.ScrollBlock = ScrollBlock.None;
					Control.HorizontalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Auto;
					Control.VerticalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Auto;
					break;
			}
		}

		void UpdateContentSize()
		{
			if (_content == null)
				return;

			_content.MinimumWidth = Forms.ConvertToScaledPixel(Element.ContentSize.Width);
			_content.MinimumHeight = Forms.ConvertToScaledPixel(Element.ContentSize.Height);
		}

		/// <summary>
		/// An event raised on element's property change.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ScrollView.OrientationProperty.PropertyName == e.PropertyName)
			{
				UpdateOrientation();
			}
			else if (ScrollView.ContentSizeProperty.PropertyName == e.PropertyName)
			{
				UpdateContentSize();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		void ScrollViewScrolledHandler(object sender, EventArgs e)
		{
			var region = Control.CurrentRegion;
			((IScrollViewController)Element).SetScrolledPosition(region.X, region.Y);
		}

		void ScrollRequestHandler(object sender, ScrollToRequestedEventArgs e)
		{
			var x = e.ScrollX;
			var y = e.ScrollY;
			if (e.Mode == ScrollToMode.Element)
			{
				Point itemPosition = (Element as IScrollViewController).GetScrollPositionForElement(e.Element as VisualElement, e.Position);
				x = itemPosition.X;
				y = itemPosition.Y;
			}

			Rect region = new Rectangle(x, y, Element.Width, Element.Height).ToPixel();
			Control.ScrollTo(region, e.ShouldAnimate);
		}
	}
}
