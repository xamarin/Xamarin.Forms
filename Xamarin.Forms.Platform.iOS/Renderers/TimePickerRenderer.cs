using System;
using System.Drawing;
using System.ComponentModel;
#if __UNIFIED__
using UIKit;
using Foundation;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#endif
#if __UNIFIED__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;

#else
using nfloat=System.Single;
using nint=System.Int32;
using nuint=System.UInt32;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	public class TimePickerRenderer : ViewRenderer<TimePicker, UITextField>
	{
		UIDatePicker _picker;
		UIColor _defaultTextColor;

		IElementController ElementController => Element as IElementController;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Control.Started -= OnStarted;
				Control.Ended -= OnEnded;

				_picker.ValueChanged -= OnValueChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var entry = new NoCaretField { BorderStyle = UITextBorderStyle.RoundedRect };

					entry.Started += OnStarted;
					entry.Ended += OnEnded;

					_picker = new UIDatePicker { Mode = UIDatePickerMode.Time, TimeZone = new NSTimeZone("UTC") };

					var width = UIScreen.MainScreen.Bounds.Width;
					var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
					var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
					var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => entry.ResignFirstResponder());

					toolbar.SetItems(new[] { spacer, doneButton }, false);

					entry.InputView = _picker;
					entry.InputAccessoryView = toolbar;

					_defaultTextColor = entry.TextColor;

					_picker.ValueChanged += OnValueChanged;

					SetNativeControl(entry);
				}

				UpdateTime();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == TimePicker.TimeProperty.PropertyName || e.PropertyName == TimePicker.FormatProperty.PropertyName)
				UpdateTime();

			if (e.PropertyName == TimePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateTextColor();
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
	}
}