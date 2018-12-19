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
				var device = CanvasDevice.GetSharedDevice();
				var localDpi = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi;
				var canvasSize = (float)fontsource.Size + 2;
				var renderTarget = new CanvasRenderTarget(device, canvasSize, canvasSize, localDpi);

				using (var ds = renderTarget.CreateDrawingSession())
				{
					ds.Clear(Windows.UI.Colors.Transparent);
					var textFormat = new CanvasTextFormat
					{
						FontFamily = fontsource.FontFamily,
						FontSize = (float)fontsource.Size,
						HorizontalAlignment = CanvasHorizontalAlignment.Center,
						Options = CanvasDrawTextOptions.Default
					};
					var iconcolor = (fontsource.Color != Color.Default ? fontsource.Color : Color.White).ToWindowsColor();
					ds.DrawText(fontsource.Glyph.ToString(), textFormat.FontSize / 2, 0, iconcolor, textFormat);
				}

				using (var stream = new InMemoryRandomAccessStream())
				{
					await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
					image = new BitmapImage();
					await image.SetSourceAsync(stream);
				}
			}
			return image;
		}
	}
}
