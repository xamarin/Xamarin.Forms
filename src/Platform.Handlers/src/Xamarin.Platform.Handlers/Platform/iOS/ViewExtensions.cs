using Foundation;
using UIKit;
using Xamarin.Forms;

namespace Xamarin.Platform
{
	public static class ViewExtensions
	{

		public static void SetBackgroundColor(this UIView view, UIColor color)
			=> view.BackgroundColor = color;
		public static UIColor GetBackgroundColor(this UIView view) =>
			view.BackgroundColor;
	}
}
