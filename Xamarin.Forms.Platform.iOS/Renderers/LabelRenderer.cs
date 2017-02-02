using System;
using System.ComponentModel;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;

#if __MOBILE__
using UIKit;
using NativeLabel = UIKit.UILabel;

namespace Xamarin.Forms.Platform.iOS
#else
using AppKit;
using NativeLabel = AppKit.NSTextField;

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	public class LabelRenderer : ViewRenderer<Label, NativeLabel>
	{
		SizeRequest _perfectSize;

		bool _perfectSizeValid;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (!_perfectSizeValid)
			{
				_perfectSize = base.GetDesiredSize(double.PositiveInfinity, double.PositiveInfinity);
				_perfectSize.Minimum = new Size(Math.Min(10, _perfectSize.Request.Width), _perfectSize.Request.Height);
				_perfectSizeValid = true;
			}

			if (widthConstraint >= _perfectSize.Request.Width && heightConstraint >= _perfectSize.Request.Height)
				return _perfectSize;

			var result = base.GetDesiredSize(widthConstraint, heightConstraint);
			result.Minimum = new Size(Math.Min(10, result.Request.Width), result.Request.Height);
			if (Element.LineBreakMode != LineBreakMode.NoWrap)
			{
				if (result.Request.Width > widthConstraint || Element.LineBreakMode == LineBreakMode.WordWrap || Element.LineBreakMode == LineBreakMode.CharacterWrap)
					result.Request = new Size(Math.Max(result.Minimum.Width, widthConstraint), result.Request.Height);
			}

			return result;
		}

#if __MOBILE__
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
#else
		public override void Layout()
		{
			base.Layout();
#endif

			if (Control == null)
				return;

			SizeF fitSize;
			nfloat labelHeight;
			switch (Element.VerticalTextAlignment)
			{
				case TextAlignment.Start:
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (nfloat)Math.Min(Bounds.Height, fitSize.Height);
					Control.Frame = new RectangleF(0, 0, (nfloat)Element.Width, labelHeight);
					break;
				case TextAlignment.Center:
					Control.Frame = new RectangleF(0, 0, (nfloat)Element.Width, (nfloat)Element.Height);
					break;
				case TextAlignment.End:
					nfloat yOffset = 0;
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (nfloat)Math.Min(Bounds.Height, fitSize.Height);
					yOffset = (nfloat)(Element.Height - labelHeight);
					Control.Frame = new RectangleF(0, yOffset, (nfloat)Element.Width, labelHeight);
					break;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new NativeLabel(RectangleF.Empty));
#if !__MOBILE__
					Control.Editable = false;
					Control.Bezeled = false;
					Control.DrawsBackground = false;
#endif
				}

				UpdateText();
				UpdateLineBreakMode();
				UpdateAlignment();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName)
				UpdateAlignment();
			else if (e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
				UpdateLayout();
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.FontProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
				UpdateLineBreakMode();
		}

#if __MOBILE__
		protected override void SetAccessibilityLabel()
		{
			// If we have not specified an AccessibilityLabel and the AccessibiltyLabel is current bound to the Text,
			// exit this method so we don't set the AccessibilityLabel value and break the binding.
			// This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
			// will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Text 
			// of the Label.

			var elemValue = (string)Element?.GetValue(Accessibility.NameProperty);
			if (string.IsNullOrWhiteSpace(elemValue) && Control?.AccessibilityLabel == Control?.Text)
				return;

			base.SetAccessibilityLabel();
		}
#endif

		protected override void SetBackgroundColor(Color color)
		{
#if __MOBILE__
			if (color == Color.Default)
				BackgroundColor = UIColor.Clear;
			else
				BackgroundColor = color.ToUIColor();
#else
			if (color == Color.Default)
				Layer.BackgroundColor = NSColor.Clear.CGColor;
			else
				Layer.BackgroundColor = color.ToCGColor();
#endif

		}

		void UpdateAlignment()
		{
#if __MOBILE__
			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
#else
			Control.Alignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
#endif
		}

		void UpdateLineBreakMode()
		{
			_perfectSizeValid = false;
#if __MOBILE__
			switch (Element.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					Control.LineBreakMode = UILineBreakMode.Clip;
					Control.Lines = 1;
					break;
				case LineBreakMode.WordWrap:
					Control.LineBreakMode = UILineBreakMode.WordWrap;
					Control.Lines = 0;
					break;
				case LineBreakMode.CharacterWrap:
					Control.LineBreakMode = UILineBreakMode.CharacterWrap;
					Control.Lines = 0;
					break;
				case LineBreakMode.HeadTruncation:
					Control.LineBreakMode = UILineBreakMode.HeadTruncation;
					Control.Lines = 1;
					break;
				case LineBreakMode.MiddleTruncation:
					Control.LineBreakMode = UILineBreakMode.MiddleTruncation;
					Control.Lines = 1;
					break;
				case LineBreakMode.TailTruncation:
					Control.LineBreakMode = UILineBreakMode.TailTruncation;
					Control.Lines = 1;
					break;
			}
#else
			switch (Element.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					Control.LineBreakMode = NSLineBreakMode.Clipping;
					Control.MaximumNumberOfLines = 1;
					break;
				case LineBreakMode.WordWrap:
					Control.LineBreakMode = NSLineBreakMode.ByWordWrapping;
					Control.MaximumNumberOfLines = 0;
					break;
				case LineBreakMode.CharacterWrap:
					Control.LineBreakMode = NSLineBreakMode.CharWrapping;
					Control.MaximumNumberOfLines = 0;
					break;
				case LineBreakMode.HeadTruncation:
					Control.LineBreakMode = NSLineBreakMode.TruncatingHead;
					Control.MaximumNumberOfLines = 1;
					break;
				case LineBreakMode.MiddleTruncation:
					Control.LineBreakMode = NSLineBreakMode.TruncatingMiddle;
					Control.MaximumNumberOfLines = 1;
					break;
				case LineBreakMode.TailTruncation:
					Control.LineBreakMode = NSLineBreakMode.TruncatingTail;
					Control.MaximumNumberOfLines = 1;
					break;
			}
#endif
		}

		void UpdateText()
		{
			_perfectSizeValid = false;

			var values = Element.GetValues(Label.FormattedTextProperty, Label.TextProperty, Label.TextColorProperty);
			var formatted = values[0] as FormattedString;
			if (formatted != null)
			{
#if __MOBILE__
				Control.AttributedText = formatted.ToAttributed(Element, (Color)values[2]);
			}
			else
			{
				Control.Text = (string)values[1];
				// default value of color documented to be black in iOS docs
				Control.Font = Element.ToUIFont();
				Control.TextColor = ((Color)values[2]).ToUIColor(ColorExtensions.Black);
			}
#else
				Control.AttributedStringValue = formatted.ToAttributed(Element, (Color)values[2]);
			}
			else
			{
				Control.StringValue = (string)values[1] ?? "";
				// default value of color documented to be black in iOS docs
				Control.Font = Element.ToNSFont();
				Control.TextColor = ((Color)values[2]).ToNSColor(ColorExtensions.Black);
			}
#endif

			UpdateLayout();
		}

		void UpdateLayout()
		{
#if __MOBILE__
			LayoutSubviews();
#else
			Layout();
#endif
		}
	}
}