using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using ACanvas = Android.Graphics.Canvas;
using AColor = Android.Graphics.Color;

namespace Xamarin.Forms.Platform.Android
{
	public class BoxRenderer : VisualElementRenderer<BoxView>
	{
		bool _disposed;
		BoxViewDrawable _backgroundDrawable;

		readonly MotionEventHelper _motionEventHelper = new MotionEventHelper();

		public BoxRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use BoxRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BoxRenderer()
		{
			AutoPackage = false;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (base.OnTouchEvent(e))
				return true;

			return _motionEventHelper.HandleMotionEvent(Parent, e);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
		{
			base.OnElementChanged(e);

			_motionEventHelper.UpdateElement(e.NewElement);

			if (e.NewElement != null && e.OldElement == null)
			{
				_backgroundDrawable?.Dispose();
				this.SetBackground(_backgroundDrawable = new BoxViewDrawable(Element, Context.ToPixels));
			}
		}

		protected override void UpdateBackgroundColor()
		{

		}

		protected override void UpdateBackground()
		{

		}
				
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (_backgroundDrawable != null)
				{
					_backgroundDrawable.Dispose();
					_backgroundDrawable = null;
				}

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					if (Platform.GetRenderer(Element) == this)
						Element.ClearValue(Platform.RendererProperty);
				}

			}

			base.Dispose(disposing);
		}

		class BoxViewDrawable : Drawable
		{
			readonly BoxView _boxView;
			readonly Func<double, float> _convertToPixels;

			bool _isDisposed;
			Bitmap _normalBitmap;

			public BoxViewDrawable(BoxView boxView, Func<double, float> convertToPixels)
			{
				_boxView = boxView;
				_convertToPixels = convertToPixels;
				boxView.PropertyChanged += BoxViewOnPropertyChanged;
			}

			public override bool IsStateful
			{
				get { return false; }
			}

			public override int Opacity
			{
				get { return 0; }
			}

			public override void Draw(ACanvas canvas)
			{
				int width = Bounds.Width();
				int height = Bounds.Height();

				if (width <= 0 || height <= 0)
				{
					if (_normalBitmap != null)
					{
						_normalBitmap.Dispose();
						_normalBitmap = null;
					}
					return;
				}

				if (_normalBitmap == null || _normalBitmap.Height != height || _normalBitmap.Width != width)
				{
					if (_normalBitmap != null)
					{
						_normalBitmap.Dispose();
						_normalBitmap = null;
					}

					_normalBitmap = CreateBitmap(width, height);
				}
				Bitmap bitmap = _normalBitmap;
				using (var paint = new Paint())
					canvas.DrawBitmap(bitmap, 0, 0, paint);
			}

			public override void SetAlpha(int alpha)
			{
			}

			public override void SetColorFilter(ColorFilter cf)
			{
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !_isDisposed)
				{
					if (_normalBitmap != null)
					{
						_normalBitmap.Dispose();
						_normalBitmap = null;
					}

					_isDisposed = true;
				}

				base.Dispose(disposing);
			}

			protected override bool OnStateChange(int[] state)
			{
				return false;
			}

			Bitmap CreateBitmap(int width, int height)
			{
				Bitmap bitmap;
				using (Bitmap.Config config = Bitmap.Config.Argb8888)
					bitmap = Bitmap.CreateBitmap(width, height, config);

				using (var canvas = new ACanvas(bitmap))
				{
					DrawCanvas(canvas, width, height);
				}

				return bitmap;
			}

			void DrawBackground(ACanvas canvas, int width, int height, CornerRadius cornerRadius)
			{
				using (var paint = new Paint { AntiAlias = true })
				using (var path = new Path())
				using (Path.Direction direction = Path.Direction.Cw)
				using (Paint.Style style = Paint.Style.Fill)
				using (var rect = new RectF(0, 0, width, height))
				{
					var cornerRadii = new[] {
						_convertToPixels(cornerRadius.TopLeft),
						_convertToPixels(cornerRadius.TopLeft),

						_convertToPixels(cornerRadius.TopRight),
						_convertToPixels(cornerRadius.TopRight),

						_convertToPixels(cornerRadius.BottomRight),
						_convertToPixels(cornerRadius.BottomRight),

						_convertToPixels(cornerRadius.BottomLeft),
						_convertToPixels(cornerRadius.BottomLeft)
					};

					path.AddRoundRect(rect, cornerRadii, direction);
					paint.SetStyle(style);

					if (_boxView.Background != null && !_boxView.Background.IsEmpty)
					{
						Brush background = _boxView.Background;
						paint.UpdateBackground(background, height, width);
					}
					else
					{
						if (!_boxView.Color.IsDefault)
						{
							AColor color = _boxView.Color.ToAndroid();
							paint.Color = color;
						}
						else
						{
							AColor backgroundColor = _boxView.BackgroundColor.ToAndroid();
							paint.Color = backgroundColor;
						}
					}

					canvas.DrawPath(path, paint);
				}
			}

			void BoxViewOnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName
					|| e.PropertyName == VisualElement.BackgroundProperty.PropertyName
					|| e.PropertyName == BoxView.ColorProperty.PropertyName
					|| e.PropertyName == BoxView.CornerRadiusProperty.PropertyName)
				{
					if (_normalBitmap == null)
						return;

					using (var canvas = new ACanvas(_normalBitmap))
					{
						int width = Bounds.Width();
						int height = Bounds.Height();
						canvas.DrawColor(AColor.Black, PorterDuff.Mode.Clear);
						DrawCanvas(canvas, width, height);
					}
					InvalidateSelf();
				}
			}

			void DrawCanvas(ACanvas canvas, int width, int height)
			{
				CornerRadius cornerRadius = _boxView.CornerRadius;

				if (cornerRadius == -1f)
					cornerRadius = 5f;

				DrawBackground(canvas, width, height, cornerRadius);
			}
		}
	}
}