using UIKit;

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

		public static void UpdateCharacterSpacing(this UILabel nativeLabel, ILabel label)
		{
			if (string.IsNullOrEmpty(label.Text))
				return;

			var textAttr = nativeLabel.AttributedText?.AddCharacterSpacing(label.Text, label.CharacterSpacing);

			if (textAttr != null)
				nativeLabel.AttributedText = textAttr;
		}
	}
}