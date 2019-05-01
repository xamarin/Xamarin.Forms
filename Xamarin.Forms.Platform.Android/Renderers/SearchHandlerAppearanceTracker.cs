using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	public class SearchHandlerAppearanceTracker : IDisposable
	{
		SearchHandler _searchHandler;
		bool _disposed;
		AView _control;
		EditText _editText;
		InputTypes _inputType;
		TextColorSwitcher _textColorSwitcher;
		TextColorSwitcher _hintColorSwitcher;

		public SearchHandlerAppearanceTracker(IShellSearchView searchView)
		{
			_searchHandler = searchView.SearchHandler;
			_control = searchView.View;
			_searchHandler.PropertyChanged += SearchHandlerPropertyChanged;
			_editText = (_control as ViewGroup).GetChildrenOfType<EditText>().FirstOrDefault();
			_textColorSwitcher = new TextColorSwitcher(_editText.TextColors, false);
			_hintColorSwitcher = new TextColorSwitcher(_editText.HintTextColors, false);
			UpdateSearchBarColors();
			UpdateFont();
			UpdateTextAlignment();
			UpdateInputType();
		}

		void UpdateSearchBarColors()
		{
			UpdateBackgroundColor();
			UpdateTextColor();
			UpdatePlaceholderColor();
			UpdateCancelButton();
		}

		void UpdateFont()
		{
			_editText.Typeface = _searchHandler.ToTypeface();
			_editText.SetTextSize(ComplexUnitType.Sp, (float)_searchHandler.FontSize);
		}

		void UpdatePlaceholderColor()
		{
			_hintColorSwitcher?.UpdateTextColor(_editText, _searchHandler.PlaceholderColor, _editText.SetHintTextColor);
		}

		void UpdateTextAlignment()
		{
			_editText.UpdateHorizontalAlignment(_searchHandler.HorizontalTextAlignment, _control.Context.HasRtlSupport(), Xamarin.Forms.TextAlignment.Center.ToVerticalGravityFlags());
		}

		void SearchHandlerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.Is(SearchHandler.BackgroundColorProperty))
			{
				UpdateBackgroundColor();
			}
			else if (e.Is(SearchHandler.TextColorProperty))
			{
				UpdateTextColor();
			}
			else if (e.IsOneOf(SearchHandler.PlaceholderColorProperty))
			{
				UpdatePlaceholderColor();
			}
			else if (e.IsOneOf(SearchHandler.FontFamilyProperty, SearchHandler.FontAttributesProperty, SearchHandler.FontSizeProperty))
			{
				UpdateFont();
			}
			else if (e.Is(SearchHandler.CancelButtonColorProperty))
			{
				UpdateCancelButton();
			}
			else if (e.Is(SearchHandler.KeyboardProperty))
			{
				UpdateInputType();
			}
			else if (e.Is(SearchHandler.HorizontalTextAlignmentProperty))
			{
				UpdateTextAlignment();
			}
		}

		void UpdateBackgroundColor()
		{
			var linearLayout = (_control as ViewGroup).GetChildrenOfType<LinearLayout>().FirstOrDefault();
			linearLayout.SetBackgroundColor(_searchHandler.BackgroundColor.ToAndroid());
		}

		void UpdateCancelButton()
		{
			int searchViewCloseButtonId = _control.Resources.GetIdentifier("android:id/search_close_btn", null, null);
			if (searchViewCloseButtonId != 0)
			{
				var image = _control.FindViewById<ImageView>(searchViewCloseButtonId);
				if (image != null && image.Drawable != null)
				{
					var cancelColor = _searchHandler.CancelButtonColor;
					if (!cancelColor.IsDefault)
						image.Drawable.SetColorFilter(cancelColor.ToAndroid(), PorterDuff.Mode.SrcIn);
					else
						image.Drawable.ClearColorFilter();
				}
			}
		}
		void UpdateTextColor()
		{
			_textColorSwitcher?.UpdateTextColor(_editText, _searchHandler.TextColor);
		}

		void UpdateInputType()
		{
			var keyboard = _searchHandler.Keyboard;

			_inputType = keyboard.ToInputType();
			bool isSpellCheckEnableSet = false;
			bool isSpellCheckEnable = false;
			// model.IsSet(InputView.IsSpellCheckEnabledProperty)
			if (!(keyboard is Internals.CustomKeyboard))
			{
				if (isSpellCheckEnableSet)
				{
					if ((_inputType & InputTypes.TextFlagNoSuggestions) != InputTypes.TextFlagNoSuggestions)
					{
						if (!isSpellCheckEnable)
							_inputType = _inputType | InputTypes.TextFlagNoSuggestions;
					}
				}
			}
			//Control.SetInputType(_inputType);

			if (keyboard == Keyboard.Numeric)
			{
				_editText.KeyListener = GetDigitsKeyListener(_inputType);
			}
		}

		protected virtual global::Android.Text.Method.NumberKeyListener GetDigitsKeyListener(InputTypes inputTypes)
		{
			// Override this in a custom renderer to use a different NumberKeyListener
			// or to filter out input types you don't want to allow
			// (e.g., inputTypes &= ~InputTypes.NumberFlagSigned to disallow the sign)
			return LocalizedDigitsKeyListener.Create(inputTypes);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (_searchHandler != null)
				{
					_searchHandler.PropertyChanged -= SearchHandlerPropertyChanged;
				}
				_searchHandler = null;
				_control = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}