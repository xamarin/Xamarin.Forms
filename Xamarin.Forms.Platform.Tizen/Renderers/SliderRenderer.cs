using System;
using ESlider = ElmSharp.Slider;
using ESize = ElmSharp.Size;

namespace Xamarin.Forms.Platform.Tizen
{
	public class SliderRenderer : ViewRenderer<Slider, ESlider>
	{

		public SliderRenderer()
		{
			RegisterPropertyHandler(Slider.ValueProperty, UpdateValue);
			RegisterPropertyHandler(Slider.MinimumProperty, UpdateMinMax);
			RegisterPropertyHandler(Slider.MaximumProperty, UpdateMinMax);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			if (Control == null)
			{
				var slider = new ESlider(Forms.Context.MainWindow)
				{
					PropagateEvents = false,
				};
				SetNativeControl(slider);
			}

			if (e.OldElement != null)
			{
				Control.ValueChanged -= SliderValueChangedHandler;
			}

			if (e.NewElement != null)
			{
				Control.ValueChanged += SliderValueChangedHandler;
			}

			base.OnElementChanged(e);
		}

		protected override ESize Measure(int availableWidth, int availableHeight)
		{
			return new ESize(Math.Min(200, availableWidth), 50);
		}

		void SliderValueChangedHandler(object sender, EventArgs e)
		{
			Element.Value = Control.Value;
		}

		protected void UpdateValue()
		{
			Control.Value = Element.Value;
		}

		protected void UpdateMinMax()
		{
			Control.Minimum = Element.Minimum;
			Control.Maximum = Element.Maximum;
			UpdateValue();
		}
	}
}
