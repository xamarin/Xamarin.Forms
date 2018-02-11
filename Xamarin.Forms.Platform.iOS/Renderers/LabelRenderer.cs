using System;
using System.ComponentModel;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using Foundation;
using System.Collections.Generic;

#if __MOBILE__
using UIKit;
using NativeLabel = UIKit.UILabel;
#else
using AppKit;
using NativeLabel = AppKit.NSTextField;
#endif

#if __MOBILE__
namespace Xamarin.Forms.Platform.iOS
#else
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

			var widthFits = widthConstraint >= _perfectSize.Request.Width;
			var heightFits = heightConstraint >= _perfectSize.Request.Height;

			if (widthFits && heightFits)
				return _perfectSize;

			var result = base.GetDesiredSize(widthConstraint, heightConstraint);
			var tinyWidth = Math.Min(10, result.Request.Width);
			result.Minimum = new Size(tinyWidth, result.Request.Height);

			if (widthFits || Element.LineBreakMode == LineBreakMode.NoWrap)
				return result;

			bool containerIsNotInfinitelyWide = !double.IsInfinity(widthConstraint);

			if (containerIsNotInfinitelyWide)
			{
				bool textCouldHaveWrapped = Element.LineBreakMode == LineBreakMode.WordWrap || Element.LineBreakMode == LineBreakMode.CharacterWrap;
				bool textExceedsContainer = result.Request.Width > widthConstraint;

				if (textExceedsContainer || textCouldHaveWrapped)
				{
					var expandedWidth = Math.Max(tinyWidth, widthConstraint);
					result.Request = new Size(expandedWidth, result.Request.Height);
				}
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

#if __MOBILE__
					Control.Frame = new RectangleF(0, 0, (nfloat)Element.Width, (nfloat)Element.Height);
#else
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (nfloat)Math.Min(Bounds.Height, fitSize.Height);
					var yOffset = (int)(Element.Height / 2 - labelHeight / 2);
					Control.Frame = new RectangleF(0, 0, (nfloat)Element.Width, (nfloat)Element.Height - yOffset);
#endif
					break;
				case TextAlignment.End:
					fitSize = Control.SizeThatFits(Element.Bounds.Size.ToSizeF());
					labelHeight = (nfloat)Math.Min(Bounds.Height, fitSize.Height);
#if __MOBILE__
					nfloat yOffset = 0;
					yOffset = (nfloat)(Element.Height - labelHeight);
					Control.Frame = new RectangleF(0, yOffset, (nfloat)Element.Width, labelHeight);
#else
					Control.Frame = new RectangleF(0, 0, (nfloat)Element.Width, labelHeight);
#endif
					break;
			}

			RecalculateSpanPositions(Control.Frame);

		}

		double FindDefaultLineHeight(int start, int length)
		{
			if (length == 0)
				return 0.0;

			var textStorage = new NSTextStorage();
#if __MOBILE__
			textStorage.SetString(Control.AttributedText.Substring(start, length));
#else
			textStorage.SetString(Control.AttributedStringValue.Substring(start, length));
#endif
			var layoutManager = new NSLayoutManager();
			textStorage.AddLayoutManager(layoutManager);

			var textContainer = new NSTextContainer(size: new SizeF(double.MaxValue, double.MaxValue))
			{
				LineFragmentPadding = 0
			};
			layoutManager.AddTextContainer(textContainer);

			var glyph = new NSRange();
#if __MOBILE__
			layoutManager.CharacterRangeForGlyphRange(new NSRange(0, 1), ref glyph);
#else
			layoutManager.CharacterRangeForGlyphRange(new NSRange(0, 1), out glyph);
#endif
			var rect = layoutManager.BoundingRectForGlyphRange(glyph, textContainer);
			return rect.Bottom - rect.Top;
		}

		void RecalculateSpanPositions(RectangleF finalSize)
		{
			if (Element?.FormattedText?.Spans == null
				|| Element.FormattedText.Spans.Count == 0)
				return;

			if (finalSize.Width <= 0 || finalSize.Height <= 0)
				return;

#if __MOBILE__
			var inline = Control.AttributedText;
#else
			var inline = Control.AttributedStringValue;
#endif
			var range = new NSRange(0, inline.Length);

			NSTextStorage textStorage = new NSTextStorage();
			textStorage.SetString(inline);

			var layoutManager = new NSLayoutManager();

			textStorage.AddLayoutManager(layoutManager);
			
			var textContainer = new NSTextContainer(size: Control.Frame.Size)
			{
				LineFragmentPadding = 0
			};

			layoutManager.AddTextContainer(textContainer);
			 
			var labelWidth = finalSize.Width;

			var currentLocation = 0;

			for (int i = 0; i < Element.FormattedText.Spans.Count; i++)
			{
				var span = Element.FormattedText.Spans[i];
				var glyphRange = new NSRange();

				var location = currentLocation;
				var length = span.Text.Length;
							
				var startRange = new NSRange(location, 1);
				var endRange = new NSRange(location + length, 1);
#if __MOBILE__
				layoutManager.CharacterRangeForGlyphRange(startRange, ref glyphRange);
#else
				layoutManager.CharacterRangeForGlyphRange(startRange, out glyphRange);
#endif
				var rect = layoutManager.BoundingRectForGlyphRange(glyphRange, textContainer);

#if __MOBILE__
				layoutManager.CharacterRangeForGlyphRange(endRange, ref glyphRange);
#else
				layoutManager.CharacterRangeForGlyphRange(endRange, out glyphRange);
#endif
				var endRect = layoutManager.BoundingRectForGlyphRange(glyphRange, textContainer);

				var startLineHeight = rect.Bottom - rect.Top;
				var endLineHeight = endRect.Bottom - endRect.Top;
				
				var defaultLineHeight = FindDefaultLineHeight(location, length);

				var yaxis = rect.Top;
				var lineHeights = new List<double>();
				while (yaxis < endRect.Bottom)
				{
					double lineHeight;
					if (yaxis == rect.Top) // First Line
					{
						lineHeight = rect.Bottom - rect.Top;
					}
					else if (yaxis != endRect.Top) // Middle Line(s)
					{
						lineHeight = defaultLineHeight;
					}
					else // Bottom Line
					{
						lineHeight = endRect.Bottom - endRect.Top;
					}
					lineHeights.Add(lineHeight);
					yaxis += (nfloat)lineHeight;
				}

				span.CalculatePositions(lineHeights.ToArray(), finalSize.Width, rect.X, endRect.X, rect.Top);

				// update current location
				currentLocation += length;
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
				UpdateTextColor();
				UpdateFont();

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
				UpdateTextColor();
			else if (e.PropertyName == Label.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Label.TextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
				UpdateLineBreakMode();
			else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
				UpdateAlignment();
		}

#if __MOBILE__
		protected override void SetAccessibilityLabel()
		{
			// If we have not specified an AccessibilityLabel and the AccessibiltyLabel is current bound to the Text,
			// exit this method so we don't set the AccessibilityLabel value and break the binding.
			// This may pose a problem for users who want to explicitly set the AccessibilityLabel to null, but this
			// will prevent us from inadvertently breaking UI Tests that are using Query.Marked to get the dynamic Text 
			// of the Label.

			var elemValue = (string)Element?.GetValue(AutomationProperties.NameProperty);
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
			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)Element).EffectiveFlowDirection);
