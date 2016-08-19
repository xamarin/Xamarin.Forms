using System;
using System.ComponentModel;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, NSDatePicker>
	{
		NSDatePicker _picker;
		NSColor _defaultTextColor;
		NSColor _defaultBackgroundColor;
		bool _disposed;

		IElementController ElementController => Element as IElementController;

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				_picker = new NSDatePicker { DatePickerMode = NSDatePickerMode.Single, TimeZone = new NSTimeZone("UTC") };
				_picker.DatePickerStyle = NSDatePickerStyle.TextFieldAndStepper;
				_picker.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay;
				_picker.ValidateProposedDateValue += HandleValueChanged;
				_defaultTextColor = _picker.TextColor;
				_defaultBackgroundColor = _picker.BackgroundColor;

				SetNativeControl(_picker);
			}

			if (e.NewElement != null)
			{
				UpdateDateFromModel();
				UpdateMaximumDate();
				UpdateMinimumDate();
				UpdateTextColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
				UpdateDateFromModel();
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
				UpdateMinimumDate();
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
				UpdateMaximumDate();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateTextColor();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				if (_picker != null)
					_picker.ValidateProposedDateValue -= HandleValueChanged;

				_disposed = true;

			}
			base.Dispose(disposing);
		}

		protected override void SetBackgroundColor(Color color)
		{
			if (color == Color.Default)
				Control.BackgroundColor = _defaultBackgroundColor;
			else
				Control.BackgroundColor = color.ToNSColor();

			base.SetBackgroundColor(color);
		}

		void HandleValueChanged(object sender, NSDatePickerValidatorEventArgs e)
		{
			ElementController?.SetValueFromRenderer(DatePicker.DateProperty, _picker.DateValue.ToDateTime().Date);
		}

		void OnEnded(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		void OnStarted(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void UpdateDateFromModel()
		{
			if (_picker.DateValue.ToDateTime().Date != Element.Date.Date)
				_picker.DateValue = Element.Date.ToNSDate();
		}

		void UpdateMaximumDate()
		{
			_picker.MaxDate = Element.MaximumDate.ToNSDate();
		}

		void UpdateMinimumDate()
		{
			_picker.MinDate = Element.MinimumDate.ToNSDate();
		}

		void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault || !Element.IsEnabled)
				Control.TextColor = _defaultTextColor;
			else
				Control.TextColor = textColor.ToNSColor();
		}
	}
}

