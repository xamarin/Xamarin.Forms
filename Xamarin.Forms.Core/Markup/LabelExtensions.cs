namespace Xamarin.Forms.Markup
{
	public static class LabelExtensions
	{
		public static TLabel TextLeft<TLabel>(this TLabel label) where TLabel : Label
		{ label.HorizontalTextAlignment = TextAlignment.Start; return label; }

		public static TLabel TextCenterH<TLabel>(this TLabel label) where TLabel : Label
		{ label.HorizontalTextAlignment = TextAlignment.Center; return label; }

		public static TLabel TextRight<TLabel>(this TLabel label) where TLabel : Label
		{ label.HorizontalTextAlignment = TextAlignment.End; return label; }

		public static TLabel TextTop<TLabel>(this TLabel label) where TLabel : Label
		{ label.VerticalTextAlignment = TextAlignment.Start; return label; }

		public static TLabel TextCenterV<TLabel>(this TLabel label) where TLabel : Label
		{ label.VerticalTextAlignment = TextAlignment.Center; return label; }

		public static TLabel TextBottom<TLabel>(this TLabel label) where TLabel : Label
		{ label.VerticalTextAlignment = TextAlignment.End; return label; }

		public static TLabel TextCenter<TLabel>(this TLabel label) where TLabel : Label
			=> label.TextCenterH().TextCenterV();

		public static Label FontSize(this Label label, double fontSize) { label.FontSize = fontSize; return label; }

		public static Label Bold(this Label label) { label.FontAttributes = FontAttributes.Bold; return label; }

		public static Label Italic(this Label label) { label.FontAttributes = FontAttributes.Italic; return label; }

		public static TLabel FormattedText<TLabel>(this TLabel label, params Span[] spans) where TLabel : Label
		{
			label.FormattedText = new FormattedString();
			foreach (var span in spans)
				label.FormattedText.Spans.Add(span);
			return label;
		}
	}
}
