using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class FontImageSourceHandler : IImageSourceHandler
	{
		public async Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource,
			CancellationToken cancelationToken = default(CancellationToken))
		{
			BitmapSource image = null;
			if (imagesource is FontImageSource fontsource)
			{
				var iconcolor = fontsource.Color != Color.Default ? fontsource.Color : Color.White;
				var device = CanvasDevice.GetSharedDevice();
				var localDpi = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi;
				var canvasSize = (float)fontsource.Size + 2;
				var renderTarget = new CanvasRenderTarget(device, canvasSize, canvasSize, localDpi);

				using (var ds = renderTarget.CreateDrawingSession())
				{
					ds.Clear(Windows.UI.Colors.Transparent);
					var textFormat = new CanvasTextFormat
					{
						FontFamily = ToWindowsFontFamily(fontsource.FontFamily),
						FontSize = (float)fontsource.Size,
						HorizontalAlignment = CanvasHorizontalAlignment.Center,
						Options = CanvasDrawTextOptions.Default
					};
					ds.DrawText(fontsource.Glyph.ToString(), textFormat.FontSize / 2, 0, iconcolor.ToWindowsColor(), textFormat);
				}
				image = new BitmapImage();
				using (var stream = new InMemoryRandomAccessStream())
				{
					await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
					await image.SetSourceAsync(stream);
				}
			}
			return image;
		}

		string ToWindowsFontFamily(string fontFamily)
		{
			var ff = fontFamily.Split("#");
			var fontFile = ff[0];
			var fontName = string.IsNullOrEmpty(ff[1]) || ff.Length == 1 
				? System.IO.Path.GetFileNameWithoutExtension(fontFile) 
				: ff[1];
			return $"/Assets/{fontFile}#{fontName}";
		}
	}
}
