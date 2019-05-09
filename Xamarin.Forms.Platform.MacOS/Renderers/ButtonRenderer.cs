﻿using System;
using System.ComponentModel;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ButtonRenderer : ViewRenderer<Button, NSButton>
	{
		class FormsNSButton : NSButton
		{
			public event Action Pressed;

			public event Action Released;

			public override void MouseDown(NSEvent theEvent)
			{
				Pressed?.Invoke();

				base.MouseDown(theEvent);

				Released?.Invoke();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
				Control.Activated -= OnButtonActivated;

			var formsButton = Control as FormsNSButton;
			if (formsButton != null)
			{
				formsButton.Pressed -= HandleButtonPressed;
				formsButton.Released -= HandleButtonReleased;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var btn = new FormsNSButton();
					btn.SetButtonType(NSButtonType.MomentaryPushIn);
					btn.Pressed += HandleButtonPressed;
					btn.Released += HandleButtonReleased;
					SetNativeControl(btn);

					Control.Activated += OnButtonActivated;
				}

				UpdateText();
				UpdateFont();
				UpdateBorder();
				UpdateImage();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Button.TextProperty.PropertyName || e.PropertyName == Button.TextColorProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Button.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName ||
					e.PropertyName == Button.CornerRadiusProperty.PropertyName ||
					e.PropertyName == Button.BorderColorProperty.PropertyName)
				UpdateBorder();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundVisibility();
			else if (e.PropertyName == Button.ImageSourceProperty.PropertyName)
				UpdateImage();
		}

		void OnButtonActivated(object sender, EventArgs eventArgs)
		{
			((IButtonController)Element)?.SendClicked();
		}

		void UpdateBackgroundVisibility()
		{
			var model = Element;
			var shouldDrawImage = model.BackgroundColor == Color.Default;
			if (!shouldDrawImage)
				Control.Cell.BackgroundColor = model.BackgroundColor.ToNSColor();
		}

		void UpdateBorder()
		{
			var uiButton = Control;
			var button = Element;

			if (button.BorderColor != Color.Default)
				uiButton.Layer.BorderColor = button.BorderColor.ToCGColor();

			uiButton.Layer.BorderWidth = (float)button.BorderWidth;
			uiButton.Layer.CornerRadius = button.CornerRadius;

			UpdateBackgroundVisibility();
		}

		void UpdateFont()
		{
			Control.Font = Element.Font.ToNSFont();
		}

		void UpdateImage()
		{
			this.ApplyNativeImageAsync(Button.ImageSourceProperty, image =>
			{
				NSButton button = Control;
				if (button != null && image != null)
				{
					button.Image = image;
					if (!string.IsNullOrEmpty(button.Title))
						button.ImagePosition = Element.ToNSCellImagePosition();
					((IVisualElementController)Element).NativeSizeChanged();
				}
			});
		}

		void UpdateText()
		{
			var color = Element.TextColor;
			if (color == Color.Default)
			{
				Control.Title = Element.Text ?? "";
			}
			else
			{
				var textWithColor = new NSAttributedString(Element.Text ?? "", font: Element.Font.ToNSFont(), foregroundColor: color.ToNSColor(), paragraphStyle: new NSMutableParagraphStyle() { Alignment = NSTextAlignment.Center });
				Control.AttributedTitle = textWithColor;
			}
		}

		void HandleButtonPressed()
		{
			Element?.SendPressed();
		}

		void HandleButtonReleased()
		{
			Element?.SendReleased();
		}

	}
}