using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal class FlyoutPageContainer : ViewGroup
	{
		const int DefaultFlyoutSize = 320;
		const int DefaultSmallFlyoutSize = 240;
		readonly bool _isFlyout;
		readonly FlyoutPage _parent;
		VisualElement _childView;

		public FlyoutPageContainer(FlyoutPage parent, bool isFlyout, Context context) : base(context)
		{
			_parent = parent;
			_isFlyout = isFlyout;
		}

		public FlyoutPageContainer(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

		IFlyoutPageController FlyoutPageController => _parent as IFlyoutPageController;

		public VisualElement ChildView
		{
			get { return _childView; }
			set
			{
				if (_childView == value)
					return;

				RemoveAllViews();
				if (_childView != null)
					DisposeChildRenderers();

				_childView = value;

				if (_childView == null)
					return;
				
				AddChildView(_childView);
			}
		}

		protected virtual void AddChildView(VisualElement childView)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer(childView);
			if (renderer == null)
				Platform.SetRenderer(childView, renderer = Platform.CreateRenderer(childView, Context));

			if (renderer.View.Parent != this)
			{
				if (renderer.View.Parent != null)
					renderer.View.RemoveFromParent();
				SetDefaultBackgroundColor(renderer);
				AddView(renderer.View);
				renderer.UpdateLayout();
			}
		}

		public int TopPadding { get; set; }

		double DefaultWidthFlyout
		{
			get
			{
				double w = Context.FromPixels(Resources.DisplayMetrics.WidthPixels);
				return w < DefaultSmallFlyoutSize ? w : (w < DefaultFlyoutSize ? DefaultSmallFlyoutSize : DefaultFlyoutSize);
			}
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			bool isShowingPopover = _parent.IsPresented && !FlyoutPageController.ShouldShowSplitMode;
			if (!_isFlyout && isShowingPopover)
				return true;
			return base.OnInterceptTouchEvent(ev);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				RemoveAllViews();
				DisposeChildRenderers();
			}
			base.Dispose(disposing);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			if (_childView == null)
				return;

			Rectangle bounds = GetBounds(_isFlyout, l, t, r, b);
			if (_isFlyout)
				FlyoutPageController.FlyoutBounds = bounds;
			else
				FlyoutPageController.DetailBounds = bounds;

			IVisualElementRenderer renderer = Platform.GetRenderer(_childView);
			renderer?.UpdateLayout();
		}

		void DisposeChildRenderers()
		{
			IVisualElementRenderer childRenderer = Platform.GetRenderer(_childView);
			childRenderer?.Dispose();
			_childView?.ClearValue(Platform.RendererProperty);
		}

		Rectangle GetBounds(bool isFlyout, int left, int top, int right, int bottom)
		{
			double width = Context.FromPixels(right - left);
			double height = Context.FromPixels(bottom - top);
			double xPos = 0;
			bool supressPadding = false;

			//splitview
			if (FlyoutPageController.ShouldShowSplitMode)
			{
				//to keep some behavior we have on iPad where you can toggle and it won't do anything 
				bool isDefaultNoToggle = _parent.FlyoutLayoutBehavior == FlyoutLayoutBehavior.Default;
				xPos = isFlyout ? 0 : (_parent.IsPresented || isDefaultNoToggle ? DefaultWidthFlyout : 0);
				width = isFlyout ? DefaultWidthFlyout : _parent.IsPresented || isDefaultNoToggle ? width - DefaultWidthFlyout : width;
			}
			else
			{
				//if we are showing the normal popover flyout doesn't have padding
				supressPadding = isFlyout;
				//popover make the flyout smaller
				width = isFlyout && (Device.Info.CurrentOrientation.IsLandscape() || Device.Idiom == TargetIdiom.Tablet) ? DefaultWidthFlyout : width;
			}

			double padding = supressPadding ? 0 : Context.FromPixels(TopPadding);
			return new Rectangle(xPos, padding, width, height - padding);
		}

		protected void SetDefaultBackgroundColor(IVisualElementRenderer renderer)
		{
			if (ChildView.BackgroundColor == Color.Default)
			{
				TypedArray colors = Context.Theme.ObtainStyledAttributes(new[] { global::Android.Resource.Attribute.ColorBackground });
				renderer.View.SetBackgroundColor(new global::Android.Graphics.Color(colors.GetColor(0, 0)));
			}
		}
	}
}
