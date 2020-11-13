using System;
using ElmSharp;
using EButton = ElmSharp.Button;

namespace Xamarin.Platform.Tizen
{
	public class Button : EButton, IMeasurable, IBatchable
	{
		public Button(EvasObject parent) : base(parent)
		{
		}

		readonly Span _span = new Span();

		Image? _image;

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

		public Image Image
		{
#pragma warning disable CS8603 // Possible null reference return.
			get => _image;
#pragma warning restore CS8603 // Possible null reference return.
			set
			{
				if (value != _image)
				{
					ApplyImage(value);
				}
			}
		}

		public virtual Size Measure(int availableWidth, int availableHeight)
		{
			if (DeviceInfo.IsWatch)
			{
				if (Style == ThemeConstants.Button.Styles.Default)
				{
					//Should gurantee the finger size (40)
					MinimumWidth = MinimumWidth < 40 ? 40 : MinimumWidth;
					if (Image != null)
						MinimumWidth += Image.Geometry.Width;
					var rawSize = this.GetTextBlockNativeSize();
					return new Size(rawSize.Width + MinimumWidth, Math.Max(MinimumHeight, rawSize.Height));
				}
				else
				{
					return new Size(MinimumWidth, MinimumHeight);
				}
			}
			else
			{
				if (Style == ThemeConstants.Button.Styles.Circle)
				{
					return new Size(MinimumWidth, MinimumHeight);
				}
				else
				{
					if (Image != null)
						MinimumWidth += Image.Geometry.Width;

					var rawSize = this.GetTextBlockNativeSize();
					return new Size(rawSize.Width + MinimumWidth, Math.Max(MinimumHeight, rawSize.Height));
				}
			}
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
			bool isVisible = true;
			if (string.IsNullOrEmpty(formattedText))
			{
				base.Text = null;
				this.SetTextBlockStyle(null);
				this.SendTextVisibleSignal(false);
			}
			else
			{
				base.Text = formattedText;
				this.SetTextBlockStyle(textStyle);
				this.SendTextVisibleSignal(isVisible);
			}
		}

		void ApplyImage(Image image)
		{
			_image = image;

			SetInternalImage();
		}

		void SetInternalImage()
		{
#pragma warning disable CS8604 // Possible null reference argument.
			this.SetIconPart(_image);
#pragma warning restore CS8604 // Possible null reference argument.
		}

		public void UpdateStyle(string style)
		{
			if (Style != style)
			{
				Style = style;
				if (Style == ThemeConstants.Button.Styles.Default)
					_span.HorizontalTextAlignment = TextAlignment.Auto;
				else
					_span.HorizontalTextAlignment = TextAlignment.Center;
				ApplyTextAndStyle();
			}
		}
	}
}