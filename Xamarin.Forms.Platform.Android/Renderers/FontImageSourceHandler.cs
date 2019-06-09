using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		private static Dictionary<string, Typeface> TypefaceCache = new Dictionary<string, Typeface>();

		public Task<Bitmap> LoadImageAsync(
			ImageSource imagesource,
			Context context,
			CancellationToken cancelationToken = default(CancellationToken))
		{
			Bitmap image = null;
			var fontsource = imagesource as FontImageSource;
			if (fontsource != null)
			{
				var paint = new Paint
				{
					TextSize = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)fontsource.Size, context.Resources.DisplayMetrics),
					Color = (fontsource.Color != Color.Default ? fontsource.Color : Color.White).ToAndroid(),
					TextAlign = Paint.Align.Left,
					AntiAlias = true,
				};

				if (!TypefaceCache.TryGetValue(fontsource.FontFamily, out Typeface typeface))
				{
					typeface = fontsource.FontFamily.ToTypeFace();
					TypefaceCache.Add(fontsource.FontFamily, typeface);
				}
				paint.SetTypeface(typeface);

				var width = (int)(paint.MeasureText(fontsource.Glyph) + .5f);
				var baseline = (int)(-paint.Ascent() + .5f);
				var height = (int)(baseline + paint.Descent() + .5f);
				image = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
				var canvas = new Canvas(image);
				canvas.DrawText(fontsource.Glyph, 0, baseline, paint);
			}

			return Task.FromResult(image);
		}
	}
}