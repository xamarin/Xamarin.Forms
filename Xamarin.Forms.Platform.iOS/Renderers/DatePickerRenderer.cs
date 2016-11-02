using System;
using System.Collections.Generic;
using System.ComponentModel;
using Foundation;
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	internal class NoCaretField : UITextFieldWrapper
	{
		public NoCaretField() : base(new RectangleF())
		{
		}

		public override RectangleF GetCaretRectForPosition(UITextPosition position)
		{
			return new RectangleF();
		}
	}

	public class DatePickerRenderer : ViewRenderer<DatePicker, UITextField>
	{
		UIDatePicker _picker;
		UIColor _defaultTextColor;
		NoCaretField _noCaretField;

		bool _disposed;

		IElementController ElementController => Element as IElementController;

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				UnregisterEvents();
			}

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					_noCaretField = new NoCaretField { BorderStyle = UITextBorderStyle.RoundedRect };

					_noCaretField.EditingDidBegin += OnStarted;
					_noCaretField.EditingDidEnd += OnEnded;

					_picker = new UIDatePicker { Mode = UIDatePickerMode.Date, TimeZone = new NSTimeZone("UTC") };

					_picker.ValueChanged += HandleValueChanged;

					var width = UIScreen.MainScreen.Bounds.Width;
					var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
					var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
					var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => _noCaretField.ResignFirstResponder());

					toolbar.SetItems(new[] { spacer, doneButton }, false);

					_noCaretField.InputView = _picker;
					_noCaretField.InputAccessoryView = toolbar;

					_defaultTextColor = _noCaretField.TextColor;

					SetNativeControl(_noCaretField);
				}

				UpdateDateFromModel(false);
				UpdateMaximumDate();
				UpdateMinimumDate();
				UpdateTextColor();
				UpdateDisabledSelectorActions();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
				UpdateDateFromModel(true);
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
				UpdateMinimumDate();
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
				UpdateMaximumDate();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.DatePicker.DisabledSelectorActionsProperty.PropertyName)
				UpdateDisabledSelectorActions();
		}

		void HandleValueChanged(object sender, EventArgs e)
		{
			ElementController?.SetValueFromRenderer(DatePicker.DateProperty, _picker.Date.ToDateTime().Date);
		}

		void OnEnded(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		void OnStarted(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void UpdateDateFromModel(bool animate)
		{
			if (_picker.Date.ToDateTime().Date != Element.Date.Date)
				_picker.SetDate(Element.Date.ToNSDate(), animate);

			Control.Text = Element.Date.ToString(Element.Format);
		}

		void UpdateMaximumDate()
		{
			_picker.MaximumDate = Element.MaximumDate.ToNSDate();
		}

		void UpdateMinimumDate()
		{
			_picker.MinimumDate = Element.MinimumDate.ToNSDate();
		}

		void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault || !Element.IsEnabled)
				Control.TextColor = _defaultTextColor;
			else
				Control.TextColor = textColor.ToUIColor();
		}

		void UpdateDisabledSelectorActions()
		{
			_noCaretField.DisabledSelectorActions = Element.On<PlatformConfiguration.iOS>().DisabledSelectorActions();
		}

		void UnregisterEvents()
		{
			if (_noCaretField != null)
			{
				_noCaretField.EditingDidBegin -= OnStarted;
				_noCaretField.EditingDidEnd -= OnEnded;
			}

			if (_picker != null)
				_picker.ValueChanged -= HandleValueChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				UnregisterEvents();

				if (_defaultTextColor != null)
				{
					_defaultTextColor.Dispose();
					_defaultTextColor = null;
				}

				if (_noCaretField != null)
				{
					_noCaretField.Dispose();
					_noCaretField = null;
				}

				if (_picker != null)
				{
					_picker.Dispose();
					_picker = null;
				}
			}

			_disposed = true;

			base.Dispose(disposing);
		}
	}
}