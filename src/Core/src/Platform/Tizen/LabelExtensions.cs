using Tizen.UIExtensions.Common;
using Tizen.UIExtensions.ElmSharp;
using TFontAttributes = Tizen.UIExtensions.Common.FontAttributes;

namespace Microsoft.Maui
{
	public static class LabelExtensions
	{
		public static void UpdateText(this Label nativeLabel, ILabel label)
		{
			nativeLabel.Text = label.Text ?? "";
		}

		public static void UpdateTextColor(this Label nativeLabel, ILabel label)
		{
			nativeLabel.TextColor = label.TextColor.ToNative();
		}

		public static void UpdateFont(this Label nativeLabel, ILabel label, IFontManager fontManager)
		{
			nativeLabel.BatchBegin();
			nativeLabel.FontSize = label.FontSize;
			nativeLabel.FontAttributes = label.FontAttributes.ToNative();
			nativeLabel.FontFamily = fontManager.GetFontFamily(label.FontFamily)??"";
			nativeLabel.BatchCommit();
		}

		public static TFontAttributes ToNative(this FontAttributes fa)
		{
			if (fa == FontAttributes.Italic)
				return TFontAttributes.Italic;
			else if (fa == FontAttributes.Bold)
				return TFontAttributes.Bold;
			else
				return TFontAttributes.None;
		}
	}
}
