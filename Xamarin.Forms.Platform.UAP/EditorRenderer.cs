using System;
using System.ComponentModel;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using WBrush = Windows.UI.Xaml.Media.Brush;
using Specifics = Xamarin.Forms.PlatformConfiguration.WindowsSpecific.InputView;

namespace Xamarin.Forms.Platform.UWP
{
	public class EditorRenderer : ViewRenderer<Editor, FormsTextBox>
	{
		bool _fontApplied;
		WBrush _backgroundColorFocusedDefaultBrush;
		WBrush _textDefaultBrush;
		WBrush _defaultTextColorFocusBrush;
		WBrush _defaultPlaceholderColorFocusBrush;
		WBrush _placeholderDefaultBrush;
		string _transformedText;
    bool _cursorPositionChangePending;
		bool _selectionLengthChangePending;
		bool _nativeSelectionIsUpdating;

		IEditorController ElementController => Element;


		FormsTextBox CreateTextBox()
		{
			return new FormsTextBox
			{
				AcceptsReturn = true,
				TextWrapping = TextWrapping.Wrap,
				Style = Windows.UI.Xaml.Application.Current.Resources["FormsTextBoxStyle"] as Windows.UI.Xaml.Style,
				VerticalContentAlignment = VerticalAlignment.Top,
				UpdateVerticalAlignmentOnLoad = false
			};
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var textBox = CreateTextBox();

					SetNativeControl(textBox);

					textBox.TextChanged += OnNativeTextChanged;
					textBox.LostFocus += OnLostFocus;
					textBox.SelectionChanged += SelectionChanged;

					// If the Forms VisualStateManager is in play or the user wants to disable the Forms legacy
					// color stuff, then the underlying textbox should just use the Forms VSM states
					textBox.UseFormsVsm = e.NewElement.HasVisualStateGroups()
						|| !e.NewElement.OnThisPlatform().GetIsLegacyColorModeEnabled();

					// The default is DetectFromContent, which we don't want because it can
					// override the FlowDirection settings. 
					textBox.TextAlignment = Windows.UI.Xaml.TextAlignment.Left;
				}

				// When we set the control text, it triggers the SelectionChanged event, which updates CursorPosition and SelectionLength;
				// These one-time-use variables will let us initialize a CursorPosition and SelectionLength via ctor/xaml/etc.
				_cursorPositionChangePending = Element.IsSet(Editor.CursorPositionProperty);
				_selectionLengthChangePending = Element.IsSet(Editor.SelectionLengthProperty);

				UpdateText();
				UpdateInputScope();
				UpdateTextColor();
				UpdateBackground();
				UpdateCharacterSpacing();
				UpdateFont();
				UpdateFlowDirection();
				UpdateMaxLength();
				UpdateDetectReadingOrderFromContent();
				UpdatePlaceholderText();
				UpdatePlaceholderColor();
				UpdateIsReadOnly();

				if (_cursorPositionChangePending)
					UpdateCursorPosition();

