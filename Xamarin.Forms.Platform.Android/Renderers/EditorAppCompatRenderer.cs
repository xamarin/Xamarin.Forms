using System;
using System.ComponentModel;
using Android.Content;
using Android.Views.InputMethods;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class EditorAppCompatRenderer : EditorRendererBase<FormsAppCompatEditText>
	{
		TextColorSwitcher _hintColorSwitcher;
		TextColorSwitcher _textColorSwitcher;

		public EditorAppCompatRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use EditorRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EditorAppCompatRenderer()
		{
			AutoPackage = false;
		}

		protected override FormsAppCompatEditText CreateNativeControl()
		{
			return new FormsAppCompatEditText(Context)
			{
				ImeOptions = ImeAction.Done
			};
		}

		protected override EditText EditText => Control;

		protected override void UpdatePlaceholderColor()
		{
			_hintColorSwitcher = _hintColorSwitcher ?? new TextColorSwitcher(EditText.HintTextColors, Element.UseLegacyColorManagement());
			_hintColorSwitcher.UpdateTextColor(EditText, Element.PlaceholderColor, EditText.SetHintTextColor);
		}

		protected override void UpdateTextColor()
		{
			_textColorSwitcher = _textColorSwitcher ?? new TextColorSwitcher(EditText.TextColors, Element.UseLegacyColorManagement());
			_textColorSwitcher.UpdateTextColor(EditText, Element.TextColor);
		}

		protected override void OnAttachedToWindow()
		{
			base.OnAttachedToWindow();

			if (EditText.IsAlive() && EditText.Enabled)
			{
				// https://issuetracker.google.com/issues/37095917
				EditText.Enabled = false;
				EditText.Enabled = true;
			}
		}
	}
}