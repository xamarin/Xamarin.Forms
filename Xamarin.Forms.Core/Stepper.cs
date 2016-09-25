using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_StepperRenderer))]
	public class Stepper : View, IElementConfiguration<Stepper>
	{
		public static readonly BindableProperty MaximumProperty = BindableProperty.Create("Maximum", typeof(double), typeof(Stepper), 100.0, validateValue: (bindable, value) =>
		{
			var stepper = (Stepper)bindable;
			return (double)value > stepper.Minimum;
		}, coerceValue: (bindable, value) =>
		{
			var stepper = (Stepper)bindable;
			stepper.Value = stepper.Value.Clamp(stepper.Minimum, (double)value);
			return value;
		});

		public static readonly BindableProperty MinimumProperty = BindableProperty.Create("Minimum", typeof(double), typeof(Stepper), 0.0, validateValue: (bindable, value) =>
		{
			var stepper = (Stepper)bindable;
			return (double)value < stepper.Maximum;
		}, coerceValue: (bindable, value) =>
		{
			var stepper = (Stepper)bindable;
			stepper.Value = stepper.Value.Clamp((double)value, stepper.Maximum);
			return value;
		});

		public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(double), typeof(Stepper), 0.0, BindingMode.TwoWay, coerceValue: (bindable, value) =>
		{
			var stepper = (Stepper)bindable;
			return ((double)value).Clamp(stepper.Minimum, stepper.Maximum);
		}, propertyChanged: (bindable, oldValue, newValue) =>
		{
			var stepper = (Stepper)bindable;
			stepper.ValueChanged?.Invoke(stepper, new ValueChangedEventArgs((double)oldValue, (double)newValue));
		});

		public static readonly BindableProperty IncrementProperty = BindableProperty.Create("Increment", typeof(double), typeof(Stepper), 1.0);

		readonly Lazy<PlatformConfigurationRegistry<Stepper>> _platformConfigurationRegistry;

		public Stepper()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Stepper>>(() => new PlatformConfigurationRegistry<Stepper>(this));
		}

		public Stepper(double min, double max, double val, double increment)
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

			Value = val.Clamp(min, max);
			Increment = increment;
		}

		/// <summary>
		/// The increment by which Value is increased or decreased.
		/// </summary>
		public double Increment
		{
			get { return (double)GetValue(IncrementProperty); }
			set { SetValue(IncrementProperty, value); }
		}

		/// <summary>
		/// Gets or sets the highest possible Value for the stepper.
		/// Must be data-bound before Value to avoid Value being set to default Maximum if Value is greater than default Maximum.
		/// </summary>
		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		/// <summary>
		/// Gets or sets the lowest possible Value for the stepper.
		/// Must be data-bound before Value to avoid Value being set to default Minimum if Value is less than default Minimum.
		/// </summary>
		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		/// <summary>
		/// Gets or sets the current value for the stepper.
		/// Must be data-bound after Minimum and Maximum to avoid falling in the default stepper range.
		/// </summary>
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public event EventHandler<ValueChangedEventArgs> ValueChanged;

		public IPlatformElementConfiguration<T, Stepper> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}