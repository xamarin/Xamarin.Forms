using System;
using System.ComponentModel;
using Foundation;
using UIKit;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	internal class NoCaretField : UITextField
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

		IElementController ElementController => Element as IElementController;

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				var entry = new NoCaretField { BorderStyle = UITextBorderStyle.RoundedRect };

				entry.EditingDidBegin += OnStarted;
				entry.EditingDidEnd += OnEnded;

				_picker = new UIDatePicker { Mode = UIDatePickerMode.Date, TimeZone = new NSTimeZone("UTC") };

				_picker.ValueChanged += HandleValueChanged;

				var width = UIScreen.MainScreen.Bounds.Width;
				var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
				var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
				var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => entry.ResignFirstResponder());

				toolbar.SetItems(new[] { spacer, doneButton }, false);

				entry.InputView = _picker;
				entry.InputAccessoryView = toolbar;

				_defaultTextColor = entry.TextColor;

				SetNativeControl(entry);
			}

			if (e.NewElement != null)
			{
				UpdateDateFromModel(false);
				UpdateMaximumDate();
				UpdateMinimumDate();
				UpdateTextColor();
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
	}
}