#else
			Control.Alignment = Element.HorizontalTextAlignment.ToNativeTextAlignment(((IVisualElementController)Element).EffectiveFlowDirection);
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

		bool isTextFormatted;
		void UpdateText()
		{
			_perfectSizeValid = false;

			var values = Element.GetValues(Label.FormattedTextProperty, Label.TextProperty, Label.TextColorProperty);
			var formatted = values[0] as FormattedString;
			if (formatted != null)
			{
#if __MOBILE__
				Control.AttributedText = formatted.ToAttributed(Element, (Color)values[2]);
#else
				Control.AttributedStringValue = formatted.ToAttributed(Element, (Color)values[2]);
#endif
				isTextFormatted = true;
			}
			else
			{
				if (isTextFormatted)
				{
					UpdateFont();
					UpdateTextColor();
				}
#if __MOBILE__
				Control.Text = (string)values[1];
#else
				Control.StringValue = (string)values[1] ?? "";
#endif
				isTextFormatted = false;
			}
			UpdateLayout();
		}

		void UpdateFont()
		{
			if(isTextFormatted)
				return;
			_perfectSizeValid = false;

#if __MOBILE__
			Control.Font = Element.ToUIFont();
#else
			Control.Font = Element.ToNSFont();
#endif
			UpdateLayout();
		}

		void UpdateTextColor()
		{
			if (isTextFormatted)
				return;

			_perfectSizeValid = false;

			var textColor = (Color)Element.GetValue(Label.TextColorProperty);

			// default value of color documented to be black in iOS docs
#if __MOBILE__
			Control.TextColor = textColor.ToUIColor(ColorExtensions.Black);
#else
			Control.TextColor = textColor.ToNSColor(ColorExtensions.Black);
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