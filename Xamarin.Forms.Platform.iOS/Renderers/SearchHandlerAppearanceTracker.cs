using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class SearchHandlerAppearanceTracker : IDisposable
	{
		UIColor _cancelButtonTextColorDefaultDisabled;
		UIColor _cancelButtonTextColorDefaultHighlighted;
		UIColor _cancelButtonTextColorDefaultNormal;
		UIColor _defaultTextColor;
		UIColor _defaultTintColor;
		bool _hasCustomBackground;
		UIColor _defaultBackgroundColor;
		SearchHandler _searchHandler;
		UISearchBar _uiSearchBar;
		UIToolbar _numericAccessoryView;
		bool _disposed;

		public SearchHandlerAppearanceTracker(UISearchBar searchBar, SearchHandler searchHandler)
		{
			_searchHandler = searchHandler;
			_searchHandler.PropertyChanged += SearchHandlerPropertyChanged;
			_uiSearchBar = searchBar;
			_uiSearchBar.OnEditingStarted += OnEditingStarted;
			_uiSearchBar.OnEditingStopped += OnEditingEnded;
			_uiSearchBar.ShowsCancelButton = true;
			GetDefaultSearchBarColors(_uiSearchBar);
			var uiTextField = searchBar.FindDescendantView<UITextField>();
			UpdateSearchBarColors();
			UpdateSearchBarTextAlignment(uiTextField);
			UpdateFont(uiTextField);
			UpdateKeyboard();
		}

		void SearchHandlerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.Is(SearchHandler.BackgroundColorProperty))
			{
				UpdateSearchBarBackgroundColor(_uiSearchBar.FindDescendantView<UITextField>());
			}
			else if (e.Is(SearchHandler.TextColorProperty))
			{
				UpdateSearchBarBackgroundColor(_uiSearchBar.FindDescendantView<UITextField>());
			}
			else if (e.IsOneOf(SearchHandler.PlaceholderColorProperty, SearchHandler.PlaceholderProperty))
			{
				UpdateSearchBarPlaceholder(_uiSearchBar.FindDescendantView<UITextField>());
			}
			else if (e.IsOneOf(SearchHandler.FontFamilyProperty, SearchHandler.FontAttributesProperty, SearchHandler.FontSizeProperty))
			{
				UpdateFont(_uiSearchBar.FindDescendantView<UITextField>());
			}
			else if (e.Is(SearchHandler.CancelButtonColorProperty))
			{
				UpdateCancelButton(_uiSearchBar.FindDescendantView<UIButton>());
			}
			else if (e.Is(SearchHandler.KeyboardProperty))
			{
				UpdateKeyboard();
			}
		}

		public void UpdateSearchBarColors()
		{
			var cancelButton = _uiSearchBar.FindDescendantView<UIButton>();
			var uiTextField = _uiSearchBar.FindDescendantView<UITextField>();
			UpdateSearchBarTextColor(uiTextField);
			UpdateSearchBarPlaceholder(uiTextField);
			UpdateCancelButton(cancelButton);
			UpdateSearchBarBackgroundColor(uiTextField);
		}

		void GetDefaultSearchBarColors(UISearchBar searchBar)
		{
			_defaultTintColor = searchBar.BarTintColor;

			var cancelButton = searchBar.FindDescendantView<UIButton>();
			if (cancelButton != null)
			{
				_cancelButtonTextColorDefaultNormal = cancelButton.TitleColor(UIControlState.Normal);
				_cancelButtonTextColorDefaultHighlighted = cancelButton.TitleColor(UIControlState.Highlighted);
				_cancelButtonTextColorDefaultDisabled = cancelButton.TitleColor(UIControlState.Disabled);
			}
		}

		void UpdateFont(UITextField textField)
		{

			if (textField == null)
				return;

			textField.Font = _searchHandler.ToUIFont();
		}


		void UpdateSearchBarBackgroundColor(UITextField textField)
		{
			if (textField == null)
				return;

			var backGroundColor = _searchHandler.BackgroundColor;

			if (!_hasCustomBackground && backGroundColor.IsDefault)
				return;

			var backgroundView = textField.Subviews[0];

			if (backGroundColor.IsDefault)
			{
				backgroundView.Layer.CornerRadius = 0;
				backgroundView.ClipsToBounds = false;
				backgroundView.BackgroundColor = _defaultBackgroundColor;
			}

			_hasCustomBackground = true;

			backgroundView.Layer.CornerRadius = 10;
			backgroundView.ClipsToBounds = true;
			if (_defaultBackgroundColor == null)
				_defaultBackgroundColor = backgroundView.BackgroundColor;
			backgroundView.BackgroundColor = backGroundColor.ToUIColor();
		}

		void UpdateCancelButton(UIButton cancelButton)
		{
			if (cancelButton == null)
				return;

			var cancelColor = _searchHandler.CancelButtonColor;
			if (cancelColor.IsDefault)
			{
				cancelButton.SetTitleColor(_cancelButtonTextColorDefaultNormal, UIControlState.Normal);
				cancelButton.SetTitleColor(_cancelButtonTextColorDefaultHighlighted, UIControlState.Highlighted);
				cancelButton.SetTitleColor(_cancelButtonTextColorDefaultDisabled, UIControlState.Disabled);
			}
			else
			{
				var cancelUIColor = cancelColor.ToUIColor();
				cancelButton.SetTitleColor(cancelUIColor, UIControlState.Normal);
				cancelButton.SetTitleColor(cancelUIColor, UIControlState.Highlighted);
				cancelButton.SetTitleColor(cancelUIColor, UIControlState.Disabled);
			}
		}

		void UpdateSearchBarPlaceholder(UITextField textField)
		{
			if (textField == null)
				return;

			var formatted = (FormattedString)_searchHandler.Placeholder ?? string.Empty;
			var targetColor = _searchHandler.PlaceholderColor;
			var placeHolderColor = targetColor.IsDefault ? ColorExtensions.SeventyPercentGrey.ToColor() : targetColor;
			textField.AttributedPlaceholder = formatted.ToAttributed(_searchHandler, placeHolderColor, _searchHandler.HorizontalTextAlignment);
		}

		void UpdateSearchBarTextColor(UITextField textField)
		{
			if (textField == null)
				return;

			_defaultTextColor = _defaultTextColor ?? textField.TextColor;
			var targetColor = _searchHandler.TextColor;

			textField.TextColor = targetColor.IsDefault ? _defaultTextColor : targetColor.ToUIColor();
			UpdateSearchBarTintColor(targetColor);
		}

		void UpdateSearchBarTintColor(Color targetColor)
		{
			_uiSearchBar.TintColor = targetColor.IsDefault ? _defaultTintColor : targetColor.ToUIColor();
		}

		void UpdateSearchBarTextAlignment(UITextField textField)
		{
			if (textField == null)
				return;

			textField.TextAlignment = _searchHandler.HorizontalTextAlignment.ToNativeTextAlignment(EffectiveFlowDirection.Explicit);
		}

		void UpdateKeyboard()
		{
			var keyboard = _searchHandler.Keyboard;
			_uiSearchBar.ApplyKeyboard(keyboard);
		
			// iPhone does not have an enter key on numeric keyboards
			if (Device.Idiom == TargetIdiom.Phone && (keyboard == Keyboard.Numeric || keyboard == Keyboard.Telephone))
			{
				_numericAccessoryView = _numericAccessoryView ?? CreateNumericKeyboardAccessoryView();
				_uiSearchBar.InputAccessoryView = _numericAccessoryView;
			}
			else
			{
				_uiSearchBar.InputAccessoryView = null;
			}

			_uiSearchBar.ReloadInputViews();
		}

		void OnEditingEnded(object sender, EventArgs e)
		{
			//ElementController?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		void OnEditingStarted(object sender, EventArgs e)
		{
			//ElementController?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void OnSearchButtonClicked(object sender, EventArgs e)
		{
			((ISearchHandlerController)_searchHandler).QueryConfirmed();
			_uiSearchBar.ResignFirstResponder();
		}

		UIToolbar CreateNumericKeyboardAccessoryView()
		{
			var keyboardWidth = UIScreen.MainScreen.Bounds.Width;
			var accessoryView = new UIToolbar(new CGRect(0, 0, keyboardWidth, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };

			var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			var searchButton = new UIBarButtonItem(UIBarButtonSystemItem.Search, OnSearchButtonClicked);
			accessoryView.SetItems(new[] { spacer, searchButton }, false);

			return accessoryView;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;

			if (disposing)
			{
				if (_uiSearchBar != null)
				{
					_uiSearchBar.OnEditingStarted -= OnEditingStarted;
					_uiSearchBar.OnEditingStopped -= OnEditingEnded;
				}
				if (_searchHandler != null)
				{
					_searchHandler.PropertyChanged -= SearchHandlerPropertyChanged;
				}
				_searchHandler = null;
				_uiSearchBar = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}
