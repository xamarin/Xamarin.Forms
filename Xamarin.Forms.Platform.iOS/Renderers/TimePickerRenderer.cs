using System;
using System.ComponentModel;
using Foundation;
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, UITextField>
	{
		UIDatePicker _picker;
		UIColor _defaultTextColor;
		NoCaretField _noCaretField;

		bool _disposed;

		IElementController ElementController => Element as IElementController;

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

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
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

					_picker = new UIDatePicker { Mode = UIDatePickerMode.Time, TimeZone = new NSTimeZone("UTC") };

					var width = UIScreen.MainScreen.Bounds.Width;
					var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
					var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
					var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => _noCaretField.ResignFirstResponder());

					toolbar.SetItems(new[] { spacer, doneButton }, false);

					_noCaretField.InputView = _picker;
					_noCaretField.InputAccessoryView = toolbar;

					_defaultTextColor = _noCaretField.TextColor;

					_picker.ValueChanged += OnValueChanged;

					SetNativeControl(_noCaretField);
				}

				UpdateTime();
				UpdateTextColor();
				UpdateDisabledSelectorActions();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TimePicker.TimeProperty.PropertyName || e.PropertyName == TimePicker.FormatProperty.PropertyName)
				UpdateTime();
			else if (e.PropertyName == TimePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.TimePicker.DisabledSelectorActionsProperty.PropertyName)
				UpdateDisabledSelectorActions();
		}

		void OnEnded(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		void OnStarted(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void OnValueChanged(object sender, EventArgs e)
		{
			ElementController.SetValueFromRenderer(TimePicker.TimeProperty, _picker.Date.ToDateTime() - new DateTime(1, 1, 1));
		}

		void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault || !Element.IsEnabled)
				Control.TextColor = _defaultTextColor;
			else
				Control.TextColor = textColor.ToUIColor();
		}

		void UpdateTime()
		{
			_picker.Date = new DateTime(1, 1, 1).Add(Element.Time).ToNSDate();
			Control.Text = DateTime.Today.Add(Element.Time).ToString(Element.Format);
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
				_picker.ValueChanged -= OnValueChanged;
		}
	}
}