using System;
using EColor = ElmSharp.Color;

namespace Xamarin.Forms.Platform.Tizen
{
	public class DatePickerRenderer : ViewRenderer<DatePicker, Native.Button>
	{
		//TODO need to add internationalization support
		const string DialogTitle = "Choose Date";
		static readonly EColor s_defaultTextColor = EColor.White;

		public DatePickerRenderer()
		{
			RegisterPropertyHandler(DatePicker.DateProperty, UpdateDate);
			RegisterPropertyHandler(DatePicker.FormatProperty, UpdateDate);
			RegisterPropertyHandler(DatePicker.TextColorProperty, UpdateTextColor);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
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
				Control.Clicked += ButtonClickedHandler;
			}

			base.OnElementChanged(e);
		}

		void ButtonClickedHandler(object sender, EventArgs e)
		{
			Native.DateTimePickerDialog dialog = new Native.DateTimePickerDialog(Forms.Context.MainWindow)
			{
				Title = DialogTitle
			};

			dialog.InitializeDatePicker(Element.Date, Element.MinimumDate, Element.MaximumDate);
			dialog.DateTimeChanged += DialogDateTimeChangedHandler;
			dialog.Dismissed += DialogDismissedHandler;
			dialog.Show();
		}

		void DialogDateTimeChangedHandler(object sender, Native.DateChangedEventArgs dcea)
		{
			Element.Date = dcea.NewDate;
			Control.Text = dcea.NewDate.ToString(Element.Format);
		}

		void DialogDismissedHandler(object sender, EventArgs e)
		{
			var dialog = sender as Native.DateTimePickerDialog;
			dialog.DateTimeChanged -= DialogDateTimeChangedHandler;
			dialog.Dismissed -= DialogDismissedHandler;
		}

		void UpdateDate()
		{
			Control.Text = Element.Date.ToString(Element.Format);
		}

		void UpdateTextColor()
		{
			Control.TextColor = Element.TextColor.IsDefault ? s_defaultTextColor : Element.TextColor.ToNative();
		}

	}
}
