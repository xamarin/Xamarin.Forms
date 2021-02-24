using Android.Widget;

namespace Microsoft.Maui
{
	public static class LabelExtensions
	{
		public static void UpdateText(this TextView textView, ILabel label)
		{
			textView.Text = label.Text;
		}

		public static void UpdateTextColor(this TextView textView, ILabel label,  Maui.Color defaultColor)
		{
			 Maui.Color textColor = label.TextColor;

			if (textColor.IsDefault)
			{
				textView.SetTextColor(defaultColor.ToNative());
			}
			else
			{
				textView.SetTextColor(textColor.ToNative());
			}
		}

		public static void UpdateGravity(this TextView textView, ILabel label)
		{
			textView.Gravity = label.HorizontalTextAlignment.ToHorizontalGravityFlags() | label.VerticalTextAlignment.ToVerticalGravityFlags();
		}
	}
}