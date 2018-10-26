using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V7.Widget;
using AImageButton = Android.Widget.ImageButton;
using AView = Android.Views.View;
using Android.Views;
using Xamarin.Forms.Internals;
using AMotionEventActions = Android.Views.MotionEventActions;
using AColor = Android.Graphics.Color;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using AStateListDrawable = Android.Graphics.Drawables.StateListDrawable;
using ARect = Android.Graphics.Rect;
using Android.Graphics.Drawables;
using Android.Graphics;
using Xamarin.Forms.Platform.Android.FastRenderers;

namespace Xamarin.Forms.Platform.Android.AppCompat
{
	internal sealed class ImageButtonRenderer :
		AppCompatImageButton,
		IVisualElementRenderer,
		IBorderVisualElementRenderer,
		IImageRendererController,
		AView.IOnFocusChangeListener,
		AView.IOnClickListener,
		AView.IOnTouchListener
	{
		bool _inputTransparent;
		bool _disposed;
		bool _skipInvalidate;
		int? _defaultLabelFor;
		VisualElementTracker _tracker;
		VisualElementRenderer _visualElementRenderer;
		BorderBackgroundManager _backgroundTracker;
		IPlatformElementConfiguration<PlatformConfiguration.Android, ImageButton> _platformElementConfiguration;
		private ImageButton _imageButton;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		void IVisualElementRenderer.UpdateLayout() => _tracker?.UpdateLayout();
		VisualElement IVisualElementRenderer.Element => ImageButton;
		AView IVisualElementRenderer.View => this;
		ViewGroup IVisualElementRenderer.ViewGroup => null;
		VisualElementTracker IVisualElementRenderer.Tracker => _tracker;

		ImageButton ImageButton
		{
			get => _imageButton;
			set
			{
				_imageButton = value;
				_platformElementConfiguration = null;
			}
		}

		void IImageRendererController.SkipInvalidate() => _skipInvalidate = true;
		bool IImageRendererController.IsDisposed => _disposed;

		AppCompatImageButton Control => this;

		public ImageButtonRenderer(Context context) : base(context)
		{
			// These set the defaults so visually it matches up with other platforms
			Background = new AStateListDrawable();
			SetPadding(0, 0, 0, 0);
			SoundEffectsEnabled = false;
			SetOnClickListener(this);
			SetOnTouchListener(this);
			OnFocusChangeListener = this;

			Tag = this;
			_backgroundTracker = new BorderBackgroundManager(this, false);
		}
		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{

				ImageElementManager.Dispose(this);

				_tracker?.Dispose();
				_tracker = null;

				_backgroundTracker?.Dispose();
				_backgroundTracker = null;

				if (ImageButton != null)
				{
					ImageButton.PropertyChanged -= OnElementPropertyChanged;

					if (Android.Platform.GetRenderer(ImageButton) == this)
					{
						ImageButton.ClearValue(Android.Platform.RendererProperty);
					}

					ImageButton = null;
				}
			}

			base.Dispose(disposing);
		}

		public override void Invalidate()
		{
			if (_skipInvalidate)
			{
				_skipInvalidate = false;
				return;
			}

			base.Invalidate();
		}

		Size MinimumSize()
		{
			return new Size();
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (_disposed)
			{
				return new SizeRequest();
			}
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), MinimumSize());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (!(element is ImageButton image))
			{
				throw new ArgumentException("Element is not of type " + typeof(ImageButton), nameof(element));
			}

			ImageButton oldElement = ImageButton;
			ImageButton = image;

			var reference = Guid.NewGuid().ToString();
			Performance.Start(reference);

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			element.PropertyChanged += OnElementPropertyChanged;

			if (_tracker == null)
			{
				_tracker = new VisualElementTracker(this);
				ImageElementManager.Init(this);

			}

			if (_visualElementRenderer == null)
			{
				_visualElementRenderer = new VisualElementRenderer(this);
			}

			Performance.Stop(reference);
			this.EnsureId();

			UpdateInputTransparent();
			UpdatePadding();

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(oldElement, ImageButton));
			ImageButton?.SendViewInitialized(Control);
		}

		public override void Draw(Canvas canvas)
		{
			base.Draw(canvas);
			var background = Background;

			if (background is RippleDrawable rd)
			{
				if (rd.GetDrawable(0) is BorderDrawable bd)
				{
					bd.DrawOutline(canvas, canvas.Width, canvas.Height);
				}
			}
			else if (background is BorderDrawable bd)
			{
				bd.DrawOutline(canvas, canvas.Width, canvas.Height);
			}
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = LabelFor;

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (!Enabled || (_inputTransparent && Enabled))
				return false;

			return base.OnTouchEvent(e);
		}


		void UpdatePadding()
		{
			SetPadding(
				(int)(Context.ToPixels(ImageButton.Padding.Left)),
				(int)(Context.ToPixels(ImageButton.Padding.Top)),
				(int)(Context.ToPixels(ImageButton.Padding.Right)),
				(int)(Context.ToPixels(ImageButton.Padding.Bottom))
			);
		}

		void UpdateInputTransparent()
		{
			if (ImageButton == null || _disposed)
			{
				return;
			}

			_inputTransparent = ImageButton.InputTransparent;
		}

		// Image related
		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
			{
				UpdateInputTransparent();
			}
			else if (e.PropertyName == ImageButton.PaddingProperty.PropertyName)
			{
				UpdatePadding();
			}

			ElementPropertyChanged?.Invoke(this, e);
		}


		// general state related
		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			((IElementController)ImageButton).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}
		// general state related


		// Button related
		void IOnClickListener.OnClick(AView v) =>
			ButtonElementManager.OnClick(ImageButton, ImageButton, v);

		bool IOnTouchListener.OnTouch(AView v, MotionEvent e) =>
			ButtonElementManager.OnTouch(ImageButton, ImageButton, v, e);
		// Button related


		float IBorderVisualElementRenderer.ShadowRadius => Context.ToPixels(OnThisPlatform().GetShadowRadius());
		float IBorderVisualElementRenderer.ShadowDx => Context.ToPixels(OnThisPlatform().GetShadowOffset().Width);
		float IBorderVisualElementRenderer.ShadowDy => Context.ToPixels(OnThisPlatform().GetShadowOffset().Height);
		AColor IBorderVisualElementRenderer.ShadowColor => OnThisPlatform().GetShadowColor().ToAndroid();
		bool IBorderVisualElementRenderer.IsShadowEnabled() => OnThisPlatform().GetIsShadowEnabled();
		bool IBorderVisualElementRenderer.UseDefaultPadding() => false;
		bool IBorderVisualElementRenderer.UseDefaultShadow() => false;
		VisualElement IBorderVisualElementRenderer.Element => ImageButton;
		AView IBorderVisualElementRenderer.View => this;

		IPlatformElementConfiguration<PlatformConfiguration.Android, ImageButton> OnThisPlatform()
		{
			if (_platformElementConfiguration == null)
				_platformElementConfiguration = ImageButton.OnThisPlatform();

			return _platformElementConfiguration;
		}
	}
}
