using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Platform;

namespace Sample
{
	public class Slider : View, ISlider
	{
		double _value;

		public Slider()
		{

		}

		public Slider(double min, double max, double val)
		{
			if (min >= max)
				throw new ArgumentOutOfRangeException("min");

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

			Value = Clamp(val, min, max);
		}

		public double Minimum { get; set; }
		public double Maximum { get; set; } = 1d;

		public double Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (_value == value)
					return;

				_value = value;
				ValueChanged?.Invoke(value);
			}
		}

		public Color MinimumTrackColor { get; set; }
		public Color MaximumTrackColor { get; set; }
		public Color ThumbColor { get; set; }

		public Action<double> ValueChanged { get; set; }
		public Action DragStarted { get; set; }
		public Action DragCompleted { get; set; }

		public ICommand DragStartedCommand { get; set; }
		public ICommand DragCompletedCommand { get; set; }

		void ISlider.DragStarted()
		{
			if (IsEnabled)
			{
				DragStartedCommand?.Execute(null);
				DragStarted?.Invoke();
			}
		}

		void ISlider.DragCompleted()
		{
			if (IsEnabled)
			{
				DragCompletedCommand?.Execute(null);
				DragCompleted?.Invoke();
			}
		}

		public static double Clamp(double self, double min, double max)
		{
			if (max < min)
			{
				return max;
			}
			else if (self < min)
			{
				return min;
			}
			else if (self > max)
			{
				return max;
			}

			return self;
		}

		public static int Clamp(int self, int min, int max)
		{
			if (max < min)
			{
				return max;
			}
			else if (self < min)
			{
				return min;
			}
			else if (self > max)
			{
				return max;
			}

			return self;
		}
	}
}