				if (_selectionLengthChangePending)
					UpdateSelectionLength();
			}

			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Control != null)
			{
				Control.TextChanged -= OnNativeTextChanged;
				Control.LostFocus -= OnLostFocus;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Editor.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
			{
				UpdateInputScope();
			}
			else if (e.PropertyName == InputView.IsSpellCheckEnabledProperty.PropertyName)
			{
				UpdateInputScope();
			}
			else if (e.PropertyName == Editor.IsTextPredictionEnabledProperty.PropertyName)
			{
				UpdateInputScope();
			}
			else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}
			else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
			{
				UpdateFont();
			}
			else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
			{
				UpdateFont();
			}
			else if (e.IsOneOf(Editor.TextProperty, Editor.TextTransformProperty))
			{
				UpdateText();
			}
			else if (e.PropertyName == Editor.CharacterSpacingProperty.PropertyName)
			{
				UpdateCharacterSpacing();
			}
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
			{
				UpdateFlowDirection();
			}
			else if (e.PropertyName == InputView.MaxLengthProperty.PropertyName)
				UpdateMaxLength();
			else if (e.PropertyName == Specifics.DetectReadingOrderFromContentProperty.PropertyName)
				UpdateDetectReadingOrderFromContent();
			else if (e.PropertyName == Editor.PlaceholderProperty.PropertyName)
				UpdatePlaceholderText();
			else if (e.PropertyName == Editor.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
			else if (e.PropertyName == InputView.IsReadOnlyProperty.PropertyName)
				UpdateIsReadOnly();
			else if(e.PropertyName == Editor.CursorPositionProperty.PropertyName)
				UpdateCursorPosition();
			else if(e.PropertyName == Editor.SelectionLengthProperty.PropertyName)
				UpdateSelectionLength();
		}

		protected override void UpdateBackground()
		{
			base.UpdateBackground();

			if (Control == null)
			{
				return;
			}

			BrushHelpers.UpdateBrush(Element.Background, ref _backgroundColorFocusedDefaultBrush,
			   () => Control.BackgroundFocusBrush, brush => Control.BackgroundFocusBrush = brush);
		}

		void OnLostFocus(object sender, RoutedEventArgs e)
		{
			ElementController.SendCompleted();
		}

		void UpdatePlaceholderText()
		{
			Control.PlaceholderText = Element.Placeholder ?? "";
		}

		void UpdatePlaceholderColor()
		{
			Color placeholderColor = Element.PlaceholderColor;

			BrushHelpers.UpdateColor(placeholderColor, ref _placeholderDefaultBrush,
				() => Control.PlaceholderForegroundBrush, brush => Control.PlaceholderForegroundBrush = brush);

			BrushHelpers.UpdateColor(placeholderColor, ref _defaultPlaceholderColorFocusBrush,
				() => Control.PlaceholderForegroundFocusBrush, brush => Control.PlaceholderForegroundFocusBrush = brush);
		}

		protected override void UpdateBackgroundColor()
		{
			base.UpdateBackgroundColor();

			if (Control == null)
			{
				return;
			}

			// By default some platforms have alternate default background colors when focused
			BrushHelpers.UpdateColor(Element.BackgroundColor, ref _backgroundColorFocusedDefaultBrush,
				() => Control.BackgroundFocusBrush, brush => Control.BackgroundFocusBrush = brush);
		}

		void OnNativeTextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs args)
		{
			_transformedText = Element.UpdateFormsText(Control.Text, Element.TextTransform);
			Element.SetValueCore(Editor.TextProperty, _transformedText);
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			FormsTextBox child = Control;

			if (Children.Count == 0 || child == null)
				return new SizeRequest();

			var constraint = new Windows.Foundation.Size(widthConstraint, heightConstraint);
			child.Measure(constraint);
			var result = FormsTextBox.GetCopyOfSize(child, constraint);
			return new SizeRequest(result);
		}

		void UpdateFont()
		{
			if (Control == null)
				return;

			Editor editor = Element;

			if (editor == null)
				return;

			bool editorIsDefault = editor.FontFamily == null &&
								   editor.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Editor), true) &&
								   editor.FontAttributes == FontAttributes.None;

			if (editorIsDefault && !_fontApplied)
				return;

			if (editorIsDefault)
			{
				// ReSharper disable AccessToStaticMemberViaDerivedType
				// Resharper wants to simplify 'TextBox' to 'Control', but then it'll conflict with the property 'Control'
				Control.ClearValue(TextBox.FontStyleProperty);
				Control.ClearValue(TextBox.FontSizeProperty);
				Control.ClearValue(TextBox.FontFamilyProperty);
				Control.ClearValue(TextBox.FontWeightProperty);
				Control.ClearValue(TextBox.FontStretchProperty);
				// ReSharper restore AccessToStaticMemberViaDerivedType
			}
			else
			{
				Control.ApplyFont(editor);
			}

			_fontApplied = true;
		}

		void UpdateInputScope()
		{
			Editor editor = Element;
			var custom = editor.Keyboard as CustomKeyboard;
			if (custom != null)
			{
				Control.IsTextPredictionEnabled = (custom.Flags & KeyboardFlags.Suggestions) != 0;
				Control.IsSpellCheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) != 0;
			}
			else
			{
				if (editor.IsSet(Editor.IsTextPredictionEnabledProperty))
					Control.IsTextPredictionEnabled = editor.IsTextPredictionEnabled;
				else
					Control.ClearValue(TextBox.IsTextPredictionEnabledProperty);
				if (editor.IsSet(InputView.IsSpellCheckEnabledProperty))
					Control.IsSpellCheckEnabled = editor.IsSpellCheckEnabled;
				else
					Control.ClearValue(TextBox.IsSpellCheckEnabledProperty);
			}

			Control.InputScope = editor.Keyboard.ToInputScope();
		}

		void UpdateCharacterSpacing()
		{
			Control.CharacterSpacing = Element.CharacterSpacing.ToEm();
		}
	
		void UpdateText()
		{
			string newText = _transformedText = Element.UpdateFormsText(Element.Text, Element.TextTransform);

			if (Control.Text == newText)
			{
				return;
			}

			Control.Text = newText;
			Control.SelectionStart = Control.Text.Length;
		}

		void UpdateTextColor()
		{
			Color textColor = Element.TextColor;

			BrushHelpers.UpdateColor(textColor, ref _textDefaultBrush,
				() => Control.Foreground, brush => Control.Foreground = brush);

			BrushHelpers.UpdateColor(textColor, ref _defaultTextColorFocusBrush,
				() => Control.ForegroundFocusBrush, brush => Control.ForegroundFocusBrush = brush);
		}

		void UpdateFlowDirection()
		{
			Control.UpdateFlowDirection(Element);
		}

		void UpdateMaxLength()
		{
			Control.MaxLength = Element.MaxLength;

			var currentControlText = Control.Text;

			if (currentControlText.Length > Element.MaxLength)
				Control.Text = currentControlText.Substring(0, Element.MaxLength);
		}

		void UpdateDetectReadingOrderFromContent()
		{
			if (Element.IsSet(Specifics.DetectReadingOrderFromContentProperty))
			{
				if (Element.OnThisPlatform().GetDetectReadingOrderFromContent())
				{
					Control.TextReadingOrder = TextReadingOrder.DetectFromContent;
				}
				else
				{
					Control.TextReadingOrder = TextReadingOrder.UseFlowDirection;
				}
			}
		}

		void UpdateIsReadOnly()
		{
			Control.IsReadOnly = Element.IsReadOnly;
		}

		void SelectionChanged(object sender, RoutedEventArgs e)
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			int cursorPosition = Element.CursorPosition;

			if (!_cursorPositionChangePending)
			{
				var start = cursorPosition;
				int selectionStart = Control.SelectionStart;
				if (selectionStart != start)
					SetCursorPositionFromRenderer(selectionStart);
			}

			if (!_selectionLengthChangePending)
			{
				int elementSelectionLength = Math.Min(Control.Text.Length - cursorPosition, Element.SelectionLength);

				int controlSelectionLength = Control.SelectionLength;
				if (controlSelectionLength != elementSelectionLength)
					SetSelectionLengthFromRenderer(controlSelectionLength);
			}
		}

		void UpdateSelectionLength()
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			if (Control.Focus(FocusState.Programmatic))
			{
				try
				{
					int selectionLength = 0;
					int elemSelectionLength = Element.SelectionLength;

					if (Element.IsSet(Editor.SelectionLengthProperty))
						selectionLength = Math.Max(0, Math.Min(Control.Text.Length - Element.CursorPosition, elemSelectionLength));

					if (elemSelectionLength != selectionLength)
						SetSelectionLengthFromRenderer(selectionLength);

					Control.SelectionLength = selectionLength;
				}
				catch (Exception ex)
				{
					Log.Warning("Editor", $"Failed to set Control.SelectionLength from SelectionLength: {ex}");
				}
				finally
				{
					_selectionLengthChangePending = false;
				}
			}
		}

		void UpdateCursorPosition()
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			if (Control.Focus(FocusState.Programmatic))
			{
				try
				{
					int start = Control.Text.Length;
					int cursorPosition = Element.CursorPosition;

					if (Element.IsSet(Editor.CursorPositionProperty))
						start = Math.Min(start, cursorPosition);

					if (start != cursorPosition)
						SetCursorPositionFromRenderer(start);

					Control.SelectionStart = start;

					// Length is dependent on start, so we'll need to update it
					UpdateSelectionLength();
				}
				catch (Exception ex)
				{
					Log.Warning("Editor", $"Failed to set Control.SelectionStart from CursorPosition: {ex}");
				}
				finally
				{
					_cursorPositionChangePending = false;
				}
			}
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
				Log.Warning("Editor", $"Failed to set CursorPosition from renderer: {ex}");
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
				Log.Warning("Editor", $"Failed to set SelectionLength from renderer: {ex}");
			}
			finally
			{
				_nativeSelectionIsUpdating = false;
			}
		}
	}
}