using System.ComponentModel;
using Android.Content.Res;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace Xamarin.Forms.Platform.Android
{
	public class EditorRenderer : ViewRenderer<Editor, EditorEditText>, ITextWatcher
	{
		ColorStateList _defaultColors;

		public EditorRenderer()
		{
			AutoPackage = false;
		}

        IEditorController ElementController => Element;

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

			if (Element.Text != s.ToString())
				((IElementController)Element).SetValueFromRenderer(Editor.TextProperty, s.ToString());
		}

		protected override EditorEditText CreateNativeControl()
		{
			return new EditorEditText(Context);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			HandleKeyboardOnFocus = true;

			EditorEditText edit = Control;
			if (edit == null)
			{
				edit = CreateNativeControl();

				SetNativeControl(edit);
				edit.AddTextChangedListener(this);
				edit.OnBackKeyboardPressed += (sender, args) =>
				{
                    ElementController.SendCompleted();
					edit.ClearFocus();
				};
			}

			edit.SetSingleLine(false);
			edit.Gravity = GravityFlags.Top;
			edit.SetHorizontallyScrolling(false);

			UpdateText();
			UpdateInputType();
			UpdateTextColor();
			UpdateFont();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Editor.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
				UpdateInputType();
			else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == Editor.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Editor.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Editor.FontSizeProperty.PropertyName)
				UpdateFont();

			base.OnElementPropertyChanged(sender, e);
		}

		protected virtual NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
		{
			// Override this in a custom renderer to use a different NumberKeyListener 
			// or to filter out input types you don't want to allow 
			// (e.g., inputTypes &= ~InputTypes.NumberFlagSigned to disallow the sign)
			return LocalizedDigitsKeyListener.Create(inputTypes);
		}

		internal override void OnNativeFocusChanged(bool hasFocus)
		{
			if (Element.IsFocused && !hasFocus) // Editor has requested an unfocus, fire completed event
                ElementController.SendCompleted();
		}

		void UpdateFont()
		{
			Control.Typeface = Element.ToTypeface();
			Control.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
		}

		void UpdateInputType()
		{
			Editor model = Element;
			EditorEditText edit = Control;
			var keyboard = model.Keyboard;

			edit.InputType = keyboard.ToInputType() | InputTypes.TextFlagMultiLine;

			if (keyboard == Keyboard.Numeric)
			{
				edit.KeyListener = GetDigitsKeyListener(edit.InputType);
			}
		}

		void UpdateText()
		{
			string newText = Element.Text ?? "";

			if (Control.Text == newText)
				return;

			Control.Text = newText;
			Control.SetSelection(newText.Length);
		}

		void UpdateTextColor()
		{
			if (Element.TextColor.IsDefault)
			{
				if (_defaultColors == null)
				{
					// This control has always had the default colors; nothing to update
					return;
				}

				// This control is being set back to the default colors
				Control.SetTextColor(_defaultColors);
			}
			else
			{
				if (_defaultColors == null)
				{
					// Keep track of the default colors so we can return to them later
					// and so we can preserve the default disabled color
					_defaultColors = Control.TextColors;
				}

				Control.SetTextColor(Element.TextColor.ToAndroidPreserveDisabled(_defaultColors));
			}
		}
	}
}