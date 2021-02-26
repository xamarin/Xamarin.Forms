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

		public static void UpdateLineBreakMode(this UILabel nativeLabel, ILabel label)
		{
			switch (label.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					nativeLabel.LineBreakMode = UILineBreakMode.Clip;
					break;
				case LineBreakMode.WordWrap:
					nativeLabel.LineBreakMode = UILineBreakMode.WordWrap;
					break;
				case LineBreakMode.CharacterWrap:
					nativeLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
					break;
				case LineBreakMode.HeadTruncation:
					nativeLabel.LineBreakMode = UILineBreakMode.HeadTruncation;
					break;
				case LineBreakMode.MiddleTruncation:
					nativeLabel.LineBreakMode = UILineBreakMode.MiddleTruncation;
					break;
				case LineBreakMode.TailTruncation:
					nativeLabel.LineBreakMode = UILineBreakMode.TailTruncation;
					break;
			}
		}

		public static void UpdateMaxLines(this UILabel nativeLabel, ILabel label)
		{
			if (label.MaxLines >= 0)
			{
				nativeLabel.Lines = label.MaxLines;
			}
			else
			{
				switch (label.LineBreakMode)
				{
					case LineBreakMode.WordWrap:
					case LineBreakMode.CharacterWrap:
						nativeLabel.Lines = 0;
						break;
					case LineBreakMode.NoWrap:
					case LineBreakMode.HeadTruncation:
					case LineBreakMode.MiddleTruncation:
					case LineBreakMode.TailTruncation:
						nativeLabel.Lines = 1;
						break;
				}
			}
		}
	}
}