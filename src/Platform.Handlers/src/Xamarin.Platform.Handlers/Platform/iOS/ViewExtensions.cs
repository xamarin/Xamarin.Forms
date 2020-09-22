using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class ViewExtensions
	{
		public static void SetText(this UILabel label, string text)
			=> label.Text = text;

		public static void SetText(this UILabel label, NSAttributedString text)
			=> label.AttributedText = text;

		public static void SetBackgroundColor(this UIView view, UIColor color)
			=> view.BackgroundColor = color;
		public static UIColor GetBackgroundColor(this UIView view) =>
			view.BackgroundColor;
	}
}
