using System;
using AppKit;
using CoreGraphics;

namespace Xamarin.Forms.Platform.MacOS
{
	public static class NSTextFieldExtensions
	{
		public static void CenterTextVertically(this NSTextField self)
		{
			self.CenterTextVertically(self.Frame);
		}

		public static void CenterTextVertically(this NSTextField self, CGRect frame)
		{
			var stringHeight = self.Cell.AttributedStringValue.Size.Height;
			var titleRect = self.Cell.TitleRectForBounds(frame);

			var h = stringHeight + (stringHeight - (self.Font.Ascender + self.Font.Descender));
			var y = frame.Size.Height / 2 - self.LastBaselineOffsetFromBottom - self.Font.XHeight / 2;

			self.Frame = new CGRect(titleRect.X, y, titleRect.Width, h);
		}
	}
}

