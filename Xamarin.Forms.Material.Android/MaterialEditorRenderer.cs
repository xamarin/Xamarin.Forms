﻿#if __ANDROID_28__
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Editor), typeof(MaterialEditorRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
namespace Xamarin.Forms.Material.Android
{
	public class MaterialEditorRenderer : EditorRendererBase<MaterialFormsTextInputLayout>
	{
		bool _disposed;
		MaterialFormsEditText _textInputEditText;
		MaterialFormsTextInputLayout _textInputLayout;

		public MaterialEditorRenderer(Context context) :
			base(MaterialContextThemeWrapper.Create(context))
		{
		}		

		protected override MaterialFormsTextInputLayout CreateNativeControl()
		{
			LayoutInflater inflater = LayoutInflater.FromContext(Context);
			var view = inflater.Inflate(Resource.Layout.TextInputLayoutFilledBox, null);
			_textInputLayout = (MaterialFormsTextInputLayout)view;
			_textInputEditText = _textInputLayout.FindViewById<MaterialFormsEditText>(Resource.Id.materialformsedittext);
			UpdatePlaceholderText();

			return _textInputLayout;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			UpdateBackgroundColor();
		}

		protected override void UpdateBackgroundColor()
		{
			if (_disposed || _textInputLayout == null)
				return;

			_textInputLayout.BoxBackgroundColor = MaterialColors.CreateEntryFilledInputBackgroundColor(Element.BackgroundColor, Element.TextColor);
		}

		protected override void UpdatePlaceholderText()
		{
			if (_disposed || _textInputLayout == null)
				return;

			_textInputLayout?.SetHint(Element.Placeholder, Element);
		}
		
		protected override void UpdatePlaceholderColor() => ApplyTheme();
		protected virtual void ApplyTheme() => _textInputLayout?.ApplyTheme(Element.TextColor, Element.PlaceholderColor);
		protected override void UpdateTextColor() => ApplyTheme();
		protected override EditText EditText => _textInputEditText;

		protected override void UpdateFont()
		{
			if (_disposed || _textInputLayout == null)
				return;

			base.UpdateFont();
			_textInputLayout.Typeface = Element.ToTypeface();
			_textInputEditText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
		}

		protected override void Dispose(bool disposing)
		{
			_disposed = true;
			base.Dispose(disposing);
		}
	}
}
#endif