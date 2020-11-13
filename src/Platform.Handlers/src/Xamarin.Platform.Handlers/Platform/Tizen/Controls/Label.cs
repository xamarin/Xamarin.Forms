using System;
using ElmSharp;
using ELabel = ElmSharp.Label;

namespace Xamarin.Platform.Tizen
{
	public class Label : ELabel, IMeasurable, IBatchable
	{
		readonly Span _span = new Span();

		public Label(EvasObject parent) : base(parent)
		{
		}

		public FormattedString? FormattedText
		{
			get
			{
				return _span.FormattedText;
			}

			set
			{
				if (value != _span.FormattedText)
				{
					_span.FormattedText = value;
					ApplyTextAndStyle();
				}
			}
		}

		public override string Text
		{
			get
			{
				return _span.Text;
			}

			set
			{
				if (value != _span.Text)
				{
					_span.Text = value;
					ApplyTextAndStyle();
				}
			}
		}

		public Color TextColor
		{
			get
			{
				return _span.ForegroundColor;
			}

			set
			{
				if (!_span.ForegroundColor.Equals(value))
				{
					_span.ForegroundColor = value;
					ApplyTextAndStyle();
				}
			}
		}

		public Color TextBackgroundColor
		{
			get
			{
				return _span.BackgroundColor;
			}

			set
			{
				if (!_span.BackgroundColor.Equals(value))
				{
					_span.BackgroundColor = value;
					ApplyTextAndStyle();
				}
			}
		}

		public string FontFamily
		{
			get
			{
				return _span.FontFamily;
			}

			set
			{
				if (value != _span.FontFamily)
				{
					_span.FontFamily = value;
					ApplyTextAndStyle();
				}
			}
		}

		public Forms.FontAttributes FontAttributes
		{
			get
			{
				return _span.FontAttributes;
			}

			set
			{
				if (value != _span.FontAttributes)
				{
					_span.FontAttributes = value;
					ApplyTextAndStyle();
				}
			}
		}

		/// <summary>
		/// Gets or sets the font size for the text.
		/// </summary>
		/// <value>The size of the font.</value>
		public double FontSize
		{
			get
			{
				return _span.FontSize;
			}

			set
			{
				if (value != _span.FontSize)
				{
					_span.FontSize = value;
					ApplyTextAndStyle();
				}
			}
		}

		public double LineHeight
		{
			get
			{
				return _span.LineHeight;
			}
			set
			{
				if (value != _span.LineHeight)
				{
					_span.LineHeight = value;
					ApplyTextAndStyle();
				}
			}
		}

		public string FontWeight
		{
			get
			{
				return _span.FontWeight;
			}

			set
			{
				if (value != _span.FontWeight)
				{
					_span.FontWeight = value;
					ApplyTextAndStyle();
				}
			}
		}

		public LineBreakMode LineBreakMode
		{
			get
			{
				return _span.LineBreakMode;
			}

			set
			{
				if (value != _span.LineBreakMode)
				{
					_span.LineBreakMode = value;
					switch (value)
					{
						case LineBreakMode.NoWrap:
							LineWrapType = WrapType.None;
							IsEllipsis = false;
							break;
						case LineBreakMode.CharacterWrap:
							LineWrapType = WrapType.Char;
							IsEllipsis = false;
							break;
						case LineBreakMode.WordWrap:
							LineWrapType = WrapType.Word;
							IsEllipsis = false;
							break;
						case LineBreakMode.MixedWrap:
							LineWrapType = WrapType.Mixed;
							IsEllipsis = false;
							break;
						default:
							LineWrapType = WrapType.None;
							IsEllipsis = true;
							break;
					}
					ApplyTextAndStyle();
				}
			}
		}

		public TextAlignment HorizontalTextAlignment
		{
			get
			{
				return _span.HorizontalTextAlignment;
			}

			set
			{
				if (value != _span.HorizontalTextAlignment)
				{
					_span.HorizontalTextAlignment = value;
					ApplyTextAndStyle();
				}
			}
		}

		public TextAlignment VerticalTextAlignment
		{
			// README: It only work on Tizen 4.0
			get
			{
				double valign = this.GetVerticalTextAlignment();
				if (valign == 0.0)
				{
					return TextAlignment.Start;
				}
				else if (valign == 0.5)
				{
					return TextAlignment.Center;
				}
				else if (valign == 1.0)
				{
					return TextAlignment.End;
				}
				else
				{
					return TextAlignment.Auto;
				}
			}
			set
			{
				double valign = 0;
				switch (value)
				{
					case TextAlignment.Auto:
						valign = -1;
						break;
					case TextAlignment.None:
					case TextAlignment.Start:
						valign = 0;
						break;
					case TextAlignment.Center:
						valign = 0.5;
						break;
					case TextAlignment.End:
						valign = 1.0;
						break;
				}
				this.SetVerticalTextAlignment(valign);
			}
		}

		public bool Underline
		{
			get
			{
				return _span.Underline;
			}

			set
			{
				if (value != _span.Underline)
				{
					_span.Underline = value;
					ApplyTextAndStyle();
				}
			}
		}
		public bool Strikethrough
		{
			get
			{
				return _span.Strikethrough;
			}

			set
			{
				if (value != _span.Strikethrough)
				{
					_span.Strikethrough = value;
					ApplyTextAndStyle();
				}
			}
		}

		public Size Measure(int availableWidth, int availableHeight)
		{
			var size = Geometry;

			Resize(availableWidth, size.Height);

			var formattedSize = this.GetTextBlockFormattedSize();
			Resize(size.Width, size.Height);

			// Set bottom padding for lower case letters that have segments below the bottom line of text (g, j, p, q, y).
			var verticalPadding = (int)Math.Ceiling(0.05 * FontSize);
			formattedSize.Height += verticalPadding;

			// This is the EFL team's guide.
			// For wrap to work properly, the label must be 1 pixel larger than the size of the formatted text.
			formattedSize.Width += 1;

			return formattedSize;
		}

		void IBatchable.OnBatchCommitted()
		{
			ApplyTextAndStyle();
		}

		void ApplyTextAndStyle()
		{
			if (!this.IsBatched())
			{
				SetInternalTextAndStyle(_span.GetDecoratedText(), _span.GetStyle());
			}
		}

		void SetInternalTextAndStyle(string formattedText, string textStyle)
		{
			base.Text = formattedText;
			TextStyle = textStyle;
		}
	}
}
