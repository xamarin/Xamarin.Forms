using System;
using System.ComponentModel;
using CoreGraphics;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace SkiaSharpVisual
{
	public partial class SkiaSharpButtonRenderer: ViewRenderer<Button, SKCanvasView>
	{
		bool _isDisposed;
		UITapGestureRecognizer _tapGestures;

		public SkiaSharpButtonRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (Control != null)
			{
				if (_tapGestures != null)
				{
					Control.RemoveGestureRecognizer(_tapGestures);
					_tapGestures.Dispose();
					_tapGestures = null;
				}
				Control.PaintSurface -= OnPaintSurface;
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

					_tapGestures = new UITapGestureRecognizer(OnTapRecognized);
					Control.AddGestureRecognizer(_tapGestures);
					Control.PaintSurface += OnPaintSurface;
				}

				UpdatePaints();
				Control.SetNeedsDisplay();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			UpdatePaints(e);
			Control?.SetNeedsDisplay();
		}

		protected override SKCanvasView CreateNativeControl()
		{
			return new SKCanvasView();
		}

		//public override CGSize SizeThatFits(CGSize size)
		//{
		//	return GetMeasuredSize(size.ToSKSize()).ToSize();
		//}

		protected override void SetBackgroundColor(Color color)
		{
			base.SetBackgroundColor(Color.Transparent);
		}

		void OnTapRecognized(UIGestureRecognizer recognizer)
		{
			Element?.SendClicked();
		}
	}
}
