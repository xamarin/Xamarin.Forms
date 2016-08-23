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
			var newTitleRect = new CGRect(titleRect.X, frame.Y + (frame.Height - stringHeight) / 2.0, titleRect.Width, stringHeight);
			self.Frame = newTitleRect;
		}
	}
}

