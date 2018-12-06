using System;
using System.ComponentModel;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using MaterialComponents;
using UIKit;
using Xamarin.Forms;
using MButton = MaterialComponents.Button;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(Xamarin.Forms.Platform.iOS.Material.MaterialButtonRenderer), new[] { typeof(VisualRendererMarker.Material) })]

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialButtonRenderer : ViewRenderer<Button, MButton>
	{
		static readonly UIControlState[] _controlStates = { UIControlState.Normal, UIControlState.Highlighted, UIControlState.Disabled };
		static readonly nfloat _minimumButtonHeight = 44; // Apple docs

		UIColor _defaultTextColorDisabled;
		UIColor _defaultTextColorHighlighted;
		UIColor _defaultTextColorNormal;
		UIColor _defaultBorderColor;
		nfloat _defaultBorderWidth = -1f;
		nfloat _defaultCornerRadius = -1f;

		bool _useLegacyColorManagement;
		bool _titleChanged;
		CGSize _titleSize;
		UIEdgeInsets _paddingDelta = new UIEdgeInsets();

		public MaterialButtonRenderer()
		{
			VisualElement.VerifyVisualFlagEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
			{
				Control.TouchUpInside -= OnButtonTouchUpInside;
				Control.TouchDown -= OnButtonTouchDown;
			}

			base.Dispose(disposing);
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			if (result.Height < _minimumButtonHeight)
				result.Height = _minimumButtonHeight;

			return result;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(CreateNativeControl());

					Debug.Assert(Control != null, "Control != null");

					_useLegacyColorManagement = e.NewElement.UseLegacyColorManagement();

					_defaultTextColorNormal = Control.TitleColor(UIControlState.Normal);
					_defaultTextColorHighlighted = Control.TitleColor(UIControlState.Highlighted);
					_defaultTextColorDisabled = Control.TitleColor(UIControlState.Disabled);

					Control.TouchUpInside += OnButtonTouchUpInside;
					Control.TouchDown += OnButtonTouchDown;
				}

				UpdateText();
				UpdateFont();
				UpdateBorder();
				UpdateImage();
				UpdateTextColor();
				UpdatePadding();
			}
		}

		protected virtual ButtonScheme CreateButtonScheme()
		{
			return new ButtonScheme
			{
				TypographyScheme = new TypographyScheme(),
				ColorScheme = MaterialColors.Light.CreateColorScheme()
			};
		}

		protected override MButton CreateNativeControl()
		{
			var button = new MButton();
			ContainedButtonThemer.ApplyScheme(CreateButtonScheme(), button);
			return button;
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
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName || e.PropertyName == Button.CornerRadiusProperty.PropertyName || e.PropertyName == Button.BorderColorProperty.PropertyName)
				UpdateBorder();
			else if (e.PropertyName == Button.ImageProperty.PropertyName)
				UpdateImage();
			else if (e.PropertyName == Button.PaddingProperty.PropertyName)
				UpdatePadding();
		}

		protected override void SetAccessibilityLabel()
		{
			// If we have not specified an AccessibilityLabel and the AccessibilityLabel is currently bound to the Title,
			// exit this method so we don't set the AccessibilityLabel value and break the binding.
			// This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
			// will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Title 
			// of the Button.

			var elemValue = (string)Element?.GetValue(AutomationProperties.NameProperty);
			if (string.IsNullOrWhiteSpace(elemValue) && Control?.AccessibilityLabel == Control?.Title(UIControlState.Normal))
				return;

			base.SetAccessibilityLabel();
		}

		void OnButtonTouchUpInside(object sender, EventArgs eventArgs)
		{
			Element?.SendReleased();
			Element?.SendClicked();
		}

		void OnButtonTouchDown(object sender, EventArgs eventArgs)
		{
			Element?.SendPressed();
		}

		void UpdateBorder()
		{
			var uiButton = Control;
			var button = Element;

			Color borderColor = button.BorderColor;
			if (borderColor != Color.Default || _defaultBorderColor == null)
			{
				if (_defaultBorderColor == null)
					_defaultBorderColor = uiButton.GetBorderColor(UIControlState.Normal);

				if (borderColor == Color.Default)
					uiButton.SetBorderColor(_defaultBorderColor, UIControlState.Normal);
				else
					uiButton.SetBorderColor(borderColor.ToUIColor(), UIControlState.Normal);
			}

			nfloat borderWidth = (nfloat)button.BorderWidth;
			if (borderWidth >= 0 || _defaultBorderWidth >= 0)
			{
				if (_defaultBorderWidth < 0f)
					_defaultBorderWidth = uiButton.GetBorderWidth(UIControlState.Normal);

				if (borderWidth < 0f)
					uiButton.SetBorderWidth(_defaultBorderWidth, UIControlState.Normal);
				else
					uiButton.SetBorderWidth(borderWidth, UIControlState.Normal);
			}

			nfloat cornerRadius = button.CornerRadius;
			if (cornerRadius >= 0 || _defaultCornerRadius >= 0)
			{
				if (_defaultCornerRadius < 0f)
					_defaultCornerRadius = uiButton.Layer.CornerRadius;

				if (cornerRadius < 0f)
					uiButton.Layer.CornerRadius = _defaultCornerRadius;
				else
					uiButton.Layer.CornerRadius = cornerRadius;
			}
		}

		void UpdateFont()
		{
			Control.SetTitleFont(Element.ToUIFont(), UIControlState.Normal);
		}

		void UpdateText()
		{
			var newText = Element.Text;

			if (Control.Title(UIControlState.Normal) != newText)
			{
				Control.SetTitle(Element.Text, UIControlState.Normal);
				_titleChanged = true;
			}
		}

		void UpdateTextColor()
		{
			Color textColor = Element.TextColor;
			if (textColor.IsDefault)
			{
				Control.SetTitleColor(_defaultTextColorNormal, UIControlState.Normal);
				Control.SetTitleColor(_defaultTextColorHighlighted, UIControlState.Highlighted);
				Control.SetTitleColor(_defaultTextColorDisabled, UIControlState.Disabled);
			}
			else
			{
				var color = textColor.ToUIColor();

				Control.SetTitleColor(color, UIControlState.Normal);
				Control.SetTitleColor(color, UIControlState.Highlighted);
				Control.SetTitleColor(_useLegacyColorManagement ? _defaultTextColorDisabled : color, UIControlState.Disabled);

				Control.TintColor = color;
			}
		}

		async void UpdateImage()
		{
			IImageSourceHandler handler;
			FileImageSource source = Element.Image;

			if (source != null && (handler = Internals.Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(source)) != null)
			{
				UIImage uiimage;
				try
				{
					uiimage = await handler.LoadImageAsync(source, scale: (float)UIScreen.MainScreen.Scale);
				}
				catch (OperationCanceledException)
				{
					uiimage = null;
				}
				UIButton button = Control;
				if (button != null && uiimage != null)
				{
					button.SetImage(uiimage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
					button.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

					ComputeEdgeInsets(Control, Element.ContentLayout);

					// disable tint for now
					// button.SetImageTintColor(UIColor.White, UIControlState.Normal);
				}
			}
			else
			{
				Control.SetImage(null, UIControlState.Normal);
				ClearEdgeInsets();
			}

			Element.NativeSizeChanged();
		}

		void UpdatePadding(UIButton button = null)
		{
			var uiElement = button ?? Control;
			if (uiElement == null)
				return;

			if (Element.IsSet(Button.PaddingProperty))
			{
				uiElement.ContentEdgeInsets = new UIEdgeInsets(
					(float)(Element.Padding.Top + _paddingDelta.Top),
					(float)(Element.Padding.Left + _paddingDelta.Left),
					(float)(Element.Padding.Bottom + _paddingDelta.Bottom),
					(float)(Element.Padding.Right + _paddingDelta.Right)
				);
			}
		}

		void UpdateContentEdge(UIButton button = null, UIEdgeInsets? delta = null)
		{
			var uiElement = button ?? Control;
			if (uiElement == null)
				return;

			_paddingDelta = delta ?? new UIEdgeInsets();
			UpdatePadding(uiElement);
		}

		void ClearEdgeInsets(UIButton button = null)
		{
			var uiElement = button ?? Control;
			if (uiElement == null)
				return;

			uiElement.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
			uiElement.TitleEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
			UpdateContentEdge(uiElement);
		}

		void ComputeEdgeInsets(UIButton button, Button.ButtonContentLayout layout)
		{
			if (button?.ImageView?.Image == null || string.IsNullOrEmpty(button?.TitleLabel?.Text))
				return;

			var position = layout.Position;
			var spacing = (nfloat)(layout.Spacing / 2);

			if (position == Button.ButtonContentLayout.ImagePosition.Left)
			{
				button.ImageEdgeInsets = new UIEdgeInsets(0, -spacing, 0, spacing);
				button.TitleEdgeInsets = new UIEdgeInsets(0, spacing, 0, -spacing);
				UpdateContentEdge(button, new UIEdgeInsets(0, 2 * spacing, 0, 2 * spacing));
				return;
			}

			if (_titleChanged)
			{
				var stringToMeasure = new NSString(button.TitleLabel.Text);
				UIStringAttributes attribs = new UIStringAttributes { Font = button.TitleLabel.Font };
				_titleSize = stringToMeasure.GetSizeUsingAttributes(attribs);
				_titleChanged = false;
			}

			var labelWidth = _titleSize.Width;
			var imageWidth = button.ImageView.Image.Size.Width;

			if (position == Button.ButtonContentLayout.ImagePosition.Right)
			{
				button.ImageEdgeInsets = new UIEdgeInsets(0, labelWidth + spacing, 0, -labelWidth - spacing);
				button.TitleEdgeInsets = new UIEdgeInsets(0, -imageWidth - spacing, 0, imageWidth + spacing);
				UpdateContentEdge(button, new UIEdgeInsets(0, 2 * spacing, 0, 2 * spacing));
				return;
			}

			var imageVertOffset = (_titleSize.Height / 2);
			var titleVertOffset = (button.ImageView.Image.Size.Height / 2);

			var edgeOffset = (float)Math.Min(imageVertOffset, titleVertOffset);

			UpdateContentEdge(button, new UIEdgeInsets(edgeOffset, 0, edgeOffset, 0));

			var horizontalImageOffset = labelWidth / 2;
			var horizontalTitleOffset = imageWidth / 2;

			if (position == Button.ButtonContentLayout.ImagePosition.Bottom)
			{
				imageVertOffset = -imageVertOffset;
				titleVertOffset = -titleVertOffset;
			}

			button.ImageEdgeInsets = new UIEdgeInsets(-imageVertOffset, horizontalImageOffset, imageVertOffset, -horizontalImageOffset);
			button.TitleEdgeInsets = new UIEdgeInsets(titleVertOffset, -horizontalTitleOffset, -titleVertOffset, horizontalTitleOffset);
		}
	}
}