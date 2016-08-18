using System;
using System.ComponentModel;
using System.Linq;
using AppKit;
using SizeF = CoreGraphics.CGSize;

namespace Xamarin.Forms.Platform.MacOS
{
	public class ButtonRenderer : ViewRenderer<Button, NSButton>
	{
		NSColor _buttonTextColorDefaultDisabled;
		NSColor _buttonTextColorDefaultHighlighted;
		NSColor _buttonTextColorDefaultNormal;
		bool _titleChanged;
		SizeF _titleSize;

		// This looks like it should be a const under iOS Classic,
		// but that doesn't work under iOS 
		// ReSharper disable once BuiltInTypeReferenceStyle
		// Under iOS Classic Resharper wants to suggest this use the built-in type ref
		// but under iOS that suggestion won't work
		readonly nfloat _minimumButtonHeight = 44; // Apple docs

		//public override SizeF SizeThatFits(SizeF size)
		//{
		//	var result = base.SizeThatFits(size);

		//	if (result.Height < _minimumButtonHeight)
		//	{
		//		result.Height = _minimumButtonHeight;
		//	}

		//	return result;
		//}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
				Control.Activated -= OnButtonActivated;

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var btn = new NSButton();
					btn.SetButtonType(NSButtonType.MomentaryPushIn);
					SetNativeControl(btn);


					_buttonTextColorDefaultNormal = NSColor.Black;
					_buttonTextColorDefaultHighlighted = NSColor.Black;
					_buttonTextColorDefaultDisabled = NSColor.Black;

					Control.Activated += OnButtonActivated;
				}

				UpdateText();
				UpdateFont();
				UpdateBorder();
				UpdateImage();
				UpdateTextColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Button.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
				UpdateTextColor();
			else if (e.PropertyName == Button.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Button.BorderRadiusProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName)
				UpdateBorder();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundVisibility();
			else if (e.PropertyName == Button.ImageProperty.PropertyName)
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

			foreach (var control in Control.Subviews.Where(sv => !(sv is NSTextField)))
				control.AlphaValue = shouldDrawImage ? 1.0f : 0.0f;
		}

		void UpdateBorder()
		{
			var uiButton = Control;
			var button = Element;

			if (button.BorderColor != Color.Default)
				uiButton.Layer.BorderColor = button.BorderColor.ToCGColor();

			uiButton.Layer.BorderWidth = (float)button.BorderWidth;
			uiButton.Layer.CornerRadius = button.BorderRadius;

			UpdateBackgroundVisibility();
		}

		void UpdateFont()
		{

		}

		void UpdateImage()
		{
			//IImageSourceHandler handler;
			//FileImageSource source = Element.Image;
			//if (source != null && (handler = Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
			//{
			//	UIImage uiimage;
			//	try
			//	{
			//		uiimage = await handler.LoadImageAsync(source, scale: (float)UIScreen.MainScreen.Scale);
			//	}
			//	catch (OperationCanceledException)
			//	{
			//		uiimage = null;
			//	}
			//	UIButton button = Control;
			//	if (button != null && uiimage != null)
			//	{
			//		if (Forms.IsiOS7OrNewer)
			//			button.SetImage(uiimage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			//		else
			//			button.SetImage(uiimage, UIControlState.Normal);

			//		button.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			//		ComputeEdgeInsets(Control, Element.ContentLayout);
			//	}
			//}
			//else
			//{
			//	Control.SetImage(null, UIControlState.Normal);
			//	ClearEdgeInsets(Control);
			//}
			//((IVisualElementController)Element).NativeSizeChanged();
		}

		void UpdateText()
		{
			var newText = Element.Text;

			//if (Control.Title(UIControlState.Normal) != newText)
			//{
			//	Control.SetTitle(Element.Text, UIControlState.Normal);
			//	_titleChanged = true;
			//}
		}

		void UpdateTextColor()
		{
			//if (Element.TextColor == Color.Default)
			//{
			//	Control.SetTitleColor(_buttonTextColorDefaultNormal, UIControlState.Normal);
			//	Control.SetTitleColor(_buttonTextColorDefaultHighlighted, UIControlState.Highlighted);
			//	Control.SetTitleColor(_buttonTextColorDefaultDisabled, UIControlState.Disabled);
			//}
			//else
			//{
			//	Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Normal);
			//	Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Highlighted);
			//	Control.SetTitleColor(_buttonTextColorDefaultDisabled, UIControlState.Disabled);

			//	if (Forms.IsiOS7OrNewer)
			//		Control.TintColor = Element.TextColor.ToUIColor();
			//}
		}
	}
}

