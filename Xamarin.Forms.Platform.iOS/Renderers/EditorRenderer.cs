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
		// Using same placeholder color as for the Entry
		readonly UIColor _defaultPlaceholderColor = ColorExtensions.PlaceholderColor;

		UILabel _placeholderLabel;

		[Preserve(Conditional = true)]
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
			bool initializing = false;
			if (e.NewElement != null && _placeholderLabel == null)
			{
				initializing = true;
				// create label so it can get updated during the initial setup loop
				_placeholderLabel = new UILabel
				{
					BackgroundColor = UIColor.Clear,
					Frame = new RectangleF(0, 0, Frame.Width, Frame.Height),
					Lines = 0
				};
			}

			base.OnElementChanged(e);

			if (e.NewElement != null && initializing)
			{
				CreatePlaceholderLabel();
			}
		}

		protected internal override void UpdateFont()
		{
			base.UpdateFont();
			_placeholderLabel.Font = Element.ToUIFont();
		}

		protected internal override void UpdatePlaceholderText()
		{
			_placeholderLabel.Text = Element.Placeholder;

			_placeholderLabel.SizeToFit();
		}

		protected internal override void UpdateCharacterSpacing()
		{
			var textAttr = TextView.AttributedText.AddCharacterSpacing(Element.Text, Element.CharacterSpacing);

			if(textAttr != null)
				TextView.AttributedText = textAttr;

			var placeHolder = _placeholderLabel.AttributedText.AddCharacterSpacing(Element.Placeholder, Element.CharacterSpacing);

			if(placeHolder != null)
				_placeholderLabel.AttributedText = placeHolder;
		}

		protected internal override void UpdatePlaceholderColor()
		{
			Color placeholderColor = Element.PlaceholderColor;
			if (placeholderColor.IsDefault)
				_placeholderLabel.TextColor = _defaultPlaceholderColor;
			else
				_placeholderLabel.TextColor = placeholderColor.ToUIColor();
		}

		void CreatePlaceholderLabel()
		{
			if (Control == null)
			{
				return;
			}

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

			_placeholderLabel.TranslatesAutoresizingMaskIntoConstraints = true;
			_placeholderLabel.AttributedText = _placeholderLabel.AttributedText.AddCharacterSpacing(Element.Placeholder, Element.CharacterSpacing);

			Control.AddConstraints(hConstraints);
			Control.AddConstraints(vConstraints);
		}

	}

	public abstract class EditorRendererBase<TControl> : ViewRenderer<Editor, TControl>
		where TControl : UIView
	{
		bool _disposed;
		IUITextViewDelegate _pleaseDontCollectMeGarbageCollector;

		IEditorController ElementController => Element;
		protected abstract UITextView TextView { get; }

		IDisposable _selectedTextRangeObserver;
		bool _nativeSelectionIsUpdating;

		bool _cursorPositionChangePending;
		bool _selectionLengthChangePending;

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
					_selectedTextRangeObserver?.Dispose();
				}
			}

			_pleaseDontCollectMeGarbageCollector = null;
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
				_selectedTextRangeObserver = TextView.AddObserver("selectedTextRange", NSKeyValueObservingOptions.New, UpdateCursorFromControl);
				_pleaseDontCollectMeGarbageCollector = TextView.Delegate;
			}

			// When we set the control text, it triggers the UpdateCursorFromControl event, which updates CursorPosition and SelectionLength;
			// These one-time-use variables will let us initialize a CursorPosition and SelectionLength via ctor/xaml/etc.
			_cursorPositionChangePending = Element.IsSet(Editor.CursorPositionProperty);
			_selectionLengthChangePending = Element.IsSet(Editor.SelectionLengthProperty);

			UpdateFont();
			UpdatePlaceholderText();
			UpdatePlaceholderColor();
			UpdateTextColor();
			UpdateText();
			UpdateCharacterSpacing();
			UpdateKeyboard();
			UpdateEditable();
			UpdateTextAlignment();
			UpdateMaxLength();
			UpdateAutoSizeOption();
			UpdateReadOnly();
			UpdateUserInteraction();

			if (_cursorPositionChangePending || _selectionLengthChangePending)
				UpdateCursorSelection();
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

			if (e.IsOneOf(Editor.TextProperty, Editor.TextTransformProperty))
			{
				UpdateText();
				UpdateCharacterSpacing();
			}
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
			else if (e.PropertyName == Editor.CharacterSpacingProperty.PropertyName)
				UpdateCharacterSpacing();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateTextAlignment();
			else if (e.PropertyName == Xamarin.Forms.InputView.MaxLengthProperty.PropertyName)
				UpdateMaxLength();
			else if (e.PropertyName == Editor.PlaceholderProperty.PropertyName)
			{
				UpdatePlaceholderText();
				UpdateCharacterSpacing();
			}
			else if (e.PropertyName == Editor.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
			else if (e.PropertyName == Editor.AutoSizeProperty.PropertyName)
				UpdateAutoSizeOption();
			else if (e.PropertyName == Editor.CursorPositionProperty.PropertyName ||
			         e.PropertyName == Editor.SelectionLengthProperty.PropertyName)
				UpdateCursorSelection();
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
			var text = Element.UpdateFormsText(Element.Text, Element.TextTransform);
			if (TextView.Text != text)
			{
				TextView.Text = text;
			}
		}

		protected internal abstract void UpdatePlaceholderText();
		protected internal abstract void UpdatePlaceholderColor();
		protected internal abstract void UpdateCharacterSpacing();

		void UpdateTextAlignment()
		{
			TextView.UpdateTextAlignment(Element);
		}

		protected internal virtual void UpdateTextColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault)
				TextView.TextColor = ColorExtensions.LabelColor;
			else
				TextView.TextColor = textColor.ToUIColor();
		}

		void UpdateMaxLength()
		{
			var currentControlText = TextView.Text;

			if (currentControlText.Length > Element.MaxLength)
				TextView.Text = currentControlText.Substring(0, Element.MaxLength);
		}

		protected virtual bool ShouldChangeText(UITextView textView, NSRange range, string text)
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

		void UpdateCursorFromControl(NSObservedChange obj)
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			var currentSelection = TextView.SelectedTextRange;
			if (currentSelection != null)
			{
				if (!_cursorPositionChangePending)
				{
					int newCursorPosition = (int)TextView.GetOffsetFromPosition(TextView.BeginningOfDocument, currentSelection.Start);
					if (newCursorPosition != Element.CursorPosition)
						SetCursorPositionFromRenderer(newCursorPosition);
				}

				if (!_selectionLengthChangePending)
				{
					int selectionLength = (int)TextView.GetOffsetFromPosition(currentSelection.Start, currentSelection.End);

					if (selectionLength != Element.SelectionLength)
						SetSelectionLengthFromRenderer(selectionLength);
				}
			}
		}

		void UpdateCursorSelection()
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			_cursorPositionChangePending = _selectionLengthChangePending = true;

			// If this is run from the ctor, the control is likely too early in its lifecycle to be first responder yet. 
			// Anything done here will have no effect, so we'll skip this work until later.
			// We'll try again when the control does become first responder later OnEditingBegan
			if (Control.BecomeFirstResponder())
			{
				try
				{
					int cursorPosition = Element.CursorPosition;

					UITextPosition start = GetSelectionStart(cursorPosition, out int startOffset);
					UITextPosition end = GetSelectionEnd(cursorPosition, start, startOffset);

					TextView.SelectedTextRange = TextView.GetTextRange(start, end);
				}
				catch (Exception ex)
				{
					Internals.Log.Warning("Editor", $"Failed to set Control.SelectedTextRange from CursorPosition/SelectionLength: {ex}");
				}
				finally
				{
					_cursorPositionChangePending = _selectionLengthChangePending = false;
				}
			}
		}

		UITextPosition GetSelectionEnd(int cursorPosition, UITextPosition start, int startOffset)
		{
			UITextPosition end = start;
			int endOffset = startOffset;
			int selectionLength = Element.SelectionLength;

			if (Element.IsSet(Editor.SelectionLengthProperty))
			{
				end = TextView.GetPosition(start, Math.Max(startOffset, Math.Min(TextView.Text.Length - cursorPosition, selectionLength))) ?? start;
				endOffset = Math.Max(startOffset, (int)TextView.GetOffsetFromPosition(TextView.BeginningOfDocument, end));
			}

			int newSelectionLength = Math.Max(0, endOffset - startOffset);
			if (newSelectionLength != selectionLength)
				SetSelectionLengthFromRenderer(newSelectionLength);

			return end;
		}

		UITextPosition GetSelectionStart(int cursorPosition, out int startOffset)
		{
			UITextPosition start = TextView.EndOfDocument;
			startOffset = TextView.Text.Length;

			if (Element.IsSet(Editor.CursorPositionProperty))
			{
				start = TextView.GetPosition(TextView.BeginningOfDocument, cursorPosition) ?? TextView.EndOfDocument;
				startOffset = Math.Max(0, (int)TextView.GetOffsetFromPosition(TextView.BeginningOfDocument, start));
			}

			if (startOffset != cursorPosition)
				SetCursorPositionFromRenderer(startOffset);

			return start;
		}

		void SetCursorPositionFromRenderer(int start)
		{
			try
			{
				_nativeSelectionIsUpdating = true;
				ElementController?.SetValueFromRenderer(Editor.CursorPositionProperty, start);
			}
			catch (Exception ex)
			{
				Internals.Log.Warning("Editor", $"Failed to set CursorPosition from renderer: {ex}");
			}
			finally
			{
				_nativeSelectionIsUpdating = false;
			}
		}

		void SetSelectionLengthFromRenderer(int selectionLength)
		{
			try
			{
				_nativeSelectionIsUpdating = true;
				ElementController?.SetValueFromRenderer(Editor.SelectionLengthProperty, selectionLength);
			}
			catch (Exception ex)
			{
				Internals.Log.Warning("Editor", $"Failed to set SelectionLength from renderer: {ex}");
			}
			finally
			{
				_nativeSelectionIsUpdating = false;
			}
		}

		internal class FormsUITextView : UITextView, IFormsUITextView
		{
			public event EventHandler FrameChanged;

			public FormsUITextView(RectangleF frame) : base(frame)
			{
			}


			public override RectangleF Frame
			{
				get => base.Frame;
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