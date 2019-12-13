using System;
using System.Diagnostics;
using System.Text;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public static class FormattedStringExtensions
	{
		public static SpannableString ToAttributed(this FormattedString formattedString, Font defaultFont, Color defaultForegroundColor, TextView view)
		{
			if (formattedString == null)
				return null;

			var builder = new StringBuilder();
			for (int i = 0; i < formattedString.Spans.Count; i++)
			{
				Span span = formattedString.Spans[i];
				var text = span.Text;
				if (text == null)
					continue;

				builder.Append(text);
			}

			var spannable = new SpannableString(builder.ToString());

			// Modify to if condition to check on the parameter of text vertical alignment for Span
			// Now we get the biggest font size.
			double biggestFontSize = 0;
			for (int i = 0; i < formattedString.Spans.Count; i++)
			{
				Span span = formattedString.Spans[i];
				if (span.FontSize > biggestFontSize)
				{
					biggestFontSize = span.FontSize;
				}
			}

			var c = 0;
			Debug.WriteLine("Span count: " + formattedString.Spans.Count);
			for (int i = 0; i < formattedString.Spans.Count; i++)
			{
				Span span = formattedString.Spans[i];
				var text = span.Text;
				if (text == null)
					continue;

				int start = c;
				int end = start + text.Length;
				c = end;

				if (span.TextColor != Color.Default)
				{
					spannable.SetSpan(new ForegroundColorSpan(span.TextColor.ToAndroid()), start, end, SpanTypes.InclusiveExclusive);
				}
				else if (defaultForegroundColor != Color.Default)
				{
					spannable.SetSpan(new ForegroundColorSpan(defaultForegroundColor.ToAndroid()), start, end, SpanTypes.InclusiveExclusive);
				}

				if (span.BackgroundColor != Color.Default)
				{
					spannable.SetSpan(new BackgroundColorSpan(span.BackgroundColor.ToAndroid()), start, end, SpanTypes.InclusiveExclusive);
				}
				if (span.LineHeight >= 0)
				{
					spannable.SetSpan(new LineHeightSpan(view, span.LineHeight), start, end, SpanTypes.InclusiveExclusive);
				}
				if (!span.IsDefault())
#pragma warning disable 618 // We will need to update this when .Font goes away
					spannable.SetSpan(new FontSpan(span.Font, view, span.CharacterSpacing.ToEm()), start, end, SpanTypes.InclusiveInclusive);
#pragma warning restore 618
				else
					spannable.SetSpan(new FontSpan(defaultFont, view, span.CharacterSpacing.ToEm()), start, end, SpanTypes.InclusiveInclusive);
				if (span.IsSet(Span.TextDecorationsProperty))
					spannable.SetSpan(new TextDecorationSpan(span), start, end, SpanTypes.InclusiveInclusive);

				// Allen
				// Set span to be top alignment
				if (span.FontSize < biggestFontSize)
				{
					float shiftPercentage = (float)span.FontSize / (float)biggestFontSize;
					spannable.SetSpan(new TopAlignSuperscriptSpan((float)shiftPercentage), start, end, SpanTypes.InclusiveInclusive);
				}


				// spannable.SetSpan(new TopAlignSuperscriptSpan(0.9f), 3, 8, SpanTypes.InclusiveInclusive);
			}
			
			return spannable;
		}

		class FontSpan : MetricAffectingSpan
		{
			public FontSpan(Font font, TextView view, float characterSpacing)
			{
				Font = font;
				TextView = view;
				if (Forms.IsLollipopOrNewer)
				{
					CharacterSpacing = characterSpacing;
				}
			}

			public Font Font { get; }

			public TextView TextView { get; }

			public float CharacterSpacing { get; }

			public override void UpdateDrawState(TextPaint tp)
			{
				Apply(tp);
			}

			public override void UpdateMeasureState(TextPaint p)
			{
				Apply(p);
			}

			void Apply(Paint paint)
			{
				paint.SetTypeface(Font.ToTypeface());
				float value = Font.ToScaledPixel();
				paint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Sp, value, TextView.Resources.DisplayMetrics);
				if (Forms.IsLollipopOrNewer)
				{
					paint.LetterSpacing = CharacterSpacing;
				}
			}
		}

		class TextDecorationSpan : MetricAffectingSpan
		{
			public TextDecorationSpan(Span span)
			{
				Span = span;
			}

			public Span Span { get; }

			public override void UpdateDrawState(TextPaint tp)
			{
				Apply(tp);
			}

			public override void UpdateMeasureState(TextPaint p)
			{
				Apply(p);
			}

			void Apply(Paint paint)
			{
				var textDecorations = Span.TextDecorations;
				paint.UnderlineText = (textDecorations & TextDecorations.Underline) != 0;
				paint.StrikeThruText = (textDecorations & TextDecorations.Strikethrough) != 0;
			}
		}

		class LineHeightSpan : Java.Lang.Object, ILineHeightSpan
		{
			private double _lineHeight;
			private int _ascent;
			private int _descent;

			public LineHeightSpan(TextView view, double lineHeight)
			{
				_lineHeight = lineHeight;
				Paint.FontMetricsInt fm = view.Paint.GetFontMetricsInt();
				_ascent = fm.Ascent;
				_descent = fm.Descent;
			}

			public void ChooseHeight(Java.Lang.ICharSequence text, int start, int end, int spanstartv, int v, Paint.FontMetricsInt fm)
			{
				fm.Ascent = (int) (_ascent * _lineHeight);
				fm.Descent = (int) (_descent * _lineHeight);
			}
		}

		class TopAlignSuperscriptSpan : SuperscriptSpan
		{
			// protected int fontScale = 1;
			protected float shiftPercentage = 0;
			public TopAlignSuperscriptSpan() { }

			public TopAlignSuperscriptSpan(float shiftPercentage)
			{
				if (shiftPercentage > 0.0 && shiftPercentage < 1.0)
					this.shiftPercentage = shiftPercentage;
			}

			public override void UpdateDrawState(TextPaint textPaint)
			{
				// Use Ascent to do.
				float ascent = textPaint.Ascent();
				float totalAscent = ascent / shiftPercentage;
				int intTotalAscent = (int)totalAscent;
				int deltaAscent = (int)(intTotalAscent * (1.0f - shiftPercentage));

				textPaint.BaselineShift += deltaAscent;
			}

			public override void UpdateMeasureState(TextPaint textPaint)
			{
				UpdateDrawState(textPaint);
			}
		}
	}
}