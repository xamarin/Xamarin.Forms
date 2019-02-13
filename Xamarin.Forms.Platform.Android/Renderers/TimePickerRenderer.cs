using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Text.Format;
using ATimePicker = Android.Widget.TimePicker;
using Android.OS;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, EditText>, TimePickerDialog.IOnTimeSetListener, IPickerRenderer
	{
		AlertDialog _dialog;
		TextColorSwitcher _textColorSwitcher;

		bool Is24HourView
		{
			get => (DateFormat.Is24HourFormat(Context) && Element.Format == (string)TimePicker.FormatProperty.DefaultValue) || Element.Format == "HH:mm";
		}

		public TimePickerRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use TimePickerRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TimePickerRenderer()
		{
			AutoPackage = false;
		}

		IElementController ElementController => Element as IElementController;

		void TimePickerDialog.IOnTimeSetListener.OnTimeSet(ATimePicker view, int hourOfDay, int minute)
		{
			ElementController.SetValueFromRenderer(TimePicker.TimeProperty, new TimeSpan(hourOfDay, minute, 0));

			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);

			if (Forms.IsLollipopOrNewer)
				_dialog.CancelEvent -= OnCancelButtonClicked;

			_dialog = null;
		}

		protected override EditText CreateNativeControl()
		{
			return new PickerEditText(Context, this);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var textField = CreateNativeControl();

				SetNativeControl(textField);

				var useLegacyColorManagement = e.NewElement.UseLegacyColorManagement();
				_textColorSwitcher = new TextColorSwitcher(textField.TextColors, useLegacyColorManagement);
			}

			SetTime(e.NewElement.Time);
			UpdateTextColor();
			UpdateFont();

			if ((int)Build.VERSION.SdkInt > 16)
				Control.TextAlignment = global::Android.Views.TextAlignment.ViewStart;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TimePicker.TimeProperty.PropertyName || e.PropertyName == TimePicker.FormatProperty.PropertyName)
				SetTime(Element.Time);
			else if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == TimePicker.FontAttributesProperty.PropertyName || e.PropertyName == TimePicker.FontFamilyProperty.PropertyName || e.PropertyName == TimePicker.FontSizeProperty.PropertyName)
				UpdateFont();
		}

		protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			base.OnFocusChangeRequested(sender, e);

			if (e.Focus)
				CallOnClick();
			else if (_dialog != null)
			{
				_dialog.Hide();
				ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);

				if (Forms.IsLollipopOrNewer)
					_dialog.CancelEvent -= OnCancelButtonClicked;

				_dialog = null;
			}
		}

		protected virtual TimePickerDialog CreateTimePickerDialog(int hours, int minutes)
		{
			var dialog = new TimePickerDialog(Context, this, hours, minutes, Is24HourView);

			if (Forms.IsLollipopOrNewer)
				dialog.CancelEvent += OnCancelButtonClicked;

			return dialog;
		}

		void IPickerRenderer.OnClick()
		{
			if (_dialog != null && _dialog.IsShowing)
			{
				return;
			}

			TimePicker view = Element;
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);

			_dialog = CreateTimePickerDialog(view.Time.Hours, view.Time.Minutes);
			_dialog.Show();
		}

		void OnCancelButtonClicked(object sender, EventArgs e)
		{
			Element.Unfocus();
		}

		void SetTime(TimeSpan time)
		{
			var timeFormat = Is24HourView ? "HH:mm" : Element.Format;
			Control.Text = DateTime.Today.Add(time).ToString(timeFormat);
		}

		void UpdateFont()
		{
			Control.Typeface = Element.ToTypeface();
			Control.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
		}

		void UpdateTextColor()
		{
			_textColorSwitcher?.UpdateTextColor(Control, Element.TextColor);
		}
	}
}
