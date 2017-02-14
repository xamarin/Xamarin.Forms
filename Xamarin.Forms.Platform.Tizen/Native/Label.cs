using System;
using ElmSharp;
using ELabel = ElmSharp.Label;
using EColor = ElmSharp.Color;
using ESize = ElmSharp.Size;

namespace Xamarin.Forms.Platform.Tizen.Native
{
	/// <summary>
	/// The Label class extends <c>ElmSharp.Label</c> to be better suited for Xamarin renderers.
	/// Mainly the formatted text support.
	/// </summary>
	public class Label : ELabel, ITextable, IMeasurable
	{
		/// <summary>
		/// The _span holds the content of the label.
		/// </summary>
		readonly Span _span = new Span();

		/// <summary>
		/// Initializes a new instance of the <see cref="Xamarin.Forms.Platform.Tizen.Native.Label"/> class.
		/// </summary>
		/// <param name="parent">Parent evas object.</param>
		public Label(EvasObject parent) : base(parent)
		{
		}

		/// <summary>
		/// Get or sets the formatted text.
		/// </summary>
		/// <remarks>Setting <c>FormattedText</c> changes the value of the <c>Text</c> property.</remarks>
		/// <value>The formatted text.</value>
		public FormattedString FormattedText
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

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <remarks>Setting <c>Text</c> overwrites the value of the <c>FormattedText</c> property too.</remarks>
		/// <value>The content of the label.</value>
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

		/// <summary>
		/// Gets or sets the color of the formatted text.
		/// </summary>
		/// <value>The color of the text.</value>
		public EColor TextColor
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

		/// <summary>
		/// Gets or sets the background color for the text.
		/// </summary>
		/// <value>The color of the label's background.</value>
		public EColor TextBackgroundColor
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

		/// <summary>
		/// Gets or sets the font family for the text.
		/// </summary>
		/// <value>The font family.</value>
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

		/// <summary>
		/// Gets or sets the font attributes.
		/// </summary>
		/// <value>The font attributes.</value>
		public FontAttributes FontAttributes
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

		/// <summary>
		/// Gets or sets the font weight for the text.
		/// </summary>
		/// <value>The weight of the font.</value>
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

		/// <summary>
		/// Gets or sets the line wrap option.
		/// </summary>
		/// <value>The line break mode.</value>
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
					ApplyTextAndStyle();
				}
			}
		}

		/// <summary>
		/// Gets or sets the horizontal text alignment.
		/// </summary>
		/// <value>The horizontal text alignment.</value>
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

		/// <summary>
		/// Gets or sets the vertical text alignment.
		/// </summary>
		/// <value>The vertical text alignment.</value>
		public TextAlignment VerticalTextAlignment
		{
			// TODO. need to EFL new API
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value that indicates whether the text is underlined.
		/// </summary>
		/// <value><c>true</c> if the text is underlined.</value>
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

		/// <summary>
		/// Gets or sets the value that indicates whether the text is striked out.
		/// </summary>
		/// <value><c>true</c> if the text is striked out.</value>
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

		/// <summary>
		/// Implements <see cref="Xamarin.Forms.Platform.Tizen.Native.IMeasurable"/> to provide a desired size of the label.
		/// </summary>
		/// <param name="availableWidth">Available width.</param>
		/// <param name="availableHeight">Available height.</param>
		/// <returns>Size of the control that fits the available area.</returns>
		public ESize Measure(int availableWidth, int availableHeight)
		{
			var size = Geometry;

			Resize(availableWidth, size.Height);

			var formattedSize = Native.TextHelper.GetFormattedTextBlockSize(this);
			Resize(size.Width, size.Height);

			// Set bottom padding for lower case letters that have segments below the bottom line of text (g, j, p, q, y).
			var verticalPadding = (int)Math.Ceiling(0.05 * FontSize);
			formattedSize.Height += verticalPadding;

			// This is the EFL team's guide.
			// For wrap to work properly, the label must be 1 pixel larger than the size of the formatted text.
			formattedSize.Width += 1;

			return formattedSize;
		}

		void ApplyTextAndStyle()
		{
			SetInternalTextAndStyle(_span.GetDecoratedText(), _span.GetStyle());
		}

		void SetInternalTextAndStyle(string formattedText, string textStyle)
		{
			string emission = "elm,state,text,visible";

			if (string.IsNullOrEmpty(formattedText))
			{
				formattedText = null;
				textStyle = null;
				emission = "elm,state,text,hidden";
			}

			base.Text = formattedText;

			var textblock = EdjeObject["elm.text"];

			if (textblock != null)
			{
				textblock.TextStyle = textStyle;
			}

			EdjeObject.EmitSignal(emission, "elm");
		}
	}
}
