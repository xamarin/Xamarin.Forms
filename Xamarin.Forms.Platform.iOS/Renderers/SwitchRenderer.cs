using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class SwitchRenderer : ViewRenderer<Switch, UISwitch>
	{
		UIColor _defaultOnColor;
		UIColor _defaultThumbColor;

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
				_defaultThumbColor = UISwitch.Appearance.ThumbTintColor;
				Control.On = Element.IsToggled;
				e.NewElement.Toggled += OnElementToggled;
				UpdateOnColor();
				UpdateThumbColor();
				UpdateIsReadonly();
			}

			base.OnElementChanged(e);
		}

		void UpdateOnColor()
		{
			if (Element != null)
			{
				if (Element.OnColor == Color.Default)
					Control.OnTintColor = _defaultOnColor;
				else
					Control.OnTintColor = Element.OnColor.ToUIColor();
			}
		}

		void UpdateThumbColor()
		{
			if (Element == null)
				return;

			Color thumbColor = Element.ThumbColor;
			Control.ThumbTintColor = thumbColor.IsDefault ? _defaultThumbColor : thumbColor.ToUIColor();
		}

		void UpdateIsReadonly()
		{
			if (Element == null)
				return;

			Control.UserInteractionEnabled = !Element.IsReadOnly;
		}

		void OnControlValueChanged(object sender, EventArgs e)
		{
			((IElementController)Element).SetValueFromRenderer(Switch.IsToggledProperty, Control.On);
		}

		void OnElementToggled(object sender, EventArgs e)
		{
			Control.SetState(Element.IsToggled, true);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Switch.OnColorProperty.PropertyName)
				UpdateOnColor();
			if (e.PropertyName == Switch.ThumbColorProperty.PropertyName)
				UpdateThumbColor();
			if (e.PropertyName == Switch.IsReadOnlyProperty.PropertyName)
				UpdateIsReadonly();
		}
	}
}