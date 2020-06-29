using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class StreamImageSourceHandler : IImageSourceHandler
	{
		internal static DependencyProperty Size = DependencyProperty.Register(nameof(Size), typeof(Size), typeof(StreamImageSourceHandler),
			new PropertyMetadata(default(Size), null));

		public async Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource, CancellationToken cancellationToken = new CancellationToken())
		{
			BitmapImage bitmapimage = null;

			if (imagesource is StreamImageSource streamsource && streamsource.Stream != null)
			{
				using (Stream stream = await ((IStreamImageSource)streamsource).GetStreamAsync(cancellationToken))
				{
					if (stream == null)
						return null;

					using (var randomAccessStream = stream.AsRandomAccessStream())
					{
						if (randomAccessStream is Windows.Media.Capture.CapturedFrame captureFrame)
						{
							var source = new SoftwareBitmapSource();
							var prep = SoftwareBitmap.Convert(captureFrame.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
							await source.SetBitmapAsync(prep);
							source.SetValue(Size, new Size(captureFrame.SoftwareBitmap.PixelWidth, captureFrame.SoftwareBitmap.PixelHeight));
							return source;
						}

						bitmapimage = new BitmapImage();
						await bitmapimage.SetSourceAsync(randomAccessStream);
					}
				}
			}

			return bitmapimage;
		}
	}
}