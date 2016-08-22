using System;
using System.ComponentModel;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class SearchBarRenderer : ViewRenderer<SearchBar, NSSearchField>
	{
		NSColor _cancelButtonTextColorDefaultNormal;
		NSColor _defaultTextColor;
		NSColor _defaultTintColor;

		IElementController ElementController => Element as IElementController;

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Control != null)
				{
					Control.Changed -= OnTextChanged;
					Control.Cell.CancelButtonCell.Activated -= OnCancelClicked;
					Control.Cell.SearchButtonCell.Activated -= OnSearchButtonClicked;
					Control.EditingEnded -= OnEditingEnded;
					Control.EditingBegan -= OnEditingStarted;
				}
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new NSSearchField { BackgroundColor = NSColor.Clear, DrawsBackground = false });

					Control.Cell.CancelButtonCell.Activated += OnCancelClicked;
					Control.Cell.SearchButtonCell.Activated += OnSearchButtonClicked;

					Control.Changed += OnTextChanged;
					Control.EditingBegan += OnEditingStarted;
					Control.EditingEnded += OnEditingEnded;
				}

				UpdatePlaceholder();
				UpdateText();
				UpdateFont();
				UpdateIsEnabled();
				UpdateCancelButton();
				UpdateAlignment();
				UpdateTextColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == SearchBar.PlaceholderProperty.PropertyName || e.PropertyName == SearchBar.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateIsEnabled();
				UpdateTextColor();
				UpdatePlaceholder();
			}
			else if (e.PropertyName == SearchBar.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == SearchBar.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == SearchBar.CancelButtonColorProperty.PropertyName)
				UpdateCancelButton();
			else if (e.PropertyName == SearchBar.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == SearchBar.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == SearchBar.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == SearchBar.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
		}

		protected override void SetBackgroundColor(Color color)
		{
			base.SetBackgroundColor(color);

			if (Control == null)
				return;
			if (color == Color.Default)
				Control.BackgroundColor = NSColor.Clear;
			else
				Control.BackgroundColor = color.ToNSColor();

			//if (_defaultTintColor == null)
			//{
			//	if (Forms.IsiOS7OrNewer)
			//		_defaultTintColor = Control.BarTintColor;
			//	else
			//		_defaultTintColor = Control.TintColor;
			//}

			//if (Forms.IsiOS7OrNewer)
			//	Control.BarTintColor = color.ToUIColor(_defaultTintColor);
			//else
			//	Control.TintColor = color.ToUIColor(_defaultTintColor);

			//if (color.A < 1)
			//	Control.SetBackgroundImage(new UIImage(), UIBarPosition.Any, UIBarMetrics.Default);

			// updating BarTintColor resets the button color so we need to update the button color again
			UpdateCancelButton();
		}

		void OnCancelClicked(object sender, EventArgs args)
		{
			ElementController.SetValueFromRenderer(SearchBar.TextProperty, null);
			Control.ResignFirstResponder();
		}

		void OnEditingEnded(object sender, EventArgs e)
		{
			ElementController?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		void OnEditingStarted(object sender, EventArgs e)
		{
			ElementController?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void OnSearchButtonClicked(object sender, EventArgs e)
		{
			((ISearchBarController)Element).OnSearchButtonPressed();
			Control.ResignFirstResponder();
		}

		void OnTextChanged(object sender, EventArgs a)
		{
			ElementController.SetValueFromRenderer(SearchBar.TextProperty, Control.StringValue);
		}

		void UpdateAlignment()
		{
			Control.Alignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		void UpdateCancelButton()
		{

			//// We can't cache the cancel button reference because iOS drops it when it's not displayed
			//// and creates a brand new one when necessary, so we have to look for it each time
			//var cancelButton = Control.FindDescendantView<UIButton>();

			//if (cancelButton == null)
			//	return;
			var cancelButtonColor = Element.CancelButtonColor;

			if (cancelButtonColor.IsDefault)
			{
				Control.Cell.CancelButtonCell.Title = "";
			}
			else
			{
				var textWithColor = new NSAttributedString(Control.Cell.CancelButtonCell.Title ?? "", foregroundColor: cancelButtonColor.ToNSColor());
				Control.Cell.CancelButtonCell.AttributedTitle = textWithColor;
			}

		}

		void UpdateFont()
		{
			Control.Font = Element.ToNSFont();
		}

		void UpdateIsEnabled()
		{
			Control.Enabled = Element.IsEnabled;
		}

		void UpdatePlaceholder()
		{
			var formatted = (FormattedString)Element.Placeholder ?? string.Empty;
			var targetColor = Element.PlaceholderColor;
			var color = Element.IsEnabled && !targetColor.IsDefault ? targetColor : ColorExtensions.SeventyPercentGrey.ToColor();
			Control.PlaceholderAttributedString = formatted.ToAttributed(Element, color);
		}

		void UpdateText()
		{
			Control.StringValue = Element.Text ?? "";
			UpdateCancelButton();
		}

		void UpdateTextColor()
		{

			_defaultTextColor = _defaultTextColor ?? Control.TextColor;
			var targetColor = Element.TextColor;

			var color = Element.IsEnabled && !targetColor.IsDefault ? targetColor : _defaultTextColor.ToColor();

			Control.TextColor = color.ToNSColor();
		}
	}
}

