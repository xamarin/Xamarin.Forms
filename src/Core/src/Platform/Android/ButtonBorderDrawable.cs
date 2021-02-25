using System;
using System.Linq;
using Android.Graphics;
using Android.Graphics.Drawables;
using AColor = Android.Graphics.Color;
using APath = Android.Graphics.Path;
using XColor = Microsoft.Maui.Color;

namespace Microsoft.Maui
{
	internal class ButtonBorderDrawable : Drawable
	{
		public const int DefaultCornerRadius = 2; // Default value for Android material button.

		readonly Func<double, float> _convertToPixels;
		bool _isDisposed;
		Bitmap? _normalBitmap;
		bool _pressed;
		Bitmap? _pressedBitmap;
		readonly XColor _defaultColor;

		public ButtonBorderDrawable(Func<double, float> convertToPixels, XColor defaultColor)
		{
			_convertToPixels = convertToPixels;
			_pressed = false;
			_defaultColor = defaultColor;
		}

		public IBorder? BorderElement
		{
			get;
			set;
		}

		public override bool IsStateful
		{
			get { return true; }
		}

		public override int Opacity
		{
			get { return 0; }
		}

		public override void Draw(Canvas canvas)
		{
			//Bounds = new Rect(Bounds.Left, Bounds.Top, Bounds.Right + (int)_convertToPixels(10), Bounds.Bottom + (int)_convertToPixels(10));
			int width = Bounds.Width();
			int height = Bounds.Height();

			if (width <= 0 || height <= 0)
				return;

			if (_normalBitmap == null ||
				_normalBitmap?.IsDisposed() == true ||
				_pressedBitmap?.IsDisposed() == true ||
				_normalBitmap?.Height != height ||
				_normalBitmap?.Width != width)
				Reset();

			if (BorderElement?.BackgroundColor == XColor.Default)
				return;

			Bitmap? bitmap;

			if (GetState().Contains(global::Android.Resource.Attribute.StatePressed))
			{
				_pressedBitmap ??= CreateBitmap(true, width, height);
				bitmap = _pressedBitmap;
			}
			else
			{
				_normalBitmap ??= CreateBitmap(false, width, height);
				bitmap = _normalBitmap;
			}

			if (bitmap != null)
				canvas.DrawBitmap(bitmap, 0, 0, new Paint());
		}

		public void Reset()
		{
			if (_normalBitmap != null)
			{
				if (!_normalBitmap.IsDisposed())
				{
					_normalBitmap.Recycle();
					_normalBitmap.Dispose();
				}
				_normalBitmap = null;
			}

			if (_pressedBitmap != null)
			{
				if (!_pressedBitmap.IsDisposed())
				{
					_pressedBitmap.Recycle();
					_pressedBitmap.Dispose();
				}
				_pressedBitmap = null;
			}
		}

		public override void SetAlpha(int alpha)
		{
		}

		public override void SetColorFilter(ColorFilter? cf)
		{
		}

		public XColor BackgroundColor => (BorderElement == null || BorderElement?.BackgroundColor == XColor.Default) ? _defaultColor : BorderElement!.BackgroundColor;
		public XColor PressedBackgroundColor => BackgroundColor.AddLuminosity(-.12); //<item name="highlight_alpha_material_light" format="float" type="dimen">0.12</item>

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (disposing)
				Reset();

			base.Dispose(disposing);
		}

		protected override bool OnStateChange(int[]? state)
		{
			bool old = _pressed;
			_pressed = state.Contains(Android.Resource.Attribute.StatePressed);

			if (_pressed != old)
			{
				InvalidateSelf();
				return true;
			}

			return false;
		}

		Bitmap? CreateBitmap(bool pressed, int width, int height)
		{
			Bitmap? bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!);

			if (bitmap != null)
			{
				using var canvas = new Canvas(bitmap);
					DrawBackground(canvas, width, height, pressed);
			}

			return bitmap;
		}

		void DrawBackground(Canvas canvas, int width, int height, bool pressed)
		{
			var paint = new Paint { AntiAlias = true };
			var path = new APath();

			float borderRadius = ConvertCornerRadiusToPixels();

			RectF rect = new RectF(0, 0, width, height);

			path.AddRoundRect(rect, borderRadius, borderRadius, APath.Direction.Cw!);

			paint.Color = pressed ? PressedBackgroundColor.ToNative() : BackgroundColor.ToNative();
			paint.SetStyle(Paint.Style.Fill);

			canvas.DrawPath(path, paint);
		}

		float ConvertCornerRadiusToPixels()
		{
			int cornerRadius = DefaultCornerRadius;

			if (BorderElement?.CornerRadius != 0)
				cornerRadius = BorderElement!.CornerRadius;

			return _convertToPixels(cornerRadius);
		}
	}
}