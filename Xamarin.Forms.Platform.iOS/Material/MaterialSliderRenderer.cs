using System;
using System.ComponentModel;
using CoreGraphics;
using MaterialComponents;
using UIKit;
using Xamarin.Forms;
using MSlider = MaterialComponents.Slider;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Slider), typeof(Xamarin.Forms.Platform.iOS.Material.MaterialSliderRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialSliderRenderer : ViewRenderer<Slider, MSlider>
	{
		UIColor _defaultMinimumTrackColor;
		UIColor _defaultMaximumTrackColor;
		UIColor _defaultThumbColor;

		public MaterialSliderRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				Control.Delegate = null;
				Control.ValueChanged -= OnControlValueChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());

					Control.Continuous = true;
					Control.ValueChanged += OnControlValueChanged;
				}

				UpdateMaximum();
				UpdateMinimum();
				UpdateValue();
				UpdateSliderColors();
			}
		}

		protected virtual IColorScheming CreateColorScheme()
		{
			return MaterialColors.Light.CreateColorScheme();
		}

		protected override MSlider CreateNativeControl()
		{
			var slider = new MSlider { StatefulApiEnabled = true };
			SliderColorThemer.ApplySemanticColorScheme(CreateColorScheme(), slider);
			return slider;
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			var height = result.Height;
			if (height == 0)
				height = nfloat.IsInfinity(size.Height) ? 12 : size.Height;

			return new CGSize(12, height);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Slider.MaximumProperty.PropertyName)
				UpdateMaximum();
			else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
				UpdateMinimum();
			else if (e.PropertyName == Slider.ValueProperty.PropertyName)
				UpdateValue();
			else if (e.PropertyName == Slider.MinimumTrackColorProperty.PropertyName)
				UpdateMinimumTrackColor();
			else if (e.PropertyName == Slider.MaximumTrackColorProperty.PropertyName)
				UpdateMaximumTrackColor();
			else if (e.PropertyName == Slider.ThumbColorProperty.PropertyName)
				UpdateThumbColor();
		}

		void UpdateMaximum()
		{
			Control.MaximumValue = (nfloat)Element.Maximum;
		}

		void UpdateMinimum()
		{
			Control.MinimumValue = (nfloat)Element.Minimum;
		}

		void UpdateValue()
		{
			nfloat value = (nfloat)Element.Value;
			if (value != Control.Value)
				Control.Value = value;
		}

		void UpdateSliderColors()
		{
			UpdateMinimumTrackColor();
			UpdateMaximumTrackColor();
			UpdateThumbColor();
		}

		void UpdateMinimumTrackColor()
		{
			Color color = Element.MinimumTrackColor;
			if (color.IsDefault && _defaultMinimumTrackColor == null)
				return;

			if (_defaultMinimumTrackColor == null)
				_defaultMinimumTrackColor = Control.GetTrackFillColor(UIControlState.Normal);

			if (color.IsDefault)
				Control.SetTrackFillColor(_defaultMinimumTrackColor, UIControlState.Normal);
			else
				Control.SetTrackFillColor(color.ToUIColor(), UIControlState.Normal);
		}

		void UpdateMaximumTrackColor()
		{
			Color color = Element.MaximumTrackColor;
			if (color.IsDefault && _defaultMaximumTrackColor == null)
				return;

			if (_defaultMaximumTrackColor == null)
				_defaultMaximumTrackColor = Control.GetTrackBackgroundColor(UIControlState.Normal);

			if (color.IsDefault)
				Control.SetTrackBackgroundColor(_defaultMaximumTrackColor, UIControlState.Normal);
			else
				Control.SetTrackBackgroundColor(color.ToUIColor(), UIControlState.Normal);
		}

		void UpdateThumbColor()
		{
			Color color = Element.ThumbColor;
			if (color.IsDefault && _defaultThumbColor == null)
				return;

			if (_defaultThumbColor == null)
				_defaultThumbColor = Control.GetThumbColor(UIControlState.Normal);

			if (color.IsDefault)
				Control.SetThumbColor(_defaultThumbColor, UIControlState.Normal);
			else
				Control.SetThumbColor(color.ToUIColor(), UIControlState.Normal);
		}

		void OnControlValueChanged(object sender, EventArgs eventArgs)
		{
			Element.SetValueFromRenderer(Slider.ValueProperty, Control.Value);
		}
	}
}