using Android.Text;
using Android.Widget;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Android
{
	internal static class TextViewExtensions
	{
		public static void SetLineBreakMode(this TextView textView, LineBreakMode lineBreakMode)
		{
			switch (lineBreakMode)
			{
				case LineBreakMode.NoWrap:
					textView.SetMaxLines(1);
					textView.SetSingleLine(true);
					textView.Ellipsize = null;
					break;
				case LineBreakMode.WordWrap:
					textView.Ellipsize = null;
					textView.SetMaxLines(100);
					textView.SetSingleLine(false);
					break;
				case LineBreakMode.CharacterWrap:
					textView.Ellipsize = null;
					textView.SetMaxLines(100);
					textView.SetSingleLine(false);
					break;
				case LineBreakMode.HeadTruncation:
					textView.SetMaxLines(1);
					textView.SetSingleLine(true);
					textView.Ellipsize = TextUtils.TruncateAt.Start;
					break;
				case LineBreakMode.TailTruncation:
					textView.SetMaxLines(1);
					textView.SetSingleLine(true);
					textView.Ellipsize = TextUtils.TruncateAt.End;
					break;
				case LineBreakMode.MiddleTruncation:
					textView.SetMaxLines(1);
					textView.SetSingleLine(true);
					textView.Ellipsize = TextUtils.TruncateAt.Middle;
					break;
			}
		}

		public static void RecalculateSpanPositions(this TextView textView, Label element, SpannableString spannableString, SizeRequest finalSize)
		{
			if (element?.FormattedText?.Spans == null
				|| element.FormattedText.Spans.Count == 0)
				return;

			var layout = textView.Layout;

			var text = spannableString.ToString();

			int next;
			int count = 0;
			IList<int> totalLineHeights = new List<int>();

			for (int i = 0; i < spannableString.Length(); i = next)
			{
				var span = element.FormattedText.Spans[count];

				// find the next span
				next = spannableString.NextSpanTransition(i, spannableString.Length(), Java.Lang.Class.FromType(typeof(Java.Lang.Object)));

				// get all spans in the range				
				var spans = spannableString.GetSpans(i, next, Java.Lang.Class.FromType(typeof(Java.Lang.Object)));

				var startSpan = spans[0];
				var endSpan = spans[spans.Length - 1];

				var startSpanOffset = spannableString.GetSpanStart(startSpan);
				var endSpanOffset = spannableString.GetSpanEnd(endSpan);

				var startX = layout.GetPrimaryHorizontal(startSpanOffset);
				var endX = layout.GetPrimaryHorizontal(endSpanOffset);

				var startLine = layout.GetLineForOffset(startSpanOffset);
				var endLine = layout.GetLineForOffset(endSpanOffset);

				var labelWidth = finalSize.Request.Width;

				double[] lineHeights = new double[endLine - startLine + 1];
			
				// calculate all the different line heights
				for (var lineCount = startLine; lineCount <= endLine; lineCount++)
				{
					var lineHeight = layout.GetLineBottom(lineCount) - layout.GetLineTop(lineCount);
					lineHeights[lineCount - startLine] = lineHeight;

					if (totalLineHeights.Count < lineCount)
						totalLineHeights.Add(lineHeight);
				}
				var yaxis = 0.0;
				for (var line = 0; line < totalLineHeights.Count; line++)
					yaxis += totalLineHeights[line];

				span.CalculatePositions(lineHeights, labelWidth, startX, endX, yaxis);

				count++;
			}
		}
	}
}
