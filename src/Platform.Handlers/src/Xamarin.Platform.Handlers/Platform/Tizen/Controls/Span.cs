using System;
using System.Text;
using ElmSharp;

namespace Xamarin.Platform.Tizen
{
	public class Span
	{
		string _text = "";

		public FormattedString? FormattedText { get; set; }

		public string Text
		{
			get
			{
				if (FormattedText != null)
				{
					return FormattedText.ToString();
				}
				else
				{
					return _text;
				}
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				else
				{
					FormattedText = null;
				}
				_text = value;
			}
		}

		public Color ForegroundColor { get; set; }

		public Color BackgroundColor { get; set; }

		public string FontFamily { get; set; }

		public Forms.FontAttributes FontAttributes { get; set; }

		public double FontSize { get; set; }

		public string FontWeight { get; set; }

		public double LineHeight { get; set; }
		public LineBreakMode LineBreakMode { get; set; }

		public TextAlignment HorizontalTextAlignment { get; set; }

		public bool Underline { get; set; }

		public bool Strikethrough { get; set; }

		public Span()
		{
			Text = "";
			FontFamily = "";
			FontSize = -1;
			FontWeight = ThemeConstants.FontWeight.Styles.None;
			FontAttributes = Forms.FontAttributes.None;
			ForegroundColor = Color.Default;
			BackgroundColor = Color.Default;
			HorizontalTextAlignment = TextAlignment.None;
			LineBreakMode = LineBreakMode.None;
			Underline = false;
			Strikethrough = false;
			LineHeight = -1.0d;
		}

		internal string GetMarkupText()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<span ");
			sb = PrepareFormattingString(sb);
			sb.Append(">");
			sb.Append(GetDecoratedText());
			sb.Append("</span>");
			return sb.ToString();
		}

		public string GetDecoratedText()
		{
			if (FormattedText != null)
			{
				return FormattedText.ToMarkupString();
			}
			else
			{
				return ConvertTags(Text);
			}
		}

		StringBuilder PrepareFormattingString(StringBuilder _formattingString)
		{
			if (!ForegroundColor.IsDefault)
			{
				_formattingString.AppendFormat("color={0} ", ForegroundColor.ToHex());
			}

			if (!BackgroundColor.IsDefault)
			{
				_formattingString.AppendFormat("backing_color={0} backing=on ", BackgroundColor.ToHex());
			}

			if (!string.IsNullOrEmpty(FontFamily))
			{
				_formattingString.AppendFormat("font={0} ", FontFamily);
			}

			if (FontSize != -1)
			{
				_formattingString.AppendFormat("font_size={0} ", FontSize.ToEflFontPoint());
			}

			if ((FontAttributes & Forms.FontAttributes.Bold) != 0)
			{
				_formattingString.Append("font_weight=Bold ");
			}
			else
			{
				// FontWeight is only available in case of FontAttributes.Bold is not used.
				if (FontWeight != ThemeConstants.FontWeight.Styles.None)
				{
					_formattingString.AppendFormat("font_weight={0} ", FontWeight);
				}
			}

			if ((FontAttributes & Forms.FontAttributes.Italic) != 0)
			{
				_formattingString.Append("font_style=italic ");
			}

			if (Underline)
			{
				_formattingString.AppendFormat("underline=on underline_color={0} ",
					ForegroundColor.IsDefault ? ThemeConstants.Span.ColorClass.DefaultUnderLineColor.ToHex() : ForegroundColor.ToHex());
			}

			if (Strikethrough)
			{
				_formattingString.AppendFormat("strikethrough=on strikethrough_color={0} ",
					ForegroundColor.IsDefault ? ThemeConstants.Span.ColorClass.DefaultUnderLineColor.ToHex() : ForegroundColor.ToHex());
			}

			switch (HorizontalTextAlignment)
			{
				case TextAlignment.Auto:
					_formattingString.Append("align=auto ");
					break;

				case TextAlignment.Start:
					_formattingString.Append("align=left ");
					break;

				case TextAlignment.End:
					_formattingString.Append("align=right ");
					break;

				case TextAlignment.Center:
					_formattingString.Append("align=center ");
					break;

				case TextAlignment.None:
					break;
			}

			if (LineHeight != -1.0d)
			{
				_formattingString.Append($"linerelsize={(int)(LineHeight * 100)}%");
			}

			switch (LineBreakMode)
			{
				case LineBreakMode.HeadTruncation:
					_formattingString.Append("ellipsis=0.0");
					break;

				case LineBreakMode.MiddleTruncation:
					_formattingString.Append("ellipsis=0.5");
					break;

				case LineBreakMode.TailTruncation:
					_formattingString.Append("ellipsis=1.0");
					break;
				case LineBreakMode.None:
					break;
			}

			return _formattingString;
		}

		string ConvertTags(string text)
		{
			return text.Replace("&", "&amp;")
					   .Replace("<", "&lt;")
					   .Replace(">", "&gt;")
					   .Replace(Environment.NewLine, "<br>");
		}

		public string GetStyle()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("DEFAULT='");

			PrepareFormattingString(sb);

			sb.Append("'");

			return sb.ToString();
		}

		public static implicit operator Span(string text)
		{
			return new Span { Text = text };
		}
	}
}