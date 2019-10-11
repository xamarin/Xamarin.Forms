using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WSlider = System.Windows.Controls.Slider;

namespace Xamarin.Forms.Platform.WPF
{
	public class SliderRenderer : ViewRenderer<Slider, WSlider>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and subscribe to value changed event
				{
					SetNativeControl(new WSlider
					{
						Value = Element.Value,
						Minimum = Element.Minimum,
						Maximum = Element.Maximum
					});
					Control.ValueChanged += HandleValueChanged;
				}
				else
				{
					// Update control property  
					UpdateValue();
					UpdateMinimum();
					UpdateMaximum();
				} 
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Slider.MinimumProperty.PropertyName)
				UpdateMinimum();
			else if (e.PropertyName == Slider.MaximumProperty.PropertyName)
				UpdateMaximum();
			else if (e.PropertyName == Slider.ValueProperty.PropertyName)
				UpdateValue();
		}
		
		void UpdateMinimum()
		{
			Control.Minimum = Element.Minimum;
		}

		void UpdateMaximum()
		{
			Control.Maximum = Element.Maximum;
		}

		void UpdateValue()
		{
			if (Control.Value != Element.Value)
				Control.Value = Element.Value;
		}

		void HandleValueChanged(object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
		{
			((IElementController)Element).SetValueFromRenderer(Slider.ValueProperty, Control.Value);
		}

		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.ValueChanged -= HandleValueChanged;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}
	}
}