using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	internal class NoCaretField : UITextField
	{
		public NoCaretField() : base(new RectangleF())
		{
			SpellCheckingType = UITextSpellCheckingType.No;
			AutocorrectionType = UITextAutocorrectionType.No;
			AutocapitalizationType = UITextAutocapitalizationType.None;
		}

		public override RectangleF GetCaretRectForPosition(UITextPosition position)
		{
			return new RectangleF();
		}

	}

	public class DatePickerRenderer : DatePickerRendererBase<UIControl>
	{
		[Internals.Preserve(Conditional = true)]
		public DatePickerRenderer()
		{

		}

		protected override UIControl CreateNativeControl()
		{
			var datePickerStyle = Element.OnThisPlatform().UIDatePickerStyle();
			if (datePickerStyle != PlatformConfiguration.iOSSpecific.UIDatePickerStyle.Automatic && Forms.IsiOS13OrNewer)
				return new UIDatePicker { Mode = UIDatePickerMode.Date, TimeZone = new NSTimeZone("UTC") };
			else
				return new NoCaretField { BorderStyle = UITextBorderStyle.RoundedRect };
		}
	}

	public abstract class DatePickerRendererBase<TControl> : ViewRenderer<DatePicker, TControl>
		where TControl : UIControl
	{
		UIDatePicker _picker;
		protected UITextField TextField => Control as UITextField;
		UIColor _defaultTextColor;
		bool _disposed;
		bool _useLegacyColorManagement;

		IElementController ElementController => Element as IElementController;


		abstract protected override TControl CreateNativeControl();

		[Internals.Preserve(Conditional = true)]
		public DatePickerRendererBase()
		{

		}

		protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{

				if (Control == null)
				{
					var control = CreateNativeControl();

					//iOS14 uses UIDatePicker compact
					if ((Control is UIDatePicker))
					{
						_picker = control as UIDatePicker;
					}
					else
					{
						_picker = new UIDatePicker { Mode = UIDatePickerMode.Time, TimeZone = new NSTimeZone("UTC") };

						if (Forms.IsiOS13OrNewer)
						{
							_picker.PreferredDatePickerStyle = UIKit.UIDatePickerStyle.Wheels;
						}

						//Not using iOS14 or using the MaterialTimePicker
						var entry = control as UITextField;

						InitTextField(entry);
					}

					_useLegacyColorManagement = e.NewElement.UseLegacyColorManagement();

					UpdateDatePickerStyle();
					_picker.ValueChanged += HandleValueChanged;

					SetNativeControl(control);
				}
			}

			UpdateDateFromModel(false);
			UpdateFont();
			UpdateMaximumDate();
			UpdateMinimumDate();
			UpdateTextColor();
			UpdateCharacterSpacing();
			UpdateFlowDirection();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
			{
				UpdateDateFromModel(true);
				UpdateCharacterSpacing();
			}
			else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
				UpdateMinimumDate();
			else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
				UpdateMaximumDate();
			else if (e.PropertyName == DatePicker.CharacterSpacingProperty.PropertyName)
				UpdateCharacterSpacing();
			else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateFlowDirection();
			else if (e.PropertyName == DatePicker.FontAttributesProperty.PropertyName ||
					 e.PropertyName == DatePicker.FontFamilyProperty.PropertyName || e.PropertyName == DatePicker.FontSizeProperty.PropertyName)
			{
				UpdateFont();
			}
		}

		void HandleValueChanged(object sender, EventArgs e)
		{
			if (Element.OnThisPlatform().UpdateMode() == UpdateMode.Immediately)
			{
				UpdateElementDate();
			}
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

			if (TextField == null)
			  return;

			//can't use Element.Format because it won't display the correct format if the region and language are set differently
			if (string.IsNullOrWhiteSpace(Element.Format) || Element.Format.Equals("d") || Element.Format.Equals("D"))
			{
				NSDateFormatter dateFormatter = new NSDateFormatter();
				if (Element.Format?.Equals("D") == true)
				{
					dateFormatter.DateStyle = NSDateFormatterStyle.Long;
					var strDate = dateFormatter.StringFor(_picker.Date);
					TextField.Text = strDate;
				}
				else
				{
					dateFormatter.DateStyle = NSDateFormatterStyle.Short;
					var strDate = dateFormatter.StringFor(_picker.Date);
					TextField.Text = strDate;
				}
			}
			else if (Element.Format.Contains("/"))
			{
				TextField.Text = Element.Date.ToString(Element.Format, CultureInfo.InvariantCulture);
			}
			else
			{
				TextField.Text = Element.Date.ToString(Element.Format);
			}
		}

		void UpdateElementDate()
		{
			ElementController.SetValueFromRenderer(DatePicker.DateProperty, _picker.Date.ToDateTime().Date);
		}

		void UpdateFlowDirection()
		{
			(Control as UITextField).UpdateTextAlignment(Element);
		}

		protected internal virtual void UpdateFont()
		{
			if (TextField != null)
				TextField.Font = Element.ToUIFont();
		}

		void UpdateCharacterSpacing()
		{
			if (TextField == null)
				return;

			var textAttr = TextField.AttributedText.AddCharacterSpacing(TextField.Text, Element.CharacterSpacing);

			if (textAttr != null)
				TextField.AttributedText = textAttr;
		}
		void UpdateMaximumDate()
		{
			_picker.MaximumDate = Element.MaximumDate.ToNSDate();
		}

		void UpdateMinimumDate()
		{
			_picker.MinimumDate = Element.MinimumDate.ToNSDate();
		}

		protected internal virtual void UpdateTextColor()
		{
			if (TextField == null)
				return;

			var textColor = Element.TextColor;

			if (textColor.IsDefault || (!Element.IsEnabled && _useLegacyColorManagement))
				TextField.TextColor = _defaultTextColor;
			else
				TextField.TextColor = textColor.ToUIColor();

			// HACK This forces the color to update; there's probably a more elegant way to make this happen
			TextField.Text = TextField.Text;
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				_defaultTextColor = null;

				if (_picker != null)
				{
					_picker.RemoveFromSuperview();
					_picker.ValueChanged -= HandleValueChanged;
					_picker.Dispose();
					_picker = null;
				}

				if (Control != null)
				{
					Control.EditingDidBegin -= OnStarted;
					Control.EditingDidEnd -= OnEnded;
				}
			}

			base.Dispose(disposing);
		}

		void InitTextField(UITextField textField)
		{
			if (textField == null)
				return;

			textField.EditingDidBegin += OnStarted;
			textField.EditingDidEnd += OnEnded;

			var width = UIScreen.MainScreen.Bounds.Width;
			var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

			var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
			{
				UpdateElementDate();
				textField.ResignFirstResponder();
			});

			toolbar.SetItems(new[] { spacer, doneButton }, false);

			textField.InputView = _picker;
			textField.InputAccessoryView = toolbar;

			textField.InputView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
			textField.InputAccessoryView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;

			textField.InputAssistantItem.LeadingBarButtonGroups = null;
			textField.InputAssistantItem.TrailingBarButtonGroups = null;

			_defaultTextColor = textField.TextColor;

			textField.AccessibilityTraits = UIAccessibilityTrait.Button;
		}

		void UpdateDatePickerStyle()
		{
			if (!Forms.IsiOS13OrNewer)
				return;

			var datePickerStyle = Element.OnThisPlatform().UIDatePickerStyle();

			switch (datePickerStyle)
			{
				case PlatformConfiguration.iOSSpecific.UIDatePickerStyle.Compact:
					_picker.PreferredDatePickerStyle = UIKit.UIDatePickerStyle.Compact;
					break;
				case PlatformConfiguration.iOSSpecific.UIDatePickerStyle.Inline:
					_picker.PreferredDatePickerStyle = UIKit.UIDatePickerStyle.Inline;
					break;
				case PlatformConfiguration.iOSSpecific.UIDatePickerStyle.Wheels:
					_picker.PreferredDatePickerStyle = UIKit.UIDatePickerStyle.Wheels;
					break;
				default:
					break;
			}
		}
	}
}