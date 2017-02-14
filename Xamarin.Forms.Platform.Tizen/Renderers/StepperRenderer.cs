using System;
using ElmSharp;

namespace Xamarin.Forms.Platform.Tizen
{
	public class StepperRenderer : ViewRenderer<Stepper, Spinner>
	{

		public StepperRenderer()
		{
			RegisterPropertyHandler(Stepper.ValueProperty, UpdateValue);
			RegisterPropertyHandler(Stepper.MinimumProperty, UpdateMinMax);
			RegisterPropertyHandler(Stepper.MaximumProperty, UpdateMinMax);
			RegisterPropertyHandler(Stepper.IncrementProperty, UpdateStep);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Stepper> e)
		{
			if (Control == null)
			{
				var stepper = new Spinner(Forms.Context.MainWindow)
				{
					IsEditable = false,
				};

				SetNativeControl(stepper);
			}

			if (e.OldElement != null)
			{
				Control.ValueChanged -= StepperValueChangedHandler;
			}

			if (e.NewElement != null)
			{
				Control.ValueChanged += StepperValueChangedHandler;
			}

			base.OnElementChanged(e);
		}

		void StepperValueChangedHandler(object sender, EventArgs e)
		{
			double newValue = Control.Value;
			((IElementController)Element).SetValueFromRenderer(Stepper.ValueProperty, newValue);

			// Determines how many decimal places are there in current Stepper's value.
			// The 15 pound characters below correspond to the maximum precision of Double type.
			var decimalValue = Decimal.Parse(newValue.ToString("0.###############"));

			// GetBits() method returns an array of four 32-bit integer values.
			// The third (0-indexing) element of an array contains the following information:
			//     bits 00-15: unused, required to be 0
			//     bits 16-23: an exponent between 0 and 28 indicating the power of 10 to divide the integer number passed as a parameter.
			//                 Conversely this is the number of decimal digits in the number as well.
			//     bits 24-30: unused, required to be 0
			//     bit     31: indicates the sign. 0 means positive number, 1 is for negative numbers.
			//
			// The precision information needs to be extracted from bits 16-23 of third element of an array
			// returned by GetBits() call. Right-shifting by 16 bits followed by zeroing anything else results
			// in a nice conversion of this data to integer variable.
			var precision = (Decimal.GetBits(decimalValue)[3] >> 16) & 0x000000FF;

			// Sets Stepper's inner label decimal format to use exactly as many decimal places as needed:
			Control.LabelFormat = string.Format("%.{0}f", precision);
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

		void UpdateStep()
		{
			Control.Step = Element.Increment;
		}
	}
}

