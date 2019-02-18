using System.ComponentModel;
using Android.Content;
using Android.Views;
using SkiaSharp;
using SkiaSharp.Views.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;

namespace SkiaSharpVisual
{
	public partial class SkiaSharpButtonRenderer : ViewRenderer<Button, SKCanvasView>,
		AView.IOnClickListener, AView.IOnTouchListener
	{
		bool _isDisposed;

		public SkiaSharpButtonRenderer(Context context)
			: base(context)
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (Control != null)
			{
				Control.PaintSurface -= OnPaintSurface;
				Control.SetOnClickListener(null);
				Control.SetOnTouchListener(null);
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());

					Control.PaintSurface += OnPaintSurface;
					Control.SetOnClickListener(this);
					Control.SetOnTouchListener(this);
				}

				UpdatePaints();
				Invalidate();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			UpdatePaints(e);
			Control?.Invalidate();
		}

		protected override SKCanvasView CreateNativeControl()
		{
			return new SKCanvasView(Context);
		}

		//protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		//{
		//	var size = GetMeasuredSize(new SKSize(widthMeasureSpec, heightMeasureSpec));
		//	SetMeasuredDimension((int)size.Width, (int)size.Height);
		//}

		protected override void UpdateBackgroundColor()
		{
			// no-op
		}

		void IOnClickListener.OnClick(AView v) => ButtonElementManager.OnClick(Element, Element, v);

		bool IOnTouchListener.OnTouch(AView v, MotionEvent e) => ButtonElementManager.OnTouch(Element, Element, v, e);
	}
}
