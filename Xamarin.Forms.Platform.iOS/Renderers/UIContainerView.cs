using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class UIContainerView : UIView
	{
		readonly View _view;
		IVisualElementRenderer _renderer;
		bool _disposed;

		public UIContainerView(View view)
		{
			_view = view;

			_renderer = Platform.CreateRenderer(view);
			Platform.SetRenderer(view, _renderer);

			AddSubview(_renderer.NativeView);
			ClipsToBounds = true;
		}

		public override void LayoutSubviews()
		{
			if (Bounds.Height == 0)
			{
				var request = _view.Measure(Frame.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
				Layout.LayoutChildIntoBoundingRegion(_view, new Rectangle(0, 0, Frame.Width, request.Request.Height));
				Frame = new CGRect(Frame.X, Frame.Y, Frame.Width, request.Request.Height);
			}
			else
			{
				_view.Layout(Bounds.ToRectangle());
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_renderer?.Dispose();
				_renderer = null;
				_view.ClearValue(Platform.RendererProperty);

				_disposed = true;
			}

			base.Dispose(disposing);
		}
	}
}