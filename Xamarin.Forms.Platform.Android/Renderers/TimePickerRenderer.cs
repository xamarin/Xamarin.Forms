using System;
using System.ComponentModel;
using Android.App;
using Android.Content.Res;
using Android.Widget;
using ADatePicker = Android.Widget.DatePicker;
using ATimePicker = Android.Widget.TimePicker;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, EditText>, TimePickerDialog.IOnTimeSetListener
	{
		AlertDialog _dialog;
		TextColorSwitcher _textColorSwitcher;

		public TimePickerRenderer()
		{
			AutoPackage = false;
		}

		IElementController ElementController => Element as IElementController;

		void TimePickerDialog.IOnTimeSetListener.OnTimeSet(ATimePicker view, int hourOfDay, int minute)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);

			ElementController.SetValueFromRenderer(TimePicker.TimeProperty, new TimeSpan(hourOfDay, minute, 0));
			Control.ClearFocus();

			if (Forms.IsLollipopOrNewer)
				_dialog.CancelEvent -= OnCancelButtonClicked;

			_dialog = null;
		}

		protected override EditText CreateNativeControl()
		{
			return new EditText(Context) { Focusable = false, Clickable = true, Tag = this };
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var textField = CreateNativeControl();

				textField.SetOnClickListener(TimePickerListener.Instance);
				SetNativeControl(textField);
				_textColorSwitcher = new TextColorSwitcher(textField.TextColors); 
			}

			SetTime(e.NewElement.Time);
			UpdateTextColor();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TimePicker.TimeProperty.PropertyName ||
			    e.PropertyName == TimePicker.FormatProperty.PropertyName)
				SetTime(Element.Time);

			if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
				UpdateTextColor();
		}

		internal override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			base.OnFocusChangeRequested(sender, e);

			if (e.Focus)
				OnClick();
			else if (_dialog != null)
			{
				_dialog.Hide();
				ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
				Control.ClearFocus();

				if (Forms.IsLollipopOrNewer)
					_dialog.CancelEvent -= OnCancelButtonClicked;

				_dialog = null;
			}
		}

		void OnClick()
		{
			TimePicker view = Element;
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);

			_dialog = new TimePickerDialog(Context, this, view.Time.Hours, view.Time.Minutes, false);

			if (Forms.IsLollipopOrNewer)
				_dialog.CancelEvent += OnCancelButtonClicked;

			_dialog.Show();
		}

		void OnCancelButtonClicked(object sender, EventArgs e)
		{
			Element.Unfocus();
		}

		void SetTime(TimeSpan time)
		{
			Control.Text = DateTime.Today.Add(time).ToString(Element.Format);
		}

		void UpdateTextColor()
		{
			_textColorSwitcher?.UpdateTextColor(Control, Element.TextColor);
		}

		class TimePickerListener : Object, IOnClickListener
		{
			public static readonly TimePickerListener Instance = new TimePickerListener();

			public void OnClick(global::Android.Views.View v)
			{
				var renderer = v.Tag as TimePickerRenderer;
				if (renderer == null)
					return;

				renderer.OnClick();
			}
		}
	}
}