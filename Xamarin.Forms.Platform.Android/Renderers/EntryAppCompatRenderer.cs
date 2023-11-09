using System;
using System.ComponentModel;
using Android.Content;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class EntryAppCompatRenderer : EntryRendererBase<FormsAppCompatEditText>
	{
		TextColorSwitcher _hintColorSwitcher;
		TextColorSwitcher _textColorSwitcher;

		public EntryAppCompatRenderer(Context context) : base(context)
		{
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use EntryRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EntryAppCompatRenderer()
		{
			AutoPackage = false;
		}

		protected override FormsAppCompatEditText CreateNativeControl()
		{
			return new FormsAppCompatEditText(Context);
		}

		protected override EditText EditText => Control;

		protected override void UpdateIsReadOnly()
		{
			base.UpdateIsReadOnly();
			bool isReadOnly = !Element.IsReadOnly;
			EditText.SetCursorVisible(isReadOnly);
		}

		protected override void UpdatePlaceholderColor()
		{
			_hintColorSwitcher = _hintColorSwitcher ?? new TextColorSwitcher(EditText.HintTextColors, Element.UseLegacyColorManagement());
			_hintColorSwitcher.UpdateTextColor(EditText, Element.PlaceholderColor, EditText.SetHintTextColor);
		}

		protected override void UpdateColor()
		{
			UpdateTextColor(Element.TextColor);
		}

		protected override void UpdateTextColor(Color color)
		{
			_textColorSwitcher = _textColorSwitcher ?? new TextColorSwitcher(EditText.TextColors, Element.UseLegacyColorManagement());
			_textColorSwitcher.UpdateTextColor(EditText, color);
		}
	}
}
