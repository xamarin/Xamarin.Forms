using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
				_defaultOffColor = Forms.IsiOS13OrNewer
					? Control.Subviews?.FirstOrDefault().Subviews?.FirstOrDefault()?.BackgroundColor
					: UISwitch.Appearance.TintColor;
				_defaultThumbColor = UISwitch.Appearance.ThumbTintColor;
				Control.On = Element.IsToggled;
				e.NewElement.Toggled += OnElementToggled;
				UpdateOnColor();
				UpdateOffColor();
				UpdateThumbColor();
			}

			base.OnElementChanged(e);
		}

		void UpdateOnColor()
		{
			if (IsElementOrControlEmpty)
				return;

			Control.OnTintColor = Element.OnColor != Color.Default
				? Element.OnColor.ToUIColor()
				: _defaultOnColor;
		}

		void UpdateOffColor()
		{
			if (IsElementOrControlEmpty)
				return;

			if (!Forms.IsiOS13OrNewer)
			{
				HandleOffColorForOlderSystemVersions(Element.BackgroundColor);
				return;
			}

			var offColorView = Control.Subviews.FirstOrDefault()?.Subviews.FirstOrDefault();
			if (offColorView == null)
				return;
			
			offColorView.BackgroundColor = Element.OffColor != Color.Default
				? Element.OffColor.ToUIColor()
				: _defaultOffColor;
			
		}

		void UpdateThumbColor()
		{
			if (IsElementOrControlEmpty)
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
		{
			if (IsElementOrControlEmpty)
				return;

			if (!Forms.IsiOS13OrNewer)
			{
				HandleOffColorForOlderSystemVersions(Element.BackgroundColor);
				return;
			}

			base.SetBackgroundColor(color);
		}

		void HandleOffColorForOlderSystemVersions(Color backgroundColor)
		{
			if (Element.OffColor == Color.Default || backgroundColor != Color.Default)
			{
				Control.TintColor = _defaultOffColor;
				Control.Layer.CornerRadius = 0;
				base.SetBackgroundColor(backgroundColor);
				return;
			}
			var offColor = Element.OffColor.ToUIColor();
			Control.TintColor = offColor;
			Control.BackgroundColor = offColor;
			Control.Layer.CornerRadius = Control.Bounds.Height / 2;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Switch.OnColorProperty.PropertyName)
				UpdateOnColor();
			else if (e.PropertyName == Switch.OffColorProperty.PropertyName)
				UpdateOffColor();
			else if (e.PropertyName == Switch.ThumbColorProperty.PropertyName)
				UpdateThumbColor();
		}
	}
}