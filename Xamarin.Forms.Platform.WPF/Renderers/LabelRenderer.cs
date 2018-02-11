using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WPF
{
	public class LabelRenderer : ViewRenderer<Label, TextBlock>
	{
		bool _fontApplied;
		IList<double> _inlineHeights = new List<double>();

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new TextBlock());
				}

				// Update control property 
				UpdateText();
				UpdateColor();
				UpdateAlign();
				UpdateFont();
				UpdateLineBreakMode();
			}

			base.OnElementChanged(e);
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var size = base.GetDesiredSize(widthConstraint, heightConstraint);
			RecalculatePositions(size.Request);
			return size;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Label.TextProperty.PropertyName || e.PropertyName == Label.FormattedTextProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName || e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
				UpdateAlign();
			else if (e.PropertyName == Label.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
				UpdateLineBreakMode();
		}

		protected override void UpdateBackground()
		{
			Control.UpdateDependencyColor(TextBlock.BackgroundProperty, Element.BackgroundColor);
		}

		void UpdateAlign()
		{
			if (Control == null)
				return;

			Label label = Element;
			if (label == null)
				return;

			Control.TextAlignment = label.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		void UpdateColor()
		{
			if (Control == null || Element == null)
				return;
			
			if (Element.TextColor != Color.Default)
				Control.Foreground = Element.TextColor.ToBrush();
			else
				Control.Foreground = Brushes.Black; 
		}

		void UpdateFont()
		{
			if (Control == null)
				return;

			Label label = Element;
			if (label == null || (label.IsDefault() && !_fontApplied))
				return;

#pragma warning disable 618
			Font fontToApply = label.IsDefault() ? Font.SystemFontOfSize(NamedSize.Medium) : label.Font;
#pragma warning restore 618

			Control.ApplyFont(fontToApply);
			_fontApplied = true;
		}

		void UpdateLineBreakMode()
		{
			if (Control == null)
				return;

			switch (Element.LineBreakMode)
			{
				case LineBreakMode.NoWrap:
					Control.TextTrimming = TextTrimming.None;
					Control.TextWrapping = TextWrapping.NoWrap;
					break;
				case LineBreakMode.WordWrap:
					Control.TextTrimming = TextTrimming.None;
					Control.TextWrapping = TextWrapping.Wrap;
					break;
				case LineBreakMode.CharacterWrap:
					Control.TextTrimming = TextTrimming.CharacterEllipsis;
					Control.TextWrapping = TextWrapping.Wrap;
					break;
				case LineBreakMode.HeadTruncation:
					Control.TextTrimming = TextTrimming.WordEllipsis;
					Control.TextWrapping = TextWrapping.NoWrap;
					break;
				case LineBreakMode.TailTruncation:
					Control.TextTrimming = TextTrimming.CharacterEllipsis;
					Control.TextWrapping = TextWrapping.NoWrap;
					break;
				case LineBreakMode.MiddleTruncation:
					Control.TextTrimming = TextTrimming.WordEllipsis;
					Control.TextWrapping = TextWrapping.NoWrap;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		double FindDefaultLineHeight(Inline inline)
		{
			var control = new TextBlock();
			control.Inlines.Add(inline);

			control.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

			var height = control.DesiredSize.Height;

			control.Inlines.Remove(inline);
			control = null;

			return height;
		}

		void RecalculatePositions(Size finalSize)
		{
			if (Element?.FormattedText?.Spans == null
				|| Element.FormattedText.Spans.Count == 0)
				return;

			if (finalSize.Height == 0 || finalSize.Width == 0)
				return;

			for (int i = 0; i < Element.FormattedText.Spans.Count; i++)
			{
				var span = Element.FormattedText.Spans[i];

				var inline = Control.Inlines.ElementAt(i);
				var rect = inline.ContentStart.GetCharacterRect(LogicalDirection.Forward);
				var endRect = inline.ContentEnd.GetCharacterRect(LogicalDirection.Forward);

				var positions = new List<Rectangle>();
				var labelWidth = Control.ActualWidth;

				var defaultLineHeight = _inlineHeights[i];

				var yaxis = rect.Top;
				var lineHeights = new List<double>();
				while (yaxis < endRect.Bottom)
				{
					double lineHeight;
					if (yaxis == rect.Top) // First Line
					{
						lineHeight = rect.Bottom - rect.Top;
					}
					else if (yaxis != endRect.Top) // Middle Line(s)
					{
						lineHeight = defaultLineHeight;
					}
					else // Bottom Line
					{
						lineHeight = endRect.Bottom - endRect.Top;
					}
					lineHeights.Add(lineHeight);
					yaxis += lineHeight;
				}

				span.CalculatePositions(lineHeights.ToArray(), finalSize.Width, rect.X, endRect.X + endRect.Width, rect.Top);

			}
		}

		void UpdateText()
		{
			if (Control == null)
				return;

			Label label = Element;
			if (label != null)
			{
				if (label.FormattedText == null)
					Control.Text = label.Text;
				else
				{
					FormattedString formattedText = label.FormattedText ?? label.Text;

					Control.Inlines.Clear();
					// Have to implement a measure here, otherwise inline.ContentStart and ContentEnd will be null, when used in RecalculatePositions
					Control.Measure(new System.Windows.Size(double.MaxValue, double.MaxValue));

					var heights = new List<double>();
					for (var i = 0; i < formattedText.Spans.Count; i++)
					{
						var span = formattedText.Spans[i];
						if (span.Text != null)
						{
							var run = span.ToRun();
							heights.Add(FindDefaultLineHeight(run));
							Control.Inlines.Add(run);
						}
					}
					_inlineHeights = heights;
				}
			}
		}
		
	}
}
