using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class SwitchRenderer : ViewRenderer<Switch, UISwitch>
	{
		UIColor _defaultOnColor;
		UIColor _defaultOffColor;
		UIColor _defaultThumbColor;

		[Internals.Preserve(Conditional = true)]
		public SwitchRenderer()
		{

		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Control.ValueChanged -= OnControlValueChanged;

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			if (e.OldElement != null)
				e.OldElement.Toggled -= OnElementToggled;

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new UISwitch(RectangleF.Empty));
					Control.ValueChanged += OnControlValueChanged;
				}

				_defaultOnColor = UISwitch.Appearance.OnTintColor;
				_defaultOffColor = UISwitch.Appearance.TintColor;
				_defaultThumbColor = UISwitch.Appearance.ThumbTintColor;
				Control.On = Element.IsToggled;
				e.NewElement.Toggled += OnElementToggled;
				UpdateOnColor();
				UpdateOffColor(Element.BackgroundColor);
				UpdateThumbColor();
			}

			base.OnElementChanged(e);
		}

		void UpdateOnColor()
		{
			if (Element == null)
				return;
			Control.OnTintColor = Element.OnColor != Color.Default
				? Element.OnColor.ToUIColor()
				: _defaultOnColor;
		}

		void UpdateOffColor(Color backgroundColor)
		{
			if (Element == null)
				return;
			if (Element.OffColor == Color.Default || backgroundColor != Color.Default)
			{
				Control.TintColor = _defaultOffColor;
				Control.Layer.CornerRadius = 0;
				base.SetBackgroundColor(backgroundColor);
				return;
			}
			var color = Element.OffColor.ToUIColor();
			Control.TintColor = color;
			Control.BackgroundColor = color;
			Control.Layer.CornerRadius = Control.Bounds.Height / 2;
		}

		void UpdateThumbColor()
		{
			if (Element == null)
				return;

			Color thumbColor = Element.ThumbColor;
			Control.ThumbTintColor = thumbColor.IsDefault ? _defaultThumbColor : thumbColor.ToUIColor();
		}

		void OnControlValueChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Switch.IsToggledProperty, Control.On);
		}

		void OnElementToggled(object sender, EventArgs e)
		{
			Control.SetState(Element.IsToggled, true);
		}

		protected override void SetBackgroundColor(Color color)
			=> UpdateOffColor(color);

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Switch.OnColorProperty.PropertyName)
				UpdateOnColor();
			else if (e.PropertyName == Switch.OffColorProperty.PropertyName)
				UpdateOffColor(Element.BackgroundColor);
			else if (e.PropertyName == Switch.ThumbColorProperty.PropertyName)
				UpdateThumbColor();
		}
	}
}