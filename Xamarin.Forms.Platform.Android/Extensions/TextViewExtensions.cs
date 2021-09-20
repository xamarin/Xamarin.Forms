using System;
using System.Collections.Generic;
using Android.Text;
using Android.Widget;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	internal static class TextViewExtensions
	{
		public static void SetMaxLines(this TextView textView, Label label)
		{
			var maxLines = label.MaxLines;

			if (maxLines == (int)Label.MaxLinesProperty.DefaultValue)
			{
				// MaxLines is not explicitly set, so just let it be whatever gets set by LineBreakMode
				textView.SetLineBreakMode(label);
				return;
			}

			textView.SetMaxLines(maxLines);
		}

		public static void	SetLineBreakMode(this TextView textView, Label label)
		{
			var maxLines = SetLineBreak(textView, label.LineBreakMode);
			textView.SetMaxLines(maxLines);
		}

		public static void SetLineBreakMode(this TextView textView, Button button) =>
			SetLineBreak(textView, button.LineBreakMode);


		public static int SetLineBreak( TextView textView, LineBreakMode lineBreakMode)
		{
			int maxLines = Int32.MaxValue;
			bool singleLine = false;

			switch (lineBreakMode)
			{
				case LineBreakMode.NoWrap:
					maxLines = 1;
					singleLine = true;
					textView.Ellipsize = null;
					break;
				case LineBreakMode.WordWrap:
					textView.Ellipsize = null;
					break;
				case LineBreakMode.CharacterWrap:
					textView.Ellipsize = null;
					break;
				case LineBreakMode.HeadTruncation:
					maxLines = 1;
					singleLine = true; // Workaround for bug in older Android API versions (https://bugzilla.xamarin.com/show_bug.cgi?id=49069)
					textView.Ellipsize = TextUtils.TruncateAt.Start;
					break;
				case LineBreakMode.TailTruncation:
					maxLines = 1;
					singleLine = true;
					textView.Ellipsize = TextUtils.TruncateAt.End;
					break;
				case LineBreakMode.MiddleTruncation:
					maxLines = 1;
					singleLine = true; // Workaround for bug in older Android API versions (https://bugzilla.xamarin.com/show_bug.cgi?id=49069)
					textView.Ellipsize = TextUtils.TruncateAt.Middle;
					break;
			}

			textView.SetSingleLine(singleLine);
			return maxLines;
		}

		public static void RecalculateSpanPositions(this TextView textView, Label element, SpannableString spannableString, SizeRequest finalSize)
		{
			if (element?.FormattedText?.Spans == null || element.FormattedText.Spans.Count == 0)
				return;

			var labelWidth = finalSize.Request.Width;
			if (labelWidth <= 0 || finalSize.Request.Height <= 0)
				return;

			if (spannableString == null || spannableString.IsDisposed())
				return;

			var layout = textView.Layout;
			if (layout == null)
				return;

			int next = 0;
			int count = 0;

			var padding = element.Padding;
			var padLeft = (int)textView.Context.ToPixels(padding.Left);
			var padTop = (int)textView.Context.ToPixels(padding.Top);

			for (int i = 0; i < spannableString.Length(); i = next)
			{
				var type = Java.Lang.Class.FromType(typeof(Java.Lang.Object));

				var span = element.FormattedText.Spans[count];

				count++;

				if (string.IsNullOrEmpty(span.Text))
					continue;

				// find the next span
				next = spannableString.NextSpanTransition(i, spannableString.Length(), type);

				// get all spans in the range - Android can have overlapping spans				
				var spans = spannableString.GetSpans(i, next, type);

				var startSpan = spans[0];
				var endSpan = spans[spans.Length - 1];

				var spanStartOffset = spannableString.GetSpanStart(startSpan);
				var spanEndOffset = spannableString.GetSpanEnd(endSpan);

				var spanStartLine = layout.GetLineForOffset(spanStartOffset);
				var spanEndLine = layout.GetLineForOffset(spanEndOffset);

				// go through all lines that are affected by the span and calculate a rectangle for each
				var spanRectangles = new List<Rectangle>();
				for (var curLine = spanStartLine; curLine <= spanEndLine; curLine++)
				{
					global::Android.Graphics.Rect bounds = new global::Android.Graphics.Rect();
					layout.GetLineBounds(curLine, bounds);

					var lineHeight = bounds.Height();
					var lineStartOffset = layout.GetLineStart(curLine);
					var lineVisibleEndOffset = layout.GetLineVisibleEnd(curLine);

					var startOffset = (curLine == spanStartLine) ? spanStartOffset : lineStartOffset;
					var spanStartX = (int)layout.GetPrimaryHorizontal(startOffset);

					var endOffset = (curLine == spanEndLine) ? spanEndOffset : lineVisibleEndOffset;
					;
					var spanEndX = (int)layout.GetSecondaryHorizontal(endOffset);

					var spanWidth = spanEndX - spanStartX;
					var spanLeftX = spanStartX;
					// if rtl is used, startX would be bigger than endX
					if (spanStartX > spanEndX)
					{
						spanWidth = spanStartX - spanEndX;
						spanLeftX = spanEndX;
					}

					if (spanWidth > 1)
					{
						var rectangle = new Rectangle(spanLeftX + padLeft, bounds.Top + padTop, spanWidth, lineHeight);
						spanRectangles.Add(rectangle);
					}
				}

				((ISpatialElement)span).Region = Region.FromRectangles(spanRectangles).Inflate(10);
			}
		}
	}
}
