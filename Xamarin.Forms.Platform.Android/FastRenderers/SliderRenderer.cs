using System;
using System.ComponentModel;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android.FastRenderers
{
	public class SliderRenderer : SeekBar, IVisualElementRenderer, IEffectControlProvider,
		SeekBar.IOnSeekBarChangeListener
	{
		int? _defaultLabelFor;
		bool _disposed;
		Slider _element;
		double _max, _min;
		bool _progressChangedOnce;

		readonly AutomationPropertiesProvider _automationPropertiesProvider;
		readonly EffectControlProvider _effectControlProvider;
		VisualElementTracker _visualElementTracker;

		public SliderRenderer() : base(Forms.Context)
		{
			_automationPropertiesProvider = new AutomationPropertiesProvider(this);
			_effectControlProvider = new EffectControlProvider(this);
		}

		protected Slider Element
		{
			get { return _element; }
			set
			{
				if (_element == value)
				{
					return;
				}

				Slider oldElement = _element;
				_element = value;

				OnElementChanged(new ElementChangedEventArgs<Slider>(oldElement, _element));

				_element?.SendViewInitialized(this);
			}
		}

		double Value
		{
			get { return _min + (_max - _min) * (Progress / 1000.0); }
			set { Progress = (int)((value - _min) / (_max - _min) * 1000.0); }
		}

		void IOnSeekBarChangeListener.OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
		{
			if (!_progressChangedOnce)
			{
				_progressChangedOnce = true;
				return;
			}

			((IElementController)Element).SetValueFromRenderer(Slider.ValueProperty, Value);
		}

		void IOnSeekBarChangeListener.OnStartTrackingTouch(SeekBar seekBar)
		{
		}

		void IOnSeekBarChangeListener.OnStopTrackingTouch(SeekBar seekBar)
		{
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			_effectControlProvider.RegisterEffect(effect);
		}

		VisualElement IVisualElementRenderer.Element => Element;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			if (_disposed)
			{
				return new SizeRequest();
			}

			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(MeasuredWidth, MeasuredHeight), new Size());
		}

		void IVisualElementRenderer.SetElement(VisualElement element)
		{
			var slider = element as Slider;
			if (slider == null)
			{
				throw new ArgumentException("Element must be of type Slider");
			}

			Element = slider;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (Element.InputTransparent)
			{
				return false;
			}

			return base.OnTouchEvent(e);
		}

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
			{
				_defaultLabelFor = LabelFor;
			}

			LabelFor = (int)(id ?? _defaultLabelFor);
		}

		VisualElementTracker IVisualElementRenderer.Tracker => _visualElementTracker;

		void IVisualElementRenderer.UpdateLayout()
		{
			VisualElementTracker tracker = _visualElementTracker;
			tracker?.UpdateLayout();
		}

		global::Android.Views.View IVisualElementRenderer.View => this;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				if (_visualElementTracker != null)
				{
					_visualElementTracker.Dispose();
					_visualElementTracker = null;
				}

				_automationPropertiesProvider?.Dispose();
				
				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;
				}
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.OldElement == null)
			{
				Max = 1000;
				SetOnSeekBarChangeListener(this);
			}

			if (e.NewElement != null)
			{
				this.EnsureId();

				if (_visualElementTracker == null)
				{
					_visualElementTracker = new VisualElementTracker(this);
				}

				Slider slider = e.NewElement;
				slider.PropertyChanged += OnElementPropertyChanged;
				_min = slider.Minimum;
				_max = slider.Maximum;
				Value = slider.Value;
			}
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ElementPropertyChanged?.Invoke(this, e);

			Slider slider = Element;

			if (e.PropertyName == Slider.MaximumProperty.PropertyName)
			{
				_max = slider.Maximum;
			}
			else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
			{
				_min = slider.Minimum;
			}
			else if (e.PropertyName == Slider.ValueProperty.PropertyName)
			{
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (Value != slider.Value)
				{
					Value = slider.Value;
				}
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			BuildVersionCodes androidVersion = Build.VERSION.SdkInt;
			if (androidVersion < BuildVersionCodes.JellyBean)
			{
				return;
			}

			// Thumb only supported JellyBean and higher
			Drawable thumb = Thumb;
			int thumbTop = Height / 2 - thumb.IntrinsicHeight / 2;

			thumb.SetBounds(thumb.Bounds.Left, thumbTop, thumb.Bounds.Left + thumb.IntrinsicWidth,
				thumbTop + thumb.IntrinsicHeight);
		}
	}
}