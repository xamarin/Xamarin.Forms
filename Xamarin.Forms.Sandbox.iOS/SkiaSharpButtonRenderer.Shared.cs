using System.ComponentModel;
using SkiaSharp;
using Xamarin.Forms;
using System.Collections.Generic;

#if __ANDROID__
using SkiaSharp.Views.Android;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Android.SKPaintSurfaceEventArgs;
#elif __IOS__
using SkiaSharp.Views.iOS;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.iOS.SKPaintSurfaceEventArgs;
#endif

[assembly: ExportRenderer(typeof(Button), typeof(SkiaSharpVisual.SkiaSharpButtonRenderer), new[] { typeof(SkiaSharpVisual.SkiaSharp) })]

namespace SkiaSharpVisual
{
	public partial class SkiaSharpButtonRenderer
	{
		float _cornerRadius;
		SKPaint _backgroundFill;
		SKPaint _backgroundStroke;
		SKPaint _foregroundPaint;
		string _text;
		SKRect _textBounds;

		Dictionary<NamedSize, float> _fontSizes = new Dictionary<NamedSize, float>
		{
			{ NamedSize.Default, 14 },
			{ NamedSize.Large, 18 },
			{ NamedSize.Medium, 14 },
			{ NamedSize.Small, 12 },
			{ NamedSize.Micro, 10 },
		};

		void UpdatePaints(PropertyChangedEventArgs e = null)
		{
			if (_backgroundFill == null)
				_backgroundFill = new SKPaint
				{
					IsAntialias = true,
					Color = SKColors.Black,
				};
			if (_backgroundStroke == null)
				_backgroundStroke = new SKPaint
				{
					IsAntialias = true,
					Style = SKPaintStyle.Stroke,
					Color = SKColors.Transparent,
				};
			if (_foregroundPaint == null)
				_foregroundPaint = new SKPaint
				{
					IsAntialias = true,
					Color = SKColors.White,
				};

			if (e == null || e.PropertyName == Button.BackgroundColorProperty.PropertyName)
				_backgroundFill.Color = GetValueOrDefault(Button.BackgroundColorProperty, SKColors.Black);

			if (e == null || e.PropertyName == Button.CornerRadiusProperty.PropertyName)
				_cornerRadius = GetValueOrDefault(Button.CornerRadiusProperty, 0);

			if (e == null || e.PropertyName == Button.BorderColorProperty.PropertyName)
				_backgroundStroke.Color = GetValueOrDefault(Button.BorderColorProperty, SKColors.Transparent);

			if (e == null || e.PropertyName == Button.BorderWidthProperty.PropertyName)
				_backgroundStroke.StrokeWidth = (float)GetValueOrDefault(Button.BorderWidthProperty, 0.0);

			if (e == null || e.PropertyName == Button.TextColorProperty.PropertyName)
				_foregroundPaint.Color = GetValueOrDefault(Button.TextColorProperty, SKColors.White);

			if (e == null || e.PropertyName == Button.TextProperty.PropertyName)
			{
				_text = Element.Text;
				_textBounds = SKRect.Empty;
			}

			if (e == null || e.PropertyName == Button.FontProperty.PropertyName)
			{
				_textBounds = SKRect.Empty;

				var font = Element.Font;

				var style = SKFontStyle.Normal;
				if (font.FontAttributes == FontAttributes.Bold)
					style = SKFontStyle.Bold;
				else if (font.FontAttributes == FontAttributes.Italic)
					style = SKFontStyle.Italic;
				else if (font.FontAttributes == (FontAttributes.Bold | FontAttributes.Italic))
					style = SKFontStyle.BoldItalic;

				if (string.IsNullOrEmpty(font.FontFamily))
					_foregroundPaint.Typeface = SKTypeface.FromFamilyName(null, style);
				else
					_foregroundPaint.Typeface = SKTypeface.FromFamilyName(font.FontFamily, style);

				if (font.UseNamedSize)
					if (_fontSizes.ContainsKey(font.NamedSize))
						_foregroundPaint.TextSize = _fontSizes[font.NamedSize];
					else
						_foregroundPaint.TextSize = _fontSizes[NamedSize.Default];
				else
					_foregroundPaint.TextSize = (float)font.FontSize;
			}
		}

		void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;

			var scale = (float)Device.Info.ScalingFactor;
			var rect = new SKRect(0, 0, e.Info.Width / scale, e.Info.Height / scale);

			canvas.Clear(SKColors.Transparent);

			canvas.Scale(scale);

			canvas.DrawRoundRect(rect, _cornerRadius, _cornerRadius, _backgroundFill);

			if (_backgroundStroke.StrokeWidth > 0)
			{
				var half = _backgroundStroke.StrokeWidth / 2f;
				rect.Inflate(-half, -half);
				canvas.DrawRoundRect(rect, _cornerRadius, _cornerRadius, _backgroundStroke);
			}

			if (!string.IsNullOrEmpty(_text))
			{
				if (_textBounds == SKRect.Empty)
					_foregroundPaint.MeasureText(_text, ref _textBounds);

				var x = (rect.Width - _textBounds.Width) / 2f;
				var y = (rect.Height + _foregroundPaint.TextSize) / 2f;
				canvas.DrawText(_text, x, y, _foregroundPaint);
			}
		}

		SKSize GetMeasuredSize(SKSize size)
		{
			return new SKSize(40, 40);
		}

		SKColor GetValueOrDefault(BindableProperty property, SKColor defaultColor)
		{
			var def = (Color)property.DefaultValue;
			var color = (Color)Element.GetValue(property);

			if (color == Color.Default || color == def)
				return defaultColor;

			return new SKColor((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255));
		}

		T GetValueOrDefault<T>(BindableProperty property, T defaultValue)
		{
			var def = property.DefaultValue;
			var value = Element.GetValue(property);

			if (value == def)
				return defaultValue;

			return (T)value;
		}
	}
}
