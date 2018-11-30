using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using MCard = MaterialComponents.Card;

// this won't go here permanently it's just for testing at this point
[assembly: ExportRenderer(typeof(Xamarin.Forms.Frame), typeof(Xamarin.Forms.Platform.iOS.Material.MaterialFrameRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialFrameRenderer : MCard, IVisualElementRenderer
	{
		double _defaultElevation = -1f;
		nfloat _defaultCornerRadius = -1f;
		nfloat _defaultStrokeWidth = -1f;
		UIColor _defaultBackgroundColor;
		UIColor _defaultStrokeColor;

		VisualElementPackager _packager;
		VisualElementTracker _tracker;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public MaterialFrameRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();

			Interactable = false;
		}

		public Frame Element { get; private set; }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_packager == null)
					return;

				SetElement(null);

				_packager.Dispose();
				_packager = null;

				_tracker.NativeControlUpdated -= OnNativeControlUpdated;
				_tracker.Dispose();
				_tracker = null;
			}

			base.Dispose(disposing);
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;

			var frame = element as Frame;
			if (frame == null)
				throw new ArgumentException("Element must be of type Frame");

			Element = frame;

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
			}

			if (element != null)
			{
				if (_packager == null)
				{
					_packager = new VisualElementPackager(this);
					_packager.Load();

					_tracker = new VisualElementTracker(this);
					_tracker.NativeControlUpdated += OnNativeControlUpdated;
				}

				element.PropertyChanged += OnElementPropertyChanged;

				UpdateShadow();
				UpdateCornerRadius();
				UpdateBorderColor();
				UpdateBackgroundColor();
			}

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName)
				UpdateShadow();
			else if (e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName)
				UpdateCornerRadius();
			else if (e.PropertyName == Xamarin.Forms.Frame.BorderColorProperty.PropertyName)
				UpdateBorderColor();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			ElementChanged?.Invoke(this, e);
		}

		void OnNativeControlUpdated(object sender, EventArgs eventArgs)
		{
		}

		void UpdateShadow()
		{
			// set the default elevation on the first time
			if (_defaultElevation < 0)
				_defaultElevation = GetShadowElevation(UIControlState.Normal);

			if (Element.HasShadow)
				SetShadowElevation(_defaultElevation, UIControlState.Normal);
			else
				SetShadowElevation(0, UIControlState.Normal);
		}

		void UpdateCornerRadius()
		{
			// set the default radius on the first time
			if (_defaultCornerRadius < 0)
				_defaultCornerRadius = CornerRadius;

			var cornerRadius = Element.CornerRadius;
			if (cornerRadius < 0)
				CornerRadius = _defaultCornerRadius;
			else
				CornerRadius = cornerRadius;
		}

		void UpdateBorderColor()
		{
			// set the default stroke properties on the first time
			if (_defaultStrokeColor == null)
				_defaultStrokeColor = GetBorderColorForState(UIControlState.Normal);
			if (_defaultStrokeWidth < 0)
				_defaultStrokeWidth = GetBorderWidth(UIControlState.Normal);

			var borderColor = Element.BorderColor;
			if (borderColor.IsDefault)
			{
				SetBorderColor(_defaultStrokeColor, UIControlState.Normal);
				SetBorderWidth(_defaultStrokeWidth, UIControlState.Normal);
			}
			else
			{
				SetBorderColor(borderColor.ToUIColor(), UIControlState.Normal);
				SetBorderWidth(1f, UIControlState.Normal);
			}
		}

		void UpdateBackgroundColor()
		{
			if (_defaultBackgroundColor == null)
				_defaultBackgroundColor = BackgroundColor;

			var bgColor = Element.BackgroundColor;
			if (bgColor.IsDefault)
				BackgroundColor = _defaultBackgroundColor;
			else
				BackgroundColor = bgColor.ToUIColor();
		}

		// IVisualElementRenderer

		VisualElement IVisualElementRenderer.Element => Element;

		UIView IVisualElementRenderer.NativeView => this;

		UIViewController IVisualElementRenderer.ViewController => null;

		SizeRequest IVisualElementRenderer.GetDesiredSize(double widthConstraint, double heightConstraint) =>
			this.GetSizeRequest(widthConstraint, heightConstraint, 44, 44);

		void IVisualElementRenderer.SetElement(VisualElement element) =>
			SetElement(element);

		void IVisualElementRenderer.SetElementSize(Size size) =>
			Layout.LayoutChildIntoBoundingRegion(Element, new Rectangle(Element.X, Element.Y, size.Width, size.Height));
	}
}