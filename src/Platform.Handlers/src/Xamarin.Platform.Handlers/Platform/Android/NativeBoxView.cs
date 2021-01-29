using System;
using Android.Content;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using AView = Android.Views.View;

namespace Xamarin.Platform
{
	public class NativeBoxView : AView, IDisposable
	{
		Color _color;
		CornerRadius _cornerRadius;

		GradientDrawable? _backgroundDrawable;

		public NativeBoxView(Context? context) : base(context)
		{

		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				SetColor(_color);
			}
		}

		public CornerRadius CornerRadius
		{
			get { return _cornerRadius; }
			set
			{
				_cornerRadius = value;
				SetCornerRadius(_cornerRadius);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_backgroundDrawable != null)
			{
				_backgroundDrawable.Dispose();
				_backgroundDrawable = null;
			}

			base.Dispose(disposing);
		}

		void SetColor(Color color)
		{
			if (_backgroundDrawable != null)
			{
				if (color != Color.Default)
					_backgroundDrawable.SetColor(color.ToNative());
				else
					_backgroundDrawable.SetColor(color.ToNative(Color.Transparent));

				this.SetBackground(_backgroundDrawable);
			}
			else
			{
				SetBackgroundColor(color.ToNative(Color.Transparent));
			}
		}

		void SetCornerRadius(CornerRadius cornerRadius)
		{
			if (cornerRadius == new CornerRadius(0d))
			{
				_backgroundDrawable?.Dispose();
				_backgroundDrawable = null;
			}
			else
			{
				this.SetBackground(_backgroundDrawable = new GradientDrawable());
				if (Context != null && Background is GradientDrawable backgroundGradient)
				{
					var cornerRadii = new[] {
						Context.ToPixels(cornerRadius.TopLeft),
						Context.ToPixels(cornerRadius.TopLeft),

						Context.ToPixels(cornerRadius.TopRight),
						Context.ToPixels(cornerRadius.TopRight),

						Context.ToPixels(cornerRadius.BottomRight),
						Context.ToPixels(cornerRadius.BottomRight),

						Context.ToPixels(cornerRadius.BottomLeft),
						Context.ToPixels(cornerRadius.BottomLeft)
					};

					backgroundGradient.SetCornerRadii(cornerRadii);
				}
			}

			SetColor(Color);
		}
	}
}