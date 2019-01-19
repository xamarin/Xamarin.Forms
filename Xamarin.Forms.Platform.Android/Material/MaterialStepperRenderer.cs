#if __ANDROID_28__
using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.FastRenderers;
using Xamarin.Forms.Platform.Android.Material;
using AButton = Android.Widget.Button;
using AView = Android.Views.View;
using MButton = Android.Support.Design.Button.MaterialButton;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Stepper), typeof(MaterialStepperRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.Android.Material
{
	public class MaterialStepperRenderer : LinearLayout,
		IStepperRenderer, IVisualElementRenderer, IViewRenderer, ITabStop
	{
		const int DefaultButtonSpacing = 4;

		int? _defaultLabelFor;

		bool _disposed;

		Stepper _element;

		VisualElementTracker _visualElementTracker;
		VisualElementRenderer _visualElementRenderer;
		MotionEventHelper _motionEventHelper;

		MButton _downButton;
		MButton _upButton;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
		public event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged;

		public MaterialStepperRenderer(Context context)
			: base(MaterialContextThemeWrapper.Create(context))
		{
			VisualElement.VerifyVisualFlagEnabled();

			_visualElementRenderer = new VisualElementRenderer(this);
			_motionEventHelper = new MotionEventHelper();

			Orientation = Orientation.Horizontal;

			StepperRendererManager.CreateStepperButtons(this, out _downButton, out _upButton);
			AddView(_downButton, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent));
			AddView(_upButton, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.MatchParent)
			{
				// because the buttons have no inset, we add spacing in the form of a margin to one button
				LeftMargin = (int)Context.ToPixels(DefaultButtonSpacing)
			});
		}

		protected LinearLayout Control => this;

		protected Stepper Element
		{
			get { return _element; }
			set
			{
				if (_element == value)
					return;

				var oldElement = _element;
				_element = value;

				OnElementChanged(new ElementChangedEventArgs<Stepper>(oldElement, _element));

				_element?.SendViewInitialized(this);

				_motionEventHelper.UpdateElement(_element);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			_disposed = true;

			if (disposing)
			{
				_visualElementTracker?.Dispose();
				_visualElementTracker = null;

				_visualElementRenderer?.Dispose();
				_visualElementRenderer = null;

				if (Element != null)
				{
					Element.PropertyChanged -= OnElementPropertyChanged;

					if (Platform.GetRenderer(Element) == this)
						Element.ClearValue(Platform.RendererProperty);
				}
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			ElementChanged?.Invoke(this, new VisualElementChangedEventArgs(e.OldElement, e.NewElement));

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (e.NewElement != null)
			{
				this.EnsureId();

				if (_visualElementTracker == null)
					_visualElementTracker = new VisualElementTracker(this);

				e.NewElement.PropertyChanged += OnElementPropertyChanged;

				StepperRendererManager.UpdateButtons(this, null, _downButton, _upButton);
				UpdateBackgroundColor();

				ElevationHelper.SetElevation(this, e.NewElement);
			}
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ElementPropertyChanged?.Invoke(this, e);

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();

			StepperRendererManager.UpdateButtons(this, e, _downButton, _upButton);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (_visualElementRenderer.OnTouchEvent(e) || base.OnTouchEvent(e))
				return true;

			return _motionEventHelper.HandleMotionEvent(Parent, e);
		}

		private void UpdateBackgroundColor()
		{
			var currentColor = Element?.BackgroundColor ?? Color.Default;
			SetBackgroundColor(currentColor.ToAndroid());
		}

		// IStepperRenderer

		Stepper IStepperRenderer.Element => Element;

		AButton IStepperRenderer.GetButton(bool upButton) => upButton ? _upButton : _downButton;

		AButton IStepperRenderer.CreateButton(bool isUpButton)
		{
			var button = new MButton(MaterialContextThemeWrapper.Create(Context), null, Resource.Attribute.materialOutlinedButtonStyle);

			// the buttons are meant to be "square", but are usually wide,
			// so, copy the vertical properties into the horizontal properties
			button.SetMinimumWidth(button.MinimumHeight);
			button.SetMinWidth(button.MinHeight);
			button.SetPadding(button.PaddingTop, button.PaddingTop, button.PaddingBottom, button.PaddingBottom);

			return button;
		}

		// IVisualElementRenderer

		VisualElement IVisualElementRenderer.Element => Element;

		VisualElementTracker IVisualElementRenderer.Tracker => _visualElementTracker;

		ViewGroup IVisualElementRenderer.ViewGroup => null;

		AView IVisualElementRenderer.View => this;

		SizeRequest IVisualElementRenderer.GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			Measure(widthConstraint, heightConstraint);
			return new SizeRequest(new Size(Control.MeasuredWidth, Control.MeasuredHeight), new Size());
		}

		void IVisualElementRenderer.SetElement(VisualElement element) =>
			Element = (element as Stepper) ?? throw new ArgumentException($"Element must be of type {nameof(Stepper)}.");

		void IVisualElementRenderer.SetLabelFor(int? id)
		{
			if (_defaultLabelFor == null)
				_defaultLabelFor = ViewCompat.GetLabelFor(this);

			ViewCompat.SetLabelFor(this, (int)(id ?? _defaultLabelFor));
		}

		void IVisualElementRenderer.UpdateLayout() =>
			_visualElementTracker?.UpdateLayout();

		// IViewRenderer

		void IViewRenderer.MeasureExactly() =>
			ViewRenderer.MeasureExactly(this, Element, Context);

		// ITabStop

		AView ITabStop.TabStop => this;
	}
}
#endif
