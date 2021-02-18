using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Xamarin.Forms.Platform.WPF
{
	public static class FormattedStringExtensions
	{
		public static IEnumerable<Inline> ToInlines(this FormattedString formattedString)
		{
			foreach (Span span in formattedString.Spans)
				yield return span.ToRun();
		}

		public static Run ToRun(this Span span)
		{
			var run = new Run { Text = span.Text ?? string.Empty };

			if (span.TextColor != Color.Default)
				run.Foreground = span.TextColor.ToBrush();

			if (span.BackgroundColor != Color.Default)
				run.Background = span.BackgroundColor.ToBrush();

			if (!span.IsDefault())
			{
				run.FontFamily = span.FontFamily.ToFontFamily("FontFamilyNormal");
				run.FontSize = span.FontSize;
				run.FontWeight = span.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
				run.FontStyle = span.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyles.Italic : FontStyles.Normal;
			}

			if (!span.IsSet(Span.TextDecorationsProperty))
				return run;

			var textDecorations = span.TextDecorations;

			if ((textDecorations & TextDecorations.Underline) != 0)
				run.TextDecorations.Add(System.Windows.TextDecorations.Underline);

			if ((textDecorations & TextDecorations.Strikethrough) != 0)
				run.TextDecorations.Add(System.Windows.TextDecorations.Strikethrough);

			return run;
		}
	}

}
