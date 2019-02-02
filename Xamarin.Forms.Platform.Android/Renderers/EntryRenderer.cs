using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Platform.Android
{
	public class EntryRenderer : EntryRendererBase<FormsEditText>
	{
		public EntryRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use EntryRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EntryRenderer()
		{
			AutoPackage = false;
		}

		protected override FormsEditText CreateNativeControl()
		{
			return new FormsEditText(Context);
		}

		protected override EditText EditText => Control;

		protected override void UpdateIsReadOnly()
		{
			base.UpdateIsReadOnly();
			bool isReadOnly = !Element.IsReadOnly;
			Control.SetCursorVisible(isReadOnly);
		}
	}

	public abstract class EntryRendererBase<TControl> : ViewRenderer<Entry, TControl>, ITextWatcher, TextView.IOnEditorActionListener
		where TControl : global::Android.Views.View
	{
		TextColorSwitcher _hintColorSwitcher;
		TextColorSwitcher _textColorSwitcher;
		bool _disposed;
		ImeAction _currentInputImeFlag;
		IElementController ElementController => Element as IElementController;

		bool _cursorPositionChangePending;
		bool _selectionLengthChangePending;
		bool _nativeSelectionIsUpdating;


		protected abstract EditText EditText { get; }


		public EntryRendererBase(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use EntryRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal EntryRendererBase()
		{
			AutoPackage = false;
		}

		bool TextView.IOnEditorActionListener.OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
		{
			// Fire Completed and dismiss keyboard for hardware / physical keyboards
			if (actionId == ImeAction.Done || actionId == _currentInputImeFlag || (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter && e.Action == KeyEventActions.Up))
			{
				Control.ClearFocus();
				v.HideKeyboard();
				((IEntryController)Element).SendCompleted();
			}

			return true;
		}

		void ITextWatcher.AfterTextChanged(IEditable s)
		{
		}

		void ITextWatcher.BeforeTextChanged(ICharSequence s, int start, int count, int after)
		{
		}

		void ITextWatcher.OnTextChanged(ICharSequence s, int start, int before, int count)
		{
			if (string.IsNullOrEmpty(Element.Text) && s.Length() == 0)
				return;

			((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
		}

		protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			if (!e.Focus)
			{
				Control.HideKeyboard();
			}

			base.OnFocusChangeRequested(sender, e);

			if (e.Focus)
			{
				// Post this to the main looper queue so it doesn't happen until the other focus stuff has resolved
				// Otherwise, ShowKeyboard will be called before this control is truly focused, and we will potentially
				// be displaying the wrong keyboard
				Control?.PostShowKeyboard();
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				SetNativeControl(CreateNativeControl());

				EditText.AddTextChangedListener(this);
				EditText.SetOnEditorActionListener(this);

				if (EditText is IFormsEditText formsEditText)
				{
					formsEditText.OnKeyboardBackPressed += OnKeyboardBackPressed;
					formsEditText.SelectionChanged += SelectionChanged;
				}

				var useLegacyColorManagement = e.NewElement.UseLegacyColorManagement();

				_textColorSwitcher = new TextColorSwitcher(EditText.TextColors, useLegacyColorManagement);
				_hintColorSwitcher = new TextColorSwitcher(EditText.HintTextColors, useLegacyColorManagement);
			}

			// When we set the control text, it triggers the SelectionChanged event, which updates CursorPosition and SelectionLength;
			// These one-time-use variables will let us initialize a CursorPosition and SelectionLength via ctor/xaml/etc.
			_cursorPositionChangePending = Element.IsSet(Entry.CursorPositionProperty);
			_selectionLengthChangePending = Element.IsSet(Entry.SelectionLengthProperty);

			UpdatePlaceHolderText();
			EditText.Text = Element.Text;
			UpdateInputType();

			UpdateColor();
			UpdateAlignment();
			UpdateFont();
			UpdatePlaceholderColor();
			UpdateMaxLength();
			UpdateImeOptions();
			UpdateReturnType();
			UpdateIsReadOnly();

			if (_cursorPositionChangePending || _selectionLengthChangePending)
				UpdateCursorSelection();
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				if (Control != null && Control is IFormsEditText formsEditContext)
				{
					formsEditContext.OnKeyboardBackPressed -= OnKeyboardBackPressed;
					formsEditContext.SelectionChanged -= SelectionChanged;
				}
			}

			base.Dispose(disposing);
		}


		internal protected virtual void UpdatePlaceHolderText() => EditText.Hint = Element.Placeholder;

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
				EditText.Hint = Element.Placeholder;
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
				UpdateInputType();
			else if (e.PropertyName == Entry.TextProperty.PropertyName)
			{
				if (EditText.Text != Element.Text)
				{
					EditText.Text = Element.Text;
					if (Control.IsFocused)
					{
						EditText.SetSelection(EditText.Text.Length);
						Control.ShowKeyboard();
					}
				}
			}
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
				UpdateInputType();
			else if (e.PropertyName == InputView.IsSpellCheckEnabledProperty.PropertyName)
				UpdateInputType();
			else if (e.PropertyName == Entry.IsTextPredictionEnabledProperty.PropertyName)
				UpdateInputType();
			else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == InputView.MaxLengthProperty.PropertyName)
				UpdateMaxLength();
			else if (e.PropertyName == PlatformConfiguration.AndroidSpecific.Entry.ImeOptionsProperty.PropertyName)
				UpdateImeOptions();
			else if (e.PropertyName == Entry.ReturnTypeProperty.PropertyName)
				UpdateReturnType();
			else if (e.PropertyName == Entry.SelectionLengthProperty.PropertyName)
				UpdateCursorSelection();
			else if (e.PropertyName == Entry.CursorPositionProperty.PropertyName)
				UpdateCursorSelection();
			else if (e.PropertyName == InputView.IsReadOnlyProperty.PropertyName)
				UpdateIsReadOnly();

			base.OnElementPropertyChanged(sender, e);
		}

		protected virtual NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
		{
			// Override this in a custom renderer to use a different NumberKeyListener
			// or to filter out input types you don't want to allow
			// (e.g., inputTypes &= ~InputTypes.NumberFlagSigned to disallow the sign)
			return LocalizedDigitsKeyListener.Create(inputTypes);
		}

		protected virtual void UpdateImeOptions()
		{
			if (Element == null || Control == null)
				return;
			var imeOptions = Element.OnThisPlatform().ImeOptions();
			_currentInputImeFlag = imeOptions.ToAndroidImeOptions();
			EditText.ImeOptions = _currentInputImeFlag;
		}

		void UpdateAlignment()
		{
			EditText.UpdateHorizontalAlignment(Element.HorizontalTextAlignment, Context.HasRtlSupport());
		}

		internal protected virtual void UpdateColor() => UpdateTextColor(Element.TextColor);

		internal protected void UpdateTextColor(Color color) => _textColorSwitcher.UpdateTextColor(EditText, color);

		protected internal virtual void UpdateFont()
		{
			EditText.Typeface = Element.ToTypeface();
			EditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
		}

		void UpdateInputType()
		{
			Entry model = Element;
			var keyboard = model.Keyboard;

			EditText.InputType = keyboard.ToInputType();
			if (!(keyboard is Internals.CustomKeyboard))
			{
				if (model.IsSet(InputView.IsSpellCheckEnabledProperty))
				{
					if ((EditText.InputType & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
					{
						if (!model.IsSpellCheckEnabled)
							EditText.InputType = EditText.InputType | InputTypes.TextFlagNoSuggestions;
					}
				}
				if (model.IsSet(Entry.IsTextPredictionEnabledProperty))
				{
					if ((EditText.InputType & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
					{
						if (!model.IsTextPredictionEnabled)
							EditText.InputType = EditText.InputType | InputTypes.TextFlagNoSuggestions;
					}
				}
			}

			if (keyboard == Keyboard.Numeric)
			{
				EditText.KeyListener = GetDigitsKeyListener(EditText.InputType);
			}

			if (model.IsPassword && ((EditText.InputType & InputTypes.ClassText) == InputTypes.ClassText))
				EditText.InputType = EditText.InputType | InputTypes.TextVariationPassword;
			if (model.IsPassword && ((EditText.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber))
				EditText.InputType = EditText.InputType | InputTypes.NumberVariationPassword;

			UpdateFont();
		}

		protected internal virtual void UpdatePlaceholderColor()
		{
			_hintColorSwitcher.UpdateTextColor(EditText, Element.PlaceholderColor, EditText.SetHintTextColor);
		}

		void OnKeyboardBackPressed(object sender, EventArgs eventArgs)
		{
			Control?.ClearFocus();
		}

		void UpdateMaxLength()
		{
			var currentFilters = new List<IInputFilter>(EditText?.GetFilters() ?? new IInputFilter[0]);

			for (var i = 0; i < currentFilters.Count; i++)
			{
				if (currentFilters[i] is InputFilterLengthFilter)
				{
					currentFilters.RemoveAt(i);
					break;
				}
			}

			currentFilters.Add(new InputFilterLengthFilter(Element.MaxLength));

			EditText?.SetFilters(currentFilters.ToArray());

			var currentControlText = EditText?.Text;

			if (currentControlText.Length > Element.MaxLength)
				EditText.Text = currentControlText.Substring(0, Element.MaxLength);
		}

		void UpdateReturnType()
		{
			if (Control == null || Element == null)
				return;

			EditText.ImeOptions = Element.ReturnType.ToAndroidImeAction();
			_currentInputImeFlag = EditText.ImeOptions;
		}

		void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			int cursorPosition = Element.CursorPosition;
			int selectionStart = EditText.SelectionStart;

			if (!_cursorPositionChangePending)
			{
				var start = cursorPosition;

				if (selectionStart != start)
					SetCursorPositionFromRenderer(selectionStart);
			}

			if (!_selectionLengthChangePending)
			{
				int elementSelectionLength = System.Math.Min(EditText.Text.Length - cursorPosition, Element.SelectionLength);

				var controlSelectionLength = EditText.SelectionEnd - selectionStart;
				if (controlSelectionLength != elementSelectionLength)
					SetSelectionLengthFromRenderer(controlSelectionLength);
			}
		}

		void UpdateCursorSelection()
		{
			if (_nativeSelectionIsUpdating || Control == null || Element == null)
				return;

			if (!Element.IsReadOnly && Control.RequestFocus())
			{
				try
				{
					int start = GetSelectionStart();
					int end = GetSelectionEnd(start);

					EditText.SetSelection(start, end);
				}
				catch (System.Exception ex)
				{
					Internals.Log.Warning("Entry", $"Failed to set Control.Selection from CursorPosition/SelectionLength: {ex}");
				}
				finally
				{
					_cursorPositionChangePending = _selectionLengthChangePending = false;
				}
			}
		}

		int GetSelectionEnd(int start)
		{
			int end = start;
			int selectionLength = Element.SelectionLength;

			if (Element.IsSet(Entry.SelectionLengthProperty))
				end = System.Math.Max(start, System.Math.Min(EditText.Length(), start + selectionLength));

			int newSelectionLength = System.Math.Max(0, end - start);
			if (newSelectionLength != selectionLength)
				SetSelectionLengthFromRenderer(newSelectionLength);

			return end;
		}

		int GetSelectionStart()
		{
			int start = EditText.Length();
			int cursorPosition = Element.CursorPosition;

			if (Element.IsSet(Entry.CursorPositionProperty))
				start = System.Math.Min(EditText.Text.Length, cursorPosition);

			if (start != cursorPosition)
				SetCursorPositionFromRenderer(start);

			return start;
		}

		void SetCursorPositionFromRenderer(int start)
		{
			try
			{
				_nativeSelectionIsUpdating = true;
				ElementController?.SetValueFromRenderer(Entry.CursorPositionProperty, start);
			}
			catch (System.Exception ex)
			{
				Internals.Log.Warning("Entry", $"Failed to set CursorPosition from renderer: {ex}");
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
				ElementController?.SetValueFromRenderer(Entry.SelectionLengthProperty, selectionLength);
			}
			catch (System.Exception ex)
			{
				Internals.Log.Warning("Entry", $"Failed to set SelectionLength from renderer: {ex}");
			}
			finally
			{
				_nativeSelectionIsUpdating = false;
			}
		}

		protected virtual void UpdateIsReadOnly()
		{
			bool isReadOnly = !Element.IsReadOnly;

			Control.FocusableInTouchMode = isReadOnly;
			Control.Focusable = isReadOnly;
		}
	}
}
