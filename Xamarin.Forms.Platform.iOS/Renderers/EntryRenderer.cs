using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Platform.iOS
{
	public class EntryRenderer : ViewRenderer<Entry, UITextField>
	{
		UIColor _defaultTextColor;
		UITextField _uiTextField;
		bool _disposed;

		public EntryRenderer()
		{
			Frame = new RectangleF(0, 20, 320, 40);
		}

		IElementController ElementController => Element as IElementController;

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				UnregisterEvents();
				if (_uiTextField != null)
				{
					_uiTextField.Dispose();
					_uiTextField = null;
				}
			}

			_disposed = true;

			base.Dispose(disposing);
		}

		void UnregisterEvents()
		{
			if (_uiTextField == null)
				return;

			_uiTextField.EditingDidBegin -= OnEditingBegan;
			_uiTextField.EditingChanged -= OnEditingChanged;
			_uiTextField.EditingDidEnd -= OnEditingEnded;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				UnregisterEvents();
			}

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					_uiTextField = new UITextFieldWrapper(RectangleF.Empty)
					{
						BorderStyle = UITextBorderStyle.RoundedRect,
						ShouldReturn = OnShouldReturn
					};

					_uiTextField.EditingChanged += OnEditingChanged;
					_uiTextField.EditingDidBegin += OnEditingBegan;
					_uiTextField.EditingDidEnd += OnEditingEnded;

					_defaultTextColor = _uiTextField.TextColor;
					SetNativeControl(_uiTextField);
				}

				UpdatePlaceholder();
				UpdatePassword();
				UpdateText();
				UpdateColor();
				UpdateFont();
				UpdateKeyboard();
				UpdateAlignment();
				UpdateAdjustsFontSizeToFitWidth();
				UpdateDisabledSelectorActions();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Entry.PlaceholderProperty.PropertyName || e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
				UpdatePassword();
			else if (e.PropertyName == Entry.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
				UpdateKeyboard();
			else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
			{
				UpdateColor();
				UpdatePlaceholder();
			}
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.Entry.AdjustsFontSizeToFitWidthProperty.PropertyName)
				UpdateAdjustsFontSizeToFitWidth();
			else if (e.PropertyName == PlatformConfiguration.iOSSpecific.Entry.DisabledSelectorActionsProperty.PropertyName)
				UpdateDisabledSelectorActions();

			base.OnElementPropertyChanged(sender, e);
		}

		void OnEditingBegan(object sender, EventArgs e)
		{
			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void OnEditingChanged(object sender, EventArgs eventArgs)
		{
			ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
		}

		void OnEditingEnded(object sender, EventArgs e)
		{
			// Typing aid changes don't always raise EditingChanged event
			if (Control.Text != Element.Text)
			{
				ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
			}

			ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		bool OnShouldReturn(UITextField view)
		{
			Control.ResignFirstResponder();
			((IEntryController)Element).SendCompleted();
			return true;
		}

		void UpdateAlignment()
		{
			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		void UpdateColor()
		{
			var textColor = Element.TextColor;

			if (textColor.IsDefault || !Element.IsEnabled)
				Control.TextColor = _defaultTextColor;
			else
				Control.TextColor = textColor.ToUIColor();
		}

		void UpdateAdjustsFontSizeToFitWidth()
		{
			Control.AdjustsFontSizeToFitWidth = Element.OnThisPlatform().AdjustsFontSizeToFitWidth();
		}

		void UpdateFont()
		{
			Control.Font = Element.ToUIFont();
		}

		void UpdateKeyboard()
		{
			Control.ApplyKeyboard(Element.Keyboard);
		}

		void UpdatePassword()
		{
			if (Element.IsPassword && Control.IsFirstResponder)
			{
				Control.Enabled = false;
				Control.SecureTextEntry = true;
				Control.Enabled = Element.IsEnabled;
				Control.BecomeFirstResponder();
			}
			else
				Control.SecureTextEntry = Element.IsPassword;
		}

		void UpdatePlaceholder()
		{
			var formatted = (FormattedString)Element.Placeholder;

			if (formatted == null)
				return;

			var targetColor = Element.PlaceholderColor;

			// Placeholder default color is 70% gray
			// https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UITextField_Class/index.html#//apple_ref/occ/instp/UITextField/placeholder

			var color = Element.IsEnabled && !targetColor.IsDefault ? targetColor : ColorExtensions.SeventyPercentGrey.ToColor();

			Control.AttributedPlaceholder = formatted.ToAttributed(Element, color);
		}

		void UpdateText()
		{
			// ReSharper disable once RedundantCheckBeforeAssignment
			if (Control.Text != Element.Text)
				Control.Text = Element.Text;
		}

		void UpdateDisabledSelectorActions()
		{
			(_uiTextField as UITextFieldWrapper).DisabledSelectorActions = Element.On<PlatformConfiguration.iOS>().DisabledSelectorActions();
		}
	}

	class UITextFieldWrapper : UITextField
	{
		internal List<SelectorAction> DisabledSelectorActions { get; set; }

		internal UITextFieldWrapper(CGRect frame) : base(frame)
		{
		}

		public override bool CanPerform(Selector action, NSObject withSender)
		{
			if(DisabledSelectorActions == null || DisabledSelectorActions.Count == 0)
				return base.CanPerform(action, withSender);

			if (DisabledSelectorActions.Contains(SelectorAction.All))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.AddShortcut) && action == new Selector("_addShortcut:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Copy) && action == new Selector("copy:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Cut) && action == new Selector("cut:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Define) && action == new Selector("_define:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Delete) && action == new Selector("delete:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Lookup) && action == new Selector("_lookup:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Paste) && action == new Selector("paste:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Replace) && action == new Selector("_promptForReplace:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Select) && action == new Selector("select:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.SelectAll) && action == new Selector("selectAll:"))
				return false;

			if (DisabledSelectorActions.Contains(SelectorAction.Share) && action == new Selector("_share:"))
				return false;

			return base.CanPerform(action, withSender);
		}
	}
}