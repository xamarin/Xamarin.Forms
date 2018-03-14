using System;
using System.ComponentModel;
using Foundation;
using UIKit;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	public class EditorRenderer : ViewRenderer<Editor, UITextView>
	{
		bool _disposed;
		IEditorController ElementController => Element;
		UILabel _placeholderLabel;

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (Control != null)
				{
					Control.Changed -= HandleChanged;
					Control.Started -= OnStarted;
					Control.Ended -= OnEnded;
					Control.ShouldChangeText -= ShouldChangeText;
				}
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement == null)
				return;

			if (Control == null)
			{
				SetNativeControl(new UITextView(RectangleF.Empty));

				if (Device.Idiom == TargetIdiom.Phone)
				{
					// iPhone does not have a dismiss keyboard button
					var keyboardWidth = UIScreen.MainScreen.Bounds.Width;
					var accessoryView = new UIToolbar(new RectangleF(0, 0, keyboardWidth, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };

					var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
					var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
					{
						Control.ResignFirstResponder();
						ElementController.SendCompleted();
					});
					accessoryView.SetItems(new[] { spacer, doneButton }, false);
					Control.InputAccessoryView = accessoryView;
				}

				Control.Changed += HandleChanged;
				Control.Started += OnStarted;
				Control.Ended += OnEnded;
				Control.ShouldChangeText += ShouldChangeText;
			}

			CreatePlaceholderLabel();
			UpdateTextColor();
			UpdateText();
			UpdateFont();
			UpdateKeyboard();
			UpdateEditable();
			UpdateTextAlignment();
			UpdateMaxLength();
		}

		void CreatePlaceholderLabel()
		{
			_placeholderLabel = new UILabel
			{
				Text = Element.Placeholder,
				TextColor = Element.PlaceholderColor.ToUIColor(),
				BackgroundColor = UIColor.Clear
			};

			Control.AddSubview(_placeholderLabel);

			var edgeInsets = Control.TextContainerInset;
			var lineFragmentPadding = Control.TextContainer.LineFragmentPadding;

			var vConstraints = NSLayoutConstraint.FromVisualFormat(
			"V:|-" + edgeInsets.Top + "-[_placeholderLabel]-" + edgeInsets.Bottom + "-|", 0, new NSDictionary(),
			NSDictionary.FromObjectsAndKeys(
				new NSObject[] { _placeholderLabel }, new NSObject[] { new NSString("_placeholderLabel") })
		);

			var hConstraints = NSLayoutConstraint.FromVisualFormat(
				"H:|-" + lineFragmentPadding + "-[_placeholderLabel]-" + lineFragmentPadding + "-|",
				0, new NSDictionary(),
				NSDictionary.FromObjectsAndKeys(
					new NSObject[] { _placeholderLabel }, new NSObject[] { new NSString("_placeholderLabel") })
			);

			_placeholderLabel.TranslatesAutoresizingMaskIntoConstraints = false;

			Control.AddConstraints(hConstraints);
			Control.AddConstraints(vConstraints);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Editor.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
				UpdateKeyboard();
			else if (e.PropertyName == Xamarin.Forms.InputView.IsSpellCheckEnabledProperty.PropertyName)
				UpdateKeyboard();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateEditable();
			else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateTextAlignment();
			else if (e.PropertyName == Xamarin.Forms.InputView.MaxLengthProperty.PropertyName)
				UpdateMaxLength();
			else if (e.PropertyName == Editor.PlaceholderProperty.PropertyName)
				UpdatePlaceholderText();
			else if (e.PropertyName == Editor.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
		}

		void HandleChanged(object sender, EventArgs e)
		{
			ElementController.SetValueFromRenderer(Editor.TextProperty, Control.Text);
		}

		void OnEnded(object sender, EventArgs eventArgs)
		{
			if (Control.Text != Element.Text)
				ElementController.SetValueFromRenderer(Editor.TextProperty, Control.Text);

			Element.SetValue(VisualElement.IsFocusedPropertyKey, false);
			ElementController.SendCompleted();
		}

		void OnStarted(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void UpdateEditable()
		{
			Control.Editable = Element.IsEnabled;
			Control.UserInteractionEnabled = Element.IsEnabled;

			if (Control.InputAccessoryView != null)
				Control.InputAccessoryView.Hidden = !Element.IsEnabled;
		}

		void UpdateFont()
		{
			Control.Font = Element.ToUIFont();
		}

		void UpdateKeyboard()
		{
			Control.ApplyKeyboard(Element.Keyboard);
			if (!(Element.Keyboard is Internals.CustomKeyboard) && Element.IsSet(Xamarin.Forms.InputView.IsSpellCheckEnabledProperty))
			{
				if (!Element.IsSpellCheckEnabled)
				{
					Control.SpellCheckingType = UITextSpellCheckingType.No;
				}
			}
			Control.ReloadInputViews();
		}

		void UpdateText()
		{
			if (Control.Text != Element.Text)
			{
				Control.Text = Element.Text;
			}
			_placeholderLabel.Hidden = !string.IsNullOrEmpty(Control.Text);
		}

		void UpdatePlaceholderText()
		{
			_placeholderLabel.Text = Element.Placeholder;
		}

		void UpdatePlaceholderColor()
		{
			if (Element.PlaceholderColor == Color.Default)
				_placeholderLabel.TextColor = UIColor.DarkGray;
			else
				_placeholderLabel.TextColor = Element.PlaceholderColor.ToUIColor();
		}

		void UpdateTextAlignment()
		{
			Control.UpdateTextAlignment(Element);
		}

		void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault)
				Control.TextColor = UIColor.Black;
			else
				Control.TextColor = textColor.ToUIColor();
		}

		void UpdateMaxLength()
		{
			var currentControlText = Control.Text;

			if (currentControlText.Length > Element.MaxLength)
				Control.Text = currentControlText.Substring(0, Element.MaxLength);
		}

		bool ShouldChangeText(UITextView textView, NSRange range, string text)
		{
			var newLength = textView.Text.Length + text.Length - range.Length;
			return newLength <= Element.MaxLength;
		}
	}
}