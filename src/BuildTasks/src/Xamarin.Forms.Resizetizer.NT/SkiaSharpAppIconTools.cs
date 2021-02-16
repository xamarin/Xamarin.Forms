using System;
using System.Diagnostics;
using System.IO;
using SkiaSharp;

namespace Resizetizer
{
	internal class SkiaSharpAppIconTools
	{
		public SkiaSharpAppIconTools(SharedImageInfo info, ILogger logger)
		{
			Info = info;
			Logger = logger;

			AppIconName = Path.GetFileNameWithoutExtension(info.Filename);
			

			hasForeground = File.Exists(info.ForegroundFilename);

			if (hasForeground)
				foregroundTools = SkiaSharpTools.Create(info.ForegroundIsVector, info.ForegroundFilename, null, info.TintColor, logger);

			backgroundTools = SkiaSharpTools.Create(info.IsVector, info.Filename, null, null, logger);

			backgroundOriginalSize = backgroundTools.GetOriginalSize();

			if (hasForeground)
				foregroundOriginalSize = foregroundTools.GetOriginalSize();
		}

		bool hasForeground = false;

		SkiaSharpTools backgroundTools;
		SkiaSharpTools foregroundTools;

		SKSize foregroundOriginalSize;
		SKSize backgroundOriginalSize;

		public SharedImageInfo Info { get; }
		public ILogger Logger { get; }

		public string AppIconName { get; }

		public ResizedImageInfo Resize(DpiPath dpi, string destination)
		{
			var sw = new Stopwatch();
			sw.Start();

			var (bgScaledSize, bgScale) = backgroundTools.GetScaledSize(backgroundOriginalSize, dpi);

			// Allocate
			using (var tempBitmap = new SKBitmap(bgScaledSize.Width, bgScaledSize.Height))
			{
				// Draw (copy)
				using (var canvas = new SKCanvas(tempBitmap))
				{
					canvas.Clear(SKColors.Transparent);
					canvas.Save();
					canvas.Scale(bgScale, bgScale);

					backgroundTools.DrawUnscaled(canvas, bgScale);
					canvas.Restore();

					if (hasForeground)
					{
						var userFgScale = (float)Info.ForegroundScale;

						// get the ratio to make the foreground fill the background
						var fitRatio = bgScaledSize.Width / foregroundOriginalSize.Width;

						// calculate the scale for the foreground to fit the background exactly
						var (fgScaledSize, fgScale) = foregroundTools.GetScaledSize(foregroundOriginalSize, (decimal)fitRatio);

						Logger.Log("dpi.Size: " + dpi.Size);
						Logger.Log("dpi.Scale: " + dpi.Scale);
						Logger.Log("bgScaledSize: " + bgScaledSize);
						Logger.Log("bgScale: " + bgScale);
						Logger.Log("foregroundOriginalSize: " + foregroundOriginalSize);
						Logger.Log("fgScaledSize: " + fgScaledSize);
						Logger.Log("fgScale: " + fgScale);
						Logger.Log("userFgScale: " + userFgScale);

						// now work out the center as if the canvas was exactly the same size as the foreground
						var fgScaledSizeCenterX = foregroundOriginalSize.Width / 2;
						var fgScaledSizeCenterY = foregroundOriginalSize.Height / 2;

						Logger.Log("fgScaledSizeCenterX: " + fgScaledSizeCenterX);
						Logger.Log("fgScaledSizeCenterY: " + fgScaledSizeCenterY);

						// scale so the forground is the same size as the background
						canvas.Scale(fgScale, fgScale);

						// scale to the user scale, centering
						canvas.Scale(userFgScale, userFgScale, fgScaledSizeCenterX, fgScaledSizeCenterY);

						foregroundTools.DrawUnscaled(canvas, fgScale * userFgScale);
					}
				}

				// Save (encode)
				using var wrapper = File.Create(destination);
				tempBitmap.Encode(wrapper, SKEncodedImageFormat.Png, 100);
			}

			sw.Stop();
			Logger?.Log($"Save Image took {sw.ElapsedMilliseconds}ms");

			return new ResizedImageInfo { Dpi = dpi, Filename = destination };
		}
	}
}
