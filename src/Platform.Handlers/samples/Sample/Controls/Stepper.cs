using System;
using Xamarin.Forms.Internals;
using Xamarin.Platform;

namespace Sample
{
	public partial class Stepper : Xamarin.Forms.View, IStepper
	{
		int _digits = 4;

		double _increment = 1.0d;
		double _value;

		public Stepper()
		{
		}

		public Stepper(double min, double max, double val, double increment) : this()
		{
			if (min >= max)
				throw new ArgumentOutOfRangeException(nameof(min));

			if (max > Minimum)
			{
				Maximum = max;
				Minimum = min;
			}
			else
			{
				Minimum = min;
				Maximum = max;
			}

			Increment = increment;
			Value = val.Clamp(min, max);
		}

		public double Increment
		{
			get { return _increment; }
			set
			{
				_increment = value;
				UpdateDigits(_increment);
			}
		}

		public double Minimum { get; set; } = 0.0d;

		public double Maximum { get; set; } = 100.0d;

		public double Value
		{
			get { return _value; }
			set { _value = CoerceValue(value); }
		}

		double CoerceValue(double value)
		{
			return Math.Round((double)value, _digits).Clamp(Minimum, Maximum);
		}

		//'-log10(increment) + 4' as rounding digits gives us 4 significant decimal digits after the most significant one.
		//If your increment uses more than 4 significant digits, you're holding it wrong.
		void UpdateDigits(double increment)
		{
			_digits = (int)(-Math.Log10((double)increment) + 4).Clamp(1, 15);
		}
	}
}