using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		private class LayerProperties
		{
			public Paint Paint { get; set; }
			public int Baseline { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public string Glyph { get; set; }
		}

		public Task<Bitmap> LoadImageAsync(
			ImageSource imageSource,
			Context context,
			CancellationToken cancelationToken = default(CancellationToken))
		{
			var layerPropertiesList = new List<LayerProperties>();
			var maxWidth = 0;
			var maxHeight = 0;

			if (imageSource is LayeredFontImageSource layeredFontImageSource)
			{
				var baseSize = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)layeredFontImageSource.Size, context.Resources.DisplayMetrics);
				var baseColor = (layeredFontImageSource.Color != Color.Default ? layeredFontImageSource.Color : Color.White).ToAndroid();
				var baseFont = layeredFontImageSource.FontFamily.ToTypeFace();

				foreach (var layer in layeredFontImageSource.Layers)
				{
					var size = layer.IsSet(FontImageSource.SizeProperty) ?
						TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)layer.Size, context.Resources.DisplayMetrics) :
						baseSize;
					var color = layer.Color.IsDefault ? baseColor : layer.Color.ToAndroid();
					var font = layer.FontFamily == null ? baseFont : layer.FontFamily.ToTypeFace();
					var paint = new Paint
					{
						TextSize = size,
						Color = (layer.Color != Color.Default ? layer.Color : Color.White).ToAndroid(),
						TextAlign = Paint.Align.Left,
						AntiAlias = true,
					};

					paint.SetTypeface(font);

					var width = (int)(paint.MeasureText(layer.Glyph) + .5f);
					var baseline = (int)(-paint.Ascent() + .5f);
					var height = (int)(baseline + paint.Descent() + .5f);

					if (width > maxWidth)
					{
						maxWidth = width;
					}
					if (height > maxHeight)
					{
						maxHeight = height;
					}

					layerPropertiesList.Add(new LayerProperties { Paint = paint, Baseline = (int)(-paint.Ascent() + .5f), Width = width, Height = height, Glyph = layer.Glyph });
				}
			}
			else if (imageSource is FontImageSource fontSource)
			{
				var paint = new Paint
				{
					TextSize = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)fontSource.Size, context.Resources.DisplayMetrics),
					Color = (fontSource.Color != Color.Default ? fontSource.Color : Color.White).ToAndroid(),
					TextAlign = Paint.Align.Left,
					AntiAlias = true,
				};

				paint.SetTypeface(fontSource.FontFamily.ToTypeFace());

				var width = (int)(paint.MeasureText(fontSource.Glyph) + .5f);
				var baseline = (int)(-paint.Ascent() + .5f);
				var height = (int)(baseline + paint.Descent() + .5f);

				maxWidth = width;
				maxHeight = height;

				layerPropertiesList.Add(new LayerProperties { Paint = paint, Baseline = (int)(-paint.Ascent() + .5f), Width = width, Height = height, Glyph = fontSource.Glyph });
			}

			var image = Bitmap.CreateBitmap(maxWidth, maxHeight, Bitmap.Config.Argb8888);
			var canvas = new Canvas(image);

			foreach (var layer in layerPropertiesList)
			{
				var x = (maxWidth - layer.Width) / 2;
				var y = (maxHeight - layer.Height) / 2 + layer.Baseline;
				canvas.DrawText(layer.Glyph, x, y, layer.Paint);
			}

			return Task.FromResult(image);
		}
	}
}