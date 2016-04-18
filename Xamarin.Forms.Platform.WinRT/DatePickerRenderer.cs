﻿using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, FormsDatePicker>, IWrapperAware
	{
		Brush _defaultBrush;

		public void NotifyWrapped()
		{
			if (Control != null)
			{
				Control.ForceInvalidate += PickerOnForceInvalidate;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Control != null)
			{
				Control.ForceInvalidate -= PickerOnForceInvalidate;
				Control.DateChanged -= OnControlDateChanged;
				Control.Loaded -= ControlOnLoaded;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var picker = new FormsDatePicker();
					SetNativeControl(picker);
					Control.Loaded += ControlOnLoaded;
					Control.DateChanged += OnControlDateChanged;
				}

				UpdateMinimumDate();
				UpdateMaximumDate();
				UpdateDate(e.NewElement.Date);
			}

			base.OnElementChanged(e);
		}

		void ControlOnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			// The defaults from the control template won't be available
			// right away; we have to wait until after the template has been applied
			_defaultBrush = Control.Foreground;
			UpdateTextColor();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName)
				UpdateDate(Element.Date);
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
				UpdateMaximumDate();
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
				UpdateMinimumDate();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
		}

		void OnControlDateChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			Element.Date = e.NewDate.Date;
			DateTime currentDate = Element.Date;
			if (currentDate != e.NewDate.Date) // Match coerced value
				UpdateDate(currentDate);

			Element.InvalidateMeasure(InvalidationTrigger.SizeRequestChanged);
		}

		void PickerOnForceInvalidate(object sender, EventArgs eventArgs)
		{
			Element?.InvalidateMeasure(InvalidationTrigger.SizeRequestChanged);
		}

		void UpdateDate(DateTime date)
		{
			Control.Date = date;
		}

		void UpdateMaximumDate()
		{
			DateTime maxdate = Element.MaximumDate;
			Control.MaxYear = new DateTimeOffset(maxdate.Date);
		}

		void UpdateMinimumDate()
		{
			DateTime mindate = Element.MinimumDate;
			Control.MinYear = new DateTimeOffset(mindate);
		}

		void UpdateTextColor()
		{
			Color color = Element.TextColor;
			Control.Foreground = color.IsDefault ? (_defaultBrush ?? color.ToBrush()) : color.ToBrush();
		}
	}
}