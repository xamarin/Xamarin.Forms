using System;
using ElmSharp;
using ElmSharp.Wearable;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen.Native.Watch
{
	public class WatchScroller : Native.Scroller, IRotaryActionWidget, IRotaryInteraction
	{
		CircleScroller _circleScroller;
		CircleSurface _surface;

		public IntPtr CircleHandle => _circleScroller.CircleHandle;

		public CircleSurface CircleSurface => _surface;

		public IRotaryActionWidget RotaryWidget { get => this; }

		public WatchScroller(EvasObject parent, CircleSurface surface)
		{
			_surface = surface;
			Realize(parent);

			VerticalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Invisible;
			HorizontalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Invisible;
		}

		public override ScrollBarVisiblePolicy VerticalScrollBarVisiblePolicy
		{
			get => _circleScroller.VerticalScrollBarVisiblePolicy;
			set => _circleScroller.VerticalScrollBarVisiblePolicy = value;
		}

		public override ScrollBarVisiblePolicy HorizontalScrollBarVisiblePolicy
		{
			get => _circleScroller.HorizontalScrollBarVisiblePolicy;
			set => _circleScroller.HorizontalScrollBarVisiblePolicy = value;
		}

		public EColor VerticalScrollBarColor
		{
			get => _circleScroller.VerticalScrollBarColor;
			set => _circleScroller.VerticalScrollBarColor = value;
		}

		public EColor HorizontalScrollBarColor
		{
			get => _circleScroller.HorizontalScrollBarColor;
			set => _circleScroller.HorizontalScrollBarColor = value;
		}

		protected override IntPtr CreateHandle(EvasObject parent)
		{
			_circleScroller = new CircleScroller(parent, _surface);
			RealHandle = _circleScroller.RealHandle;
			return _circleScroller.Handle;
		}
	}
}
