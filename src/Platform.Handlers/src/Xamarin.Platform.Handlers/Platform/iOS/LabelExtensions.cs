using UIKit;
using Xamarin.Forms;

namespace Xamarin.Platform
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

			if (textColor.IsDefault && label.TextType == TextType.Html)
			{
				// If no explicit text color has been specified and we're displaying HTML, 
				// let the HTML determine the colors
				return;
			}

			// Default value of color documented to be black in iOS docs
			nativeLabel.TextColor = textColor.ToNative(ColorExtensions.LabelColor);

		}

		public static void UpdateFont(this UILabel nativeLabel, ILabel label)
		{
			
		}

		public static void UpdateCharacterSpacing(this UILabel nativeLabel, ILabel label)
		{

		}

		public static void UpdateLineHeight(this UILabel nativeLabel, ILabel label)
		{
			
		}

		public static void UpdateHorizontalTextAlignment(this UILabel nativeLabel, ILabel label)
		{

		}

		public static void UpdateVerticalTextAlignment(this UILabel nativeLabel, ILabel label)
		{
	
		}

		public static void UpdateTextDecorations(this UILabel nativeLabel, ILabel label)
		{
	
		}

		public static void UpdateLineBreakMode(this UILabel nativeLabel, ILabel label)
		{

		}

		public static void UpdateMaxLines(this UILabel nativeLabel, ILabel label)
		{

		}

		public static void UpdatePadding(this UILabel nativeLabel, ILabel label)
		{
		
		}
	}
}