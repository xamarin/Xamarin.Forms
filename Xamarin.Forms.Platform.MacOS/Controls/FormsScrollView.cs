using System;
using AppKit;
using Foundation;
using ObjCRuntime;
using PointF = CoreGraphics.CGPoint;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class FormsScrollView : NSScrollView
	{
		bool _shouldHandleCustomScrolling = true;

		public EventHandler<FormsScrollViewScrollChangedEventArgs> ScrollChanged;

		public FormsScrollView(bool handleCustomScrolling = true)
		{
			_shouldHandleCustomScrolling = handleCustomScrolling;

			ContentView.PostsBoundsChangedNotifications = true;

			NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector(nameof(UpdateScrollPosition)), NSView.BoundsChangedNotification, ContentView);
		}

		public nfloat CustompageScroll
		{
			get;
			set;
		}

		public override async void ScrollWheel(NSEvent theEvent)
		{
			if (!_shouldHandleCustomScrolling)
			{
				base.ScrollWheel(theEvent);
				return;
			}
			if (theEvent.MomentumPhase != NSEventPhase.Began)
				return;

			var scrollingX = theEvent.ScrollingDeltaX;
			var c = ContentView.Bounds.Location.X;

			if (scrollingX > 0)
				c -= CustompageScroll;
			else if (scrollingX < 0)
				c += CustompageScroll;

			await this.ScrollToPositionAsync(new PointF(Math.Max(c, 0), ContentView.Bounds.Location.Y), true, 0.2);

		}

		[Export(nameof(UpdateScrollPosition))]
		void UpdateScrollPosition()
		{
			ScrollChanged?.Invoke(this, new FormsScrollViewScrollChangedEventArgs { CurrentScrollPoint = ContentView.Bounds.Location });
		}
	}
}
