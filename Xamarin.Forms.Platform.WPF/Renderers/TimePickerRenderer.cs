using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WPF.Renderers
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, Xceed.Wpf.Toolkit.TimePicker>
	{
		private bool _disposed;

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var timePicker = new Xceed.Wpf.Toolkit.TimePicker();
					timePicker.ValueChanged += OnNativeSelectedTimeChanged;
					SetNativeControl(timePicker);
				}
			}

			UpdateTime();
			UpdateTextColor();
			UpdateFormat();

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
				UpdateTime();
			if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == TimePicker.FormatProperty.PropertyName)
				UpdateFormat();
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.ValueChanged -= OnNativeSelectedTimeChanged;
				}
			}

			_disposed = true;
			base.Dispose(disposing);
		}

		private void UpdateTime()
		{
			Control.Value = DateTime.Today + Element.Time;
		}

		private void UpdateTextColor()
		{
			if (!Element.TextColor.IsDefault)
				Control.Foreground = Element.TextColor.ToBrush();
			else
				Control.Foreground = Color.Black.ToBrush();
		}

		private void UpdateFormat()
		{
			Control.FormatString = Element.Format;
		}

		private void OnNativeSelectedTimeChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
		{
			var currentTime =  Control.Value;

			((IElementController)Element)?.SetValueFromRenderer(TimePicker.TimeProperty, currentTime);
		}
	}
}