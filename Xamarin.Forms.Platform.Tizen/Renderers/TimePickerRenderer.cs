using System;
using System.Globalization;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, Native.Button>
	{
		//TODO need to add internationalization support
		const string DialogTitle = "Choose Time";

		static readonly string s_defaultFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

		static readonly EColor s_defaultTextColor = EColor.White;

		string _format;

		TimeSpan _time;

		public TimePickerRenderer()
		{
			RegisterPropertyHandler(TimePicker.FormatProperty, UpdateFormat);
			RegisterPropertyHandler(TimePicker.TimeProperty, UpdateTime);
			RegisterPropertyHandler(TimePicker.TextColorProperty, UpdateTextColor);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			if (Control == null)
			{
				var button = new Native.Button(Forms.Context.MainWindow);
				SetNativeControl(button);
			}

			if (e.OldElement != null)
			{
				Control.Clicked -= ButtonClickedHandler;
			}

			if (e.NewElement != null)
			{
				_time = DateTime.Now.TimeOfDay;
				_format = s_defaultFormat;
				UpdateTimeAndFormat();

				Control.Clicked += ButtonClickedHandler;
			}

			base.OnElementChanged(e);
		}

		void ButtonClickedHandler(object o, EventArgs e)
		{
			Native.DateTimePickerDialog dialog = new Native.DateTimePickerDialog(Forms.Context.MainWindow)
			{
				Title = DialogTitle
			};

			dialog.InitializeTimePicker(_time, _format);
			dialog.DateTimeChanged += DialogDateTimeChangedHandler;
			dialog.Dismissed += DialogDismissedHandler;
			dialog.Show();
		}

		void DialogDateTimeChangedHandler(object sender, Native.DateChangedEventArgs dcea)
		{
			Element.Time = dcea.NewDate.TimeOfDay;
			UpdateTime();
		}

		void DialogDismissedHandler(object sender, EventArgs e)
		{
			var dialog = sender as Native.DateTimePickerDialog;
			dialog.DateTimeChanged -= DialogDateTimeChangedHandler;
			dialog.Dismissed -= DialogDismissedHandler;
		}

		void UpdateFormat()
		{
			_format = Element.Format ?? s_defaultFormat;
			UpdateTimeAndFormat();
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.IsDefault ? s_defaultTextColor : Element.TextColor.ToNative();
		}

		void UpdateTime()
		{
			_time = Element.Time;
			UpdateTimeAndFormat();
		}

		void UpdateTimeAndFormat()
		{
			// Xamarin using DateTime formatting (https://developer.xamarin.com/api/property/Xamarin.Forms.TimePicker.Format/)
			Control.Text = new DateTime(_time.Ticks).ToString(_format);
		}
	}
}
