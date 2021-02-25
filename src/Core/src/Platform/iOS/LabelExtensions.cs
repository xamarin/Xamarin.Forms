using System;
using UIKit;
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;

namespace Microsoft.Maui
{
	public static class LabelExtensions
	{
		public static void UpdateText(this UILabel nativeLabel, ILabel label)
		{
			nativeLabel.Text = label.Text;
		}

		public static void UpdateTextColor(this UILabel nativeLabel, ILabel label)
		{
			var textColor = label.TextColor;

			if (textColor.IsDefault)
			{
				// Default value of color documented to be black in iOS docs
				nativeLabel.TextColor = textColor.ToNative(ColorExtensions.LabelColor);
			}
			else
			{
				nativeLabel.TextColor = textColor.ToNative(textColor);
			}
		}

		public static void UpdateHorizontalTextAlignment(this UILabel nativeLabel, ILabel label)
		{
			// We don't have a FlowDirection yet, so there's nothing to pass in here. 
			// TODO ezhart Update this when FlowDirection is available 
			// (or update the extension to take an ILabel instead of an alignment and work it out from there) 
			nativeLabel.TextAlignment = label.HorizontalTextAlignment.ToNative(true);
		}

		public static void UpdateVerticalTextAlignment(this UILabel nativeLabel, ILabel label)
		{
			SizeF fitSize;
			nfloat labelHeight;

			switch (label.VerticalTextAlignment)
			{
				case TextAlignment.Start:
					fitSize = nativeLabel.SizeThatFits(label.Frame.Size.ToNative());
					labelHeight = (nfloat)Math.Min(nativeLabel.Bounds.Height, fitSize.Height);
					nativeLabel.Frame = new RectangleF(0, 0, (nfloat)label.Frame.Width, labelHeight);
					break;
				case TextAlignment.Center:
					nativeLabel.Frame = new RectangleF(0, 0, (nfloat)label.Frame.Width, (nfloat)label.Frame.Height);
					break;
				case TextAlignment.End:
					fitSize = nativeLabel.SizeThatFits(label.Frame.Size.ToNative());
					labelHeight = (nfloat)Math.Min(nativeLabel.Bounds.Height, fitSize.Height);
					nfloat yOffset = (nfloat)(label.Frame.Height - labelHeight);
					nativeLabel.Frame = new RectangleF(0, yOffset, (nfloat)label.Frame.Width, labelHeight);
					break;
			}
		}
	}
}