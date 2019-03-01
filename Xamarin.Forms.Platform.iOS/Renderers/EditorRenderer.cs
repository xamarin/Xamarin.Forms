using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using RectangleF = CoreGraphics.CGRect;

namespace Xamarin.Forms.Platform.iOS
{
	public class EditorRenderer : EditorRendererBase<UITextView>
	{
		UILabel _placeholderLabel;

		public EditorRenderer()
		{
			Frame = new RectangleF(0, 20, 320, 40);
		}

		protected override UITextView CreateNativeControl()
		{
			return new FormsUITextView(RectangleF.Empty);
		}

		protected override UITextView TextView => Control;

		protected internal override void UpdateText()
		{
			base.UpdateText();
			_placeholderLabel.Hidden = !string.IsNullOrEmpty(TextView.Text);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			// create label so it can get updated during the initial setup loop
			_placeholderLabel = new UILabel
			{
				BackgroundColor = UIColor.Clear
			};

			base.OnElementChanged(e);

			CreatePlaceholderLabel();
		}

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			_placeholderLabel.Font = Element.ToUIFont();
		}

		protected internal override void UpdatePlaceholderText()
		{
			_placeholderLabel.Text = Element.Placeholder;
		}

		protected internal override void UpdatePlaceholderColor()
		{
			if (Element.PlaceholderColor == Color.Default)
				_placeholderLabel.TextColor = UIColor.DarkGray;
			else
				_placeholderLabel.TextColor = Element.PlaceholderColor.ToUIColor();
		}

		void CreatePlaceholderLabel()
		{
			Control.AddSubview(_placeholderLabel);

			var edgeInsets = TextView.TextContainerInset;
			var lineFragmentPadding = TextView.TextContainer.LineFragmentPadding;

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

	}

	public abstract class EditorRendererBase<TControl> : ViewRenderer<Editor, TControl>
		where TControl : UIView
	{
		bool _disposed;
		IEditorController ElementController => Element;
		protected abstract UITextView TextView { get; }

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (Control != null)
				{
					TextView.Changed -= HandleChanged;
					TextView.Started -= OnStarted;
					TextView.Ended -= OnEnded;
					TextView.ShouldChangeText -= ShouldChangeText;
					if(Control is IFormsUITextView formsUITextView)
						formsUITextView.FrameChanged -= OnFrameChanged;
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
				SetNativeControl(CreateNativeControl());

				if (Device.Idiom == TargetIdiom.Phone)
				{
					// iPhone does not have a dismiss keyboard button
					var keyboardWidth = UIScreen.MainScreen.Bounds.Width;
					var accessoryView = new UIToolbar(new RectangleF(0, 0, keyboardWidth, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };

					var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
					var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) =>
					{
						TextView.ResignFirstResponder();
						ElementController.SendCompleted();
					});
					accessoryView.SetItems(new[] { spacer, doneButton }, false);
					TextView.InputAccessoryView = accessoryView;
				}

				TextView.Changed += HandleChanged;
				TextView.Started += OnStarted;
				TextView.Ended += OnEnded;
				TextView.ShouldChangeText += ShouldChangeText;
			}

