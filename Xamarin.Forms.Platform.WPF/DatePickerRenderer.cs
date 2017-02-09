using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, System.Windows.Controls.DatePicker>
	{
		Brush _defaultBrush;

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			var datePicker = new System.Windows.Controls.DatePicker { SelectedDate= Element.Date };
            

			datePicker.Loaded += (sender, args) => {
				// The defaults from the control template won't be available
				// right away; we have to wait until after the template has been applied
				_defaultBrush = datePicker.Foreground;
				UpdateTextColor();
			};

			datePicker.SelectedDateChanged += DatePickerOnValueChanged;
			SetNativeControl(datePicker);

			UpdateFormatString();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName)
				Control.SelectedDate = Element.Date;
			else if (e.PropertyName == DatePicker.FormatProperty.PropertyName)
				UpdateFormatString();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
		}

		internal override void OnModelFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
		{
			System.Windows.Controls.DatePicker control = Control;
			if (control == null)
				return;

			if (args.Focus)
			{
				//typeof(DateTimePickerBase).InvokeMember("OpenPickerPage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, Type.DefaultBinder, control, null);
				args.Result = true;
			}
			else
			{
				UnfocusControl(control);
				args.Result = true;
			}
		}

		void DatePickerOnValueChanged(object sender, SelectionChangedEventArgs dateTimeValueChangedEventArgs)
		{
			if (Control.SelectedDate.HasValue)
				((IElementController)Element).SetValueFromRenderer(DatePicker.DateProperty, Control.SelectedDate.Value);
		}

		void UpdateFormatString()
		{
            //"{0:" + Element.Format + "}";
            Control.SelectedDateFormat = DatePickerFormat.Long;
		}

		void UpdateTextColor()
		{
			Color color = Element.TextColor;
			Control.Foreground = color.IsDefault ? (_defaultBrush ?? color.ToBrush()) : color.ToBrush();
		}
	}
}