using Android.Content;
using Android.Content.Res;
using Android.Support.V4.View;
using Android.Views;
using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using AView = Android.Views.View;
using AFloatingButton = global::Android.Support.Design.Widget.FloatingActionButton;
using Xamarin.Forms.Platform.Android.FastRenderers;
using Android.Graphics.Drawables;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class FloatingActionButtonRenderer : AFloatingButton,
		IVisualElementRenderer, IViewRenderer, ITabStop,
		AView.IOnAttachStateChangeListener, AView.IOnFocusChangeListener, AView.IOnClickListener, AView.IOnTouchListener
	{
		ColorStateList _defaultBackgroundTintList;
		int? _defaultLabelFor;
		bool _disposed;
		bool _inputTransparent;
		AutomationPropertiesProvider _automationPropertiesProvider;
		VisualElementTracker _tracker;
		VisualElementRenderer _visualElementRenderer;
		IPlatformElementConfiguration<PlatformConfiguration.Android, FloatingActionButton> _platformElementConfiguration;
		FloatingActionButton _button;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public FloatingActionButtonRenderer(Context context) : base(context)
		{
			Initialize();
		}

		protected FloatingActionButton Element => Button;
		protected AFloatingButton Control => this;

		VisualElement IVisualElementRenderer.Element => Element;
		AView IVisualElementRenderer.View => this;
		ViewGroup IVisualElementRenderer.ViewGroup => null;
		VisualElementTracker IVisualElementRenderer.Tracker => _tracker;

		FloatingActionButton Button
		{
			get => _button;
			set
			{
				_button = value;
				_platformElementConfiguration = null;
			}
		}

		AView ITabStop.TabStop => this;

		void IOnClickListener.OnClick(AView v) => ButtonElementManager.OnClick(Button, Button, v);

		bool IOnTouchListener.OnTouch(AView v, MotionEvent e) => ButtonElementManager.OnTouch(Button, Button, v, e);

		void IOnAttachStateChangeListener.OnViewAttachedToWindow(AView attachedView) =>
			UpdateAll();

		void IOnAttachStateChangeListener.OnViewDetachedFromWindow(AView detachedView) { }

		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			((IElementController)Button).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
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

			if (!(element is FloatingActionButton))
			{
				throw new ArgumentException($"{nameof(element)} must be of type {nameof(FloatingActionButton)}");
			}

			VisualElement oldElement = Button;
			Button = (FloatingActionButton)element;

			Performance.Start(out string reference);

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			element.PropertyChanged += OnElementPropertyChanged;

			if (_tracker == null)
			{
				// Can't set up the tracker in the constructor because it access the Element (for now)
				SetTracker(new VisualElementTracker(this));
			}

			if (_visualElementRenderer == null)
			{
				_visualElementRenderer = new VisualElementRenderer(this);
			}

			OnElementChanged(new ElementChangedEventArgs<FloatingActionButton>(oldElement as FloatingActionButton, Button));

			SendVisualElementInitialized(element, this);

			Performance.Stop(reference);
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = ViewCompat.GetLabelFor(this);
			}

			ViewCompat.SetLabelFor(this, (int)(id ?? _defaultLabelFor));
		}

		void IVisualElementRenderer.UpdateLayout() => _tracker?.UpdateLayout();

		void IViewRenderer.MeasureExactly()
		{
			ViewRenderer.MeasureExactly(this, Element, Context);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				SetOnClickListener(null);
				SetOnTouchListener(null);
				RemoveOnAttachStateChangeListener(this);
				OnFocusChangeListener = null;

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;
				}

				_automationPropertiesProvider?.Dispose();
				_tracker?.Dispose();
				_visualElementRenderer?.Dispose();

				_defaultBackgroundTintList = null;

				if (Element != null)
				{
					if (Platform.GetRenderer(Element) == this)
						Element.ClearValue(Platform.RendererProperty);
				}
			}

			base.Dispose(disposing);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (!Enabled || (_inputTransparent && Enabled))
				return false;

			return base.OnTouchEvent(e);
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
		{
			if (e.NewElement != null && !_disposed)
			{
				this.EnsureId();

				UpdateColor();
				UpdateSize();
				UpdateInputTransparent();
				UpdateImage();

				ElevationHelper.SetElevation(this, e.NewElement);
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.Is(VisualElement.BackgroundColorProperty))
			{
				UpdateColor();
			}
			else if (e.Is(VisualElement.InputTransparentProperty))
			{
				UpdateInputTransparent();
			}
			else if (e.Is(FloatingActionButton.ImageSourceProperty))
			{
				UpdateImage();
			}
			else if (e.Is(FloatingActionButton.SizeProperty))
			{
				UpdateSize();
			}

			ElementPropertyChanged?.Invoke(this, e);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var size = (float)Element.Size;
			size = Context.ToPixels(size);

			base.OnLayout(changed, l, t, l + (int)size, t + (int)size);
		}

		void SetTracker(VisualElementTracker tracker)
		{
			_tracker = tracker;
		}

		void UpdateAll()
		{
			UpdateColor();
			UpdateImage();
			UpdateSize();
		}

		void UpdateColor()
		{
			if (_defaultBackgroundTintList == null)
				_defaultBackgroundTintList = BackgroundTintList;

			if (!Element.IsSet(VisualElement.BackgroundColorProperty) || Element.BackgroundColor == Color.Default)
				BackgroundTintList = _defaultBackgroundTintList;
			else
				BackgroundTintList = ColorStateList.ValueOf(Element.BackgroundColor.ToAndroid());
		}

		void UpdateImage()
		{
			if (Element == null)
				return;

			ImageSource elementImage = Element.ImageSource;

			if (elementImage == null || elementImage.IsEmpty)
			{
				SetImageDrawable(null);
				return;
			}

			Drawable existingImage = Drawable;

			if (this is IVisualElementRenderer visualElementRenderer)
			{
				visualElementRenderer.ApplyDrawableAsync(FloatingActionButton.ImageSourceProperty, Context, image =>
				{
					if (image == existingImage)
						return;

					SetImageDrawable(image);
				});
			}
		}

		void UpdateInputTransparent()
		{
			if (Element == null || _disposed)
			{
				return;
			}

			_inputTransparent = Element.InputTransparent;
		}

		void UpdateSize()
		{
			Size = Element.Size == FloatingActionButtonSize.Mini ? SizeMini : SizeNormal;
			_tracker?.UpdateLayout();
		}

		internal void SendVisualElementInitialized(VisualElement element, AView nativeView)
		{
			element.SendViewInitialized(nativeView);
		}

		void Initialize()
		{
			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			SoundEffectsEnabled = false;
			SetOnClickListener(this);
			SetOnTouchListener(this);
			AddOnAttachStateChangeListener(this);
			OnFocusChangeListener = this;

			LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
			{
				Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal
			};

			Tag = this;
		}

		IPlatformElementConfiguration<PlatformConfiguration.Android, FloatingActionButton> OnThisPlatform()
		{
			if (_platformElementConfiguration == null)
				_platformElementConfiguration = Button.OnThisPlatform();

			return _platformElementConfiguration;
		}
	}
}