			UpdateFont();
			UpdatePlaceholderText();
			UpdatePlaceholderColor();
			UpdateTextColor();
			UpdateText();
			UpdateKeyboard();
			UpdateEditable();
			UpdateTextAlignment();
			UpdateMaxLength();
			UpdateAutoSizeOption();
			UpdateReadOnly();
			UpdateUserInteraction();
		}

		protected internal virtual void UpdateAutoSizeOption()
		{
			if (Control is IFormsUITextView textView)
			{
				textView.FrameChanged -= OnFrameChanged;
				if (Element.AutoSize == EditorAutoSizeOption.TextChanges)
					textView.FrameChanged += OnFrameChanged;
			}
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
			else if (e.PropertyName == Editor.IsTextPredictionEnabledProperty.PropertyName)
				UpdateKeyboard();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName || e.PropertyName == Xamarin.Forms.InputView.IsReadOnlyProperty.PropertyName)
				UpdateUserInteraction();
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
			else if (e.PropertyName == Editor.AutoSizeProperty.PropertyName)
				UpdateAutoSizeOption();
		}

		void HandleChanged(object sender, EventArgs e)
		{
			ElementController.SetValueFromRenderer(Editor.TextProperty, TextView.Text);
		}

		private void OnFrameChanged(object sender, EventArgs e)
		{
			// When a new line is added to the UITextView the resize happens after the view has already scrolled
			// This causes the view to reposition without the scroll. If TextChanges is enabled then the Frame
			// will resize until it can't anymore and thus it should never be scrolled until the Frame can't increase in size
			if (Element.AutoSize == EditorAutoSizeOption.TextChanges)
			{
				TextView.ScrollRangeToVisible(new NSRange(0, 0));
			}
		}

		void OnEnded(object sender, EventArgs eventArgs)
		{
			if (TextView.Text != Element.Text)
				ElementController.SetValueFromRenderer(Editor.TextProperty, TextView.Text);

			Element.SetValue(VisualElement.IsFocusedPropertyKey, false);
			ElementController.SendCompleted();
		}

		void OnStarted(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void UpdateEditable()
		{
			TextView.Editable = Element.IsEnabled;
			TextView.UserInteractionEnabled = Element.IsEnabled;

			if (TextView.InputAccessoryView != null)
				TextView.InputAccessoryView.Hidden = !Element.IsEnabled;
		}

		protected internal virtual void UpdateFont()
		{
			var font = Element.ToUIFont();
			TextView.Font = font;
		}

		void UpdateKeyboard()
		{
			var keyboard = Element.Keyboard;
			TextView.ApplyKeyboard(keyboard);
			if (!(keyboard is Internals.CustomKeyboard))
			{
				if (Element.IsSet(Xamarin.Forms.InputView.IsSpellCheckEnabledProperty))
				{
					if (!Element.IsSpellCheckEnabled)
					{
						TextView.SpellCheckingType = UITextSpellCheckingType.No;
					}
				}
				if (Element.IsSet(Editor.IsTextPredictionEnabledProperty))
				{
					if (!Element.IsTextPredictionEnabled)
					{
						TextView.AutocorrectionType = UITextAutocorrectionType.No;
					}
				}
			}
			TextView.ReloadInputViews();
		}

		protected internal virtual void UpdateText()
		{
			if (TextView.Text != Element.Text)
			{
				TextView.Text = Element.Text;
			}
		}

		protected internal abstract void UpdatePlaceholderText();
		protected internal abstract void UpdatePlaceholderColor();


		void UpdateTextAlignment()
		{
			TextView.UpdateTextAlignment(Element);
		}

		protected internal virtual void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault)
				TextView.TextColor = UIColor.Black;
			else
				TextView.TextColor = textColor.ToUIColor();
		}

		void UpdateMaxLength()
		{
			var currentControlText = TextView.Text;

			if (currentControlText.Length > Element.MaxLength)
				TextView.Text = currentControlText.Substring(0, Element.MaxLength);
		}

		bool ShouldChangeText(UITextView textView, NSRange range, string text)
		{
			var newLength = textView.Text.Length + text.Length - range.Length;
			return newLength <= Element.MaxLength;
		}

		void UpdateReadOnly()
		{
			TextView.UserInteractionEnabled = !Element.IsReadOnly;

			// Control and TextView might be different
			Control.UserInteractionEnabled = !Element.IsReadOnly;
		}

		void UpdateUserInteraction()
		{
			if (Element.IsEnabled && Element.IsReadOnly)
				UpdateReadOnly();
			else
				UpdateEditable();
		}

		internal class FormsUITextView : UITextView, IFormsUITextView
		{
			public event EventHandler FrameChanged;

			public FormsUITextView(RectangleF frame) : base(frame)
			{
			}


			public override RectangleF Frame
			{
				get
				{
					return base.Frame;
				}
				set
				{
					base.Frame = value;
					FrameChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}
	}

	internal interface IFormsUITextView
	{
		event EventHandler FrameChanged;
	}
}