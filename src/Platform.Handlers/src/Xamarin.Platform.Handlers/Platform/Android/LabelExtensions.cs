using Android.Widget;

namespace Xamarin.Platform
{
	public static class LabelExtensions
	{
		public static void UpdateText(this TextView textView, ILabel label)
		{
			textView.Text = label.Text;
		}

		public static void UpdateTextColor(this TextView textView, ILabel label)
		{
			textView.UpdateTextColor(label, null);
		}

		public static void UpdateTextColor(this TextView textView, ILabel label, Forms.Color? lastUpdateColor)
		{
			Forms.Color textColor = label.TextColor;

			if (textColor == lastUpdateColor)
				return;

			lastUpdateColor = textColor;

			textView.SetTextColor(textColor.ToNative());
		}

		public static void UpdateFont(this TextView textView, ILabel label)
		{
		
		}

		public static void UpdateCharacterSpacing(this TextView textView, ILabel label)
		{

		}

		public static void UpdateLineHeight(this TextView textView, ILabel label)
		{

		}

		public static void UpdateHorizontalTextAlignment(this TextView textView, ILabel label)
		{

		}

		public static void UpdateVerticalTextAlignment(this TextView textView, ILabel label)
		{

		}

		public static void UpdateTextDecorations(this TextView textView, ILabel label)
		{

		}

		public static void UpdateLineBreakMode(this TextView textView, ILabel label)
		{
		
		}

		public static void UpdateMaxLines(this TextView textView, ILabel label)
		{
		
		}

		public static void UpdatePadding(this TextView textView, ILabel label)
		{
	
		}
	}
}