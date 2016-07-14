using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.ControlGallery.WindowsUniversal
{
	public sealed class BrokenImageSourceHandler : IImageSourceHandler
	{
		public Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource, CancellationToken cancellationToken = new CancellationToken())
		{
			throw new Exception("Fail");
		}
	}
}