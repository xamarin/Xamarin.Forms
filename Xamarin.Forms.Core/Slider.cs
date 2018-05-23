using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_SliderRenderer))]
	public class Slider : View, IElementConfiguration<Slider>
	{
		public static readonly BindableProperty MinimumProperty = BindableProperty.Create("Minimum", typeof(double), typeof(Slider), 0d, coerceValue: (bindable, value) =>
		{
			var slider = (Slider)bindable;
			slider.Value = Math.Max((double)value, slider.Value);
			return value;
		}, propertyChanged: (bindable, oldValue, newValue) =>
		{
			var slider = (Slider)bindable;
			slider.SetValueCore(MaximumProperty, Math.Max(slider.Maximum, (double)newValue));
		});

		public static readonly BindableProperty MaximumProperty = BindableProperty.Create("Maximum", typeof(double), typeof(Slider), 1d, coerceValue: (bindable, value) =>
		{
			var slider = (Slider)bindable;
			slider.Value = Math.Min((double)value, slider.Value);
			return value;
		}, propertyChanged: (bindable, oldvalue, newValue) =>
		{
			var slider = (Slider)bindable;
			slider.SetValueCore(MinimumProperty, Math.Min(slider.Minimum, (double)newValue));
		});

		public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(double), typeof(Slider), 0d, BindingMode.TwoWay, coerceValue: (bindable, value) =>
		{
			var slider = (Slider)bindable;
			return ((double)value).Clamp(slider.Minimum, slider.Maximum);
		}, propertyChanged: (bindable, oldValue, newValue) =>
		{
			var slider = (Slider)bindable;
			EventHandler<ValueChangedEventArgs> eh = slider.ValueChanged;
			if (eh != null)
				eh(slider, new ValueChangedEventArgs((double)oldValue, (double)newValue));
		});

		public static readonly BindableProperty MinimumTrackColorProperty = BindableProperty.Create(nameof(MinimumTrackColor), typeof(Color), typeof(Slider), Color.Default);

		public static readonly BindableProperty MaximumTrackColorProperty = BindableProperty.Create(nameof(MaximumTrackColor), typeof(Color), typeof(Slider), Color.Default);

		public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(Slider), Color.Default);

		public static readonly BindableProperty ThumbImageProperty = BindableProperty.Create(nameof(ThumbImage), typeof(FileImageSource), typeof(Slider), default(FileImageSource));

		readonly Lazy<PlatformConfigurationRegistry<Slider>> _platformConfigurationRegistry;

		public Slider()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Slider>>(() => new PlatformConfigurationRegistry<Slider>(this));
		}

		public Slider(double min, double max, double val) : this()
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
			Value = val.Clamp(min, max);
		}

		public Color MinimumTrackColor
		{
			get { return (Color)GetValue(MinimumTrackColorProperty); }
			set { SetValue(MinimumTrackColorProperty, value); }
		}

		public Color MaximumTrackColor
		{
			get { return (Color)GetValue(MaximumTrackColorProperty); }
			set { SetValue(MaximumTrackColorProperty, value); }
		}

		public Color ThumbColor
		{
			get { return (Color)GetValue(ThumbColorProperty); }
			set { SetValue(ThumbColorProperty, value); }
		}

		public FileImageSource ThumbImage
		{
			get { return (FileImageSource)GetValue(ThumbImageProperty); }
			set { SetValue(ThumbImageProperty, value); }
		}

		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public event EventHandler<ValueChangedEventArgs> ValueChanged;

		public IPlatformElementConfiguration<T, Slider> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}