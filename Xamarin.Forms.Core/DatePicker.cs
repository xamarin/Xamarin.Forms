using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_DatePickerRenderer))]
	public class DatePicker : View, IFontElement, ITextElement, IElementConfiguration<DatePicker>
	{
		bool _isCoercingDateMinOrMax;

		public static readonly BindableProperty FormatProperty = BindableProperty.Create(nameof(Format), typeof(string), typeof(DatePicker), "d");

		[Obsolete("Date is obsolete as of version x.x.x. Please use SelectedDate instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(DatePicker), default(DateTime), BindingMode.TwoWay,
			coerceValue: CoerceDate,
			propertyChanged: DatePropertyChanged,
			defaultValueCreator: (bindable) => DateTime.Today);

		public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(DatePicker), default(DateTime?), BindingMode.TwoWay,
			coerceValue: CoerceSelectedDate,
			propertyChanged: SelectedDatePropertyChanged,
			defaultValueCreator: (bindable) => DateTime.Today);

		public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(DatePicker), new DateTime(1900, 1, 1),
			validateValue: ValidateMinimumDate, coerceValue: CoerceMinimumDate);

		public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(DatePicker), new DateTime(2100, 12, 31),
			validateValue: ValidateMaximumDate, coerceValue: CoerceMaximumDate);

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		readonly Lazy<PlatformConfigurationRegistry<DatePicker>> _platformConfigurationRegistry;

		public DatePicker()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<DatePicker>>(() => new PlatformConfigurationRegistry<DatePicker>(this));
		}

		[Obsolete("Date is obsolete as of version x.x.x. Please use SelectedDate instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public DateTime? SelectedDate
		{
			get { return (DateTime?)GetValue(SelectedDateProperty); }
			set { SetValue(SelectedDateProperty, value); }
		}

		public string Format
		{
			get { return (string)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}

		public DateTime MaximumDate
		{
			get { return (DateTime)GetValue(MaximumDateProperty); }
			set { SetValue(MaximumDateProperty, value); }
		}

		public DateTime MinimumDate
		{
			get { return (DateTime)GetValue(MinimumDateProperty); }
			set { SetValue(MinimumDateProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (DatePicker)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		[Obsolete("DateSelected is obsolete as of version x.x.x. Please use SelectedDateChanged instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<DateChangedEventArgs> DateSelected;

		public event EventHandler<SelectedDateChangedEventArgs> SelectedDateChanged;

		static object CoerceDate(BindableObject bindable, object value)
		{
			var picker = (DatePicker)bindable;
			DateTime dateValue = ((DateTime)value).Date;

			if (dateValue > picker.MaximumDate)
				dateValue = picker.MaximumDate;

			if (dateValue < picker.MinimumDate)
				dateValue = picker.MinimumDate;

			return dateValue;
		}

		static object CoerceSelectedDate(BindableObject bindable, object value)
		{
			DateTime? newSelectedDate = ((DateTime?)value)?.Date;
			if (newSelectedDate == null)
				return null;

			var picker = (DatePicker)bindable;
			if (newSelectedDate > picker.MaximumDate)
				newSelectedDate = picker.MaximumDate;

			if (newSelectedDate < picker.MinimumDate)
				newSelectedDate = picker.MinimumDate;

			return newSelectedDate;
		}


		static object CoerceMaximumDate(BindableObject bindable, object value)
		{
			DateTime dateValue = ((DateTime)value).Date;
			var picker = (DatePicker)bindable;
#pragma warning disable 0618
			if (picker.Date > dateValue)
			{
				picker._isCoercingDateMinOrMax = true;
				picker.Date = dateValue;
				picker._isCoercingDateMinOrMax = false;
			}
#pragma warning restore

			return dateValue;
		}

		static object CoerceMinimumDate(BindableObject bindable, object value)
		{
			DateTime dateValue = ((DateTime)value).Date;
			var picker = (DatePicker)bindable;
#pragma warning disable 0618
			if (picker.Date < dateValue)
			{
				picker._isCoercingDateMinOrMax = true;
				picker.Date = dateValue;
				picker._isCoercingDateMinOrMax = false;
			}
#pragma warning restore

			return dateValue;
		}

		static void DatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var datePicker = (DatePicker)bindable;

			// Keep SelectedDate null when Date is changed as a result of coercing MininimumDate or MaximumumDate
			if (!datePicker._isCoercingDateMinOrMax || datePicker.SelectedDate != null)
				datePicker.SetValueFromRenderer(SelectedDateProperty, newValue);

			datePicker.DateSelected?.Invoke(datePicker, new DateChangedEventArgs((DateTime)oldValue, (DateTime)newValue));
		}

		static void SelectedDatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var datePicker = (DatePicker)bindable;

#pragma warning disable 0618
			datePicker.SetValueFromRenderer(DateProperty, newValue);
#pragma warning restore

			datePicker.SelectedDateChanged?.Invoke(datePicker, new SelectedDateChangedEventArgs((DateTime?)oldValue, (DateTime?)newValue));
		}

		static bool ValidateMaximumDate(BindableObject bindable, object value)
		{
			return (DateTime)value >= ((DatePicker)bindable).MinimumDate;
		}

		static bool ValidateMinimumDate(BindableObject bindable, object value)
		{
			return (DateTime)value <= ((DatePicker)bindable).MaximumDate;
		}

		public IPlatformElementConfiguration<T, DatePicker> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}
	}
}