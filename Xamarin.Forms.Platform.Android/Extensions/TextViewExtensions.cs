﻿using System;
using Android.Text;
using Android.Widget;
using System.Collections.Generic;
using Android.Support.V4.View;
using Java.Lang;
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

		static void SetMaxLines(this TextView textView, Label label, int lines)
		{
			// If the Label's MaxLines has been explicitly set, we should not set it here
			if (label.MaxLines != (int)Label.MaxLinesProperty.DefaultValue)
			{
				return;
			}

			textView.SetMaxLines(lines);
		}

		public static void SetLineBreakMode(this TextView textView, Label label)
		{
			var lineBreakMode = label.LineBreakMode;

			int maxLines = Int32.MaxValue;
			bool singleLine = false;

			switch (lineBreakMode)
			{
				case LineBreakMode.NoWrap:
					maxLines = 1;
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
					textView.Ellipsize = TextUtils.TruncateAt.End;
					break;
				case LineBreakMode.MiddleTruncation:
					maxLines = 1;
					singleLine = true; // Workaround for bug in older Android API versions (https://bugzilla.xamarin.com/show_bug.cgi?id=49069)
					textView.Ellipsize = TextUtils.TruncateAt.Middle;
					break;
			}

			textView.SetSingleLine(singleLine);
			textView.SetMaxLines(label, maxLines);
		}

		public static void RecalculateSpanPositions(this TextView textView, Label element, SpannableString spannableString, SizeRequest finalSize)
		{
			var layout = textView.Layout;
			if (layout == null)
				return;

			if (element?.FormattedText?.Spans == null || element.FormattedText.Spans.Count == 0)
				return;

			if (spannableString == null || spannableString.IsDisposed())
				return;

			var labelWidth = finalSize.Request.Width;
			if (labelWidth <= 0 || finalSize.Request.Height <= 0)
				return;

			var text = spannableString.ToString();

			int next = 0;
			int count = 0;
			IList<int> totalLineHeights = new List<int>();

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

				var startSpanOffset = spannableString.GetSpanStart(startSpan);
				var endSpanOffset = spannableString.GetSpanEnd(endSpan);

				var startX = layout.GetPrimaryHorizontal(startSpanOffset);
				var endX = layout.GetPrimaryHorizontal(endSpanOffset);

				var startLine = layout.GetLineForOffset(startSpanOffset);
				var endLine = layout.GetLineForOffset(endSpanOffset);

				double[] lineHeights = new double[endLine - startLine + 1];

				// calculate all the different line heights
				for (var lineCount = startLine; lineCount <= endLine; lineCount++)
				{
					var lineHeight = layout.GetLineBottom(lineCount) - layout.GetLineTop(lineCount);
					lineHeights[lineCount - startLine] = lineHeight;

					if (totalLineHeights.Count <= lineCount)
						totalLineHeights.Add(lineHeight);
				}

				var yaxis = 0.0;

				for (var line = startLine; line > 0; line--)
					yaxis += totalLineHeights[line];

				((ISpatialElement)span).Region = Region.FromLines(lineHeights, labelWidth, startX, endX, yaxis).Inflate(10);
			}
		}

		public static ICharSequence EllipsizeText(this TextView textView, ICharSequence originalText)
		{
			if (ViewCompat.IsLaidOut(textView) && originalText != null)
			{
				int textWidth = textView.Width - textView.CompoundPaddingLeft - textView.CompoundPaddingRight;
				return TextUtils.EllipsizeFormatted(originalText, textView.Paint, textWidth, TextUtils.TruncateAt.End);
			}

			return originalText;
		}
	}
}
