using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using Android.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android.FastRenderers;
using AView = Android.Views.View;
using AFloatingButton = global::Android.Support.Design.Widget.FloatingActionButton;

namespace Xamarin.Forms.Platform.Android
{
	public class FloatingActionButtonRenderer : AFloatingButton,
		IVisualElementRenderer, IViewRenderer, ITabStop,
		AView.IOnFocusChangeListener, AView.IOnClickListener, AView.IOnTouchListener, AView.IOnAttachStateChangeListener
	{
		const float SmallSize = 44f;
		const float NormalSize = 56f;

		int? _defaultLabelFor;
		bool _isDisposed;
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

		void IOnFocusChangeListener.OnFocusChange(AView v, bool hasFocus)
		{
			((IElementController)Button).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, hasFocus);
		}

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			var size = MinimumSize();
			Measure((int)size.Width, (int)size.Height);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), size);
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
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;

			if (disposing)
			{
				SetOnClickListener(null);
				SetOnTouchListener(null);
				RemoveOnAttachStateChangeListener(this);

				_automationPropertiesProvider?.Dispose();
				_tracker?.Dispose();
				_visualElementRenderer?.Dispose();

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

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

		Size MinimumSize()
		{
			var size = Element.Size == FloatingActionButtonSize.Mini ? SmallSize : NormalSize;
			return new Xamarin.Forms.Size(size, size);
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<FloatingActionButton> e)
		{
			if (e.NewElement != null && !_isDisposed)
			{
				this.EnsureId();

				UpdateInputTransparent();
				UpdateColor();
				UpdateEnabled();
				TryUpdateBitmap();

				//ElevationHelper.SetElevation(this, e.NewElement);
			}

			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
			{
				UpdateInputTransparent();
			}
			else if (e.PropertyName == Image.SourceProperty.PropertyName)
			{
				TryUpdateBitmap();
			}
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				UpdateColor();
			}
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateEnabled();
			}
			else if (e.PropertyName == FloatingActionButton.SizeProperty.PropertyName)
			{
				_tracker?.UpdateLayout();
				Size = Element.Size == FloatingActionButtonSize.Mini ? SizeMini : SizeNormal;
			}

			ElementPropertyChanged?.Invoke(this, e);
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			var size = Element.Size == FloatingActionButtonSize.Mini ? SmallSize : NormalSize;
			size = Context.ToPixels(size);

			base.OnLayout(changed, l, t, l + (int)size, t + (int)size);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
		}

		void SetTracker(VisualElementTracker tracker)
		{
			_tracker = tracker;
		}

		internal void OnNativeFocusChanged(bool hasFocus)
		{
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

			Tag = this;
		}

		void UpdateInputTransparent()
		{
			if (Element == null || _isDisposed)
			{
				return;
			}

			_inputTransparent = Element.InputTransparent;
		}

		void TryUpdateBitmap()
		{
			try
			{
				UpdateBitmap();
			}
			catch (Exception ex)
			{
				Internals.Log.Warning(nameof(FloatingActionButtonRenderer), "Error loading image: {0}", ex);
			}
			finally
			{
			}
		}

		void UpdateBitmap()
		{
			if (Element == null || Control == null || Control.IsDisposed())
			{
				return;
			}

			if (Control == null || Control.IsDisposed())
				return;

			if (Device.IsInvokeRequired)
				throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

			var source = Element.ImageSource;

			Bitmap bitmap = null;
			Drawable drawable = null;

			IImageSourceHandler handler;

			if (source != null && (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				if (handler is FileImageSourceHandler)
				{
					drawable = Control.Context.GetDrawable((FileImageSource)source);
				}

				if (drawable == null)
				{
					try
					{
						bitmap = handler.LoadImageAsync(source, Control.Context).Result;
					}
					catch (TaskCanceledException)
					{
					}
				}
			}

			if (!Control.IsDisposed())
			{
				if (bitmap == null && drawable != null)
				{
					Control.SetImageDrawable(drawable);
				}
				else
				{
					Control.SetImageBitmap(bitmap);
				}
			}

			bitmap?.Dispose();
		}

		void UpdateColor()
		{
			Control.BackgroundTintList = ColorStateList.ValueOf(Element.BackgroundColor.ToAndroid());
		}

		void UpdateEnabled()
		{
			Control.Enabled = Element.IsEnabled;
		}

		IPlatformElementConfiguration<PlatformConfiguration.Android, FloatingActionButton> OnThisPlatform()
		{
			if (_platformElementConfiguration == null)
				_platformElementConfiguration = Button.OnThisPlatform();

			return _platformElementConfiguration;
		}

		void IOnAttachStateChangeListener.OnViewAttachedToWindow(global::Android.Views.View attachedView)
		{
			TryUpdateBitmap();
		}

		void IOnAttachStateChangeListener.OnViewDetachedFromWindow(global::Android.Views.View detachedView)
		{
		}
	}
}