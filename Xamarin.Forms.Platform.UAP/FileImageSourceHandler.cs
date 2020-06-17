﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using IOPath = System.IO.Path;

namespace Xamarin.Forms.Platform.UWP
{
	public sealed class FileImageSourceHandler : IImageSourceHandler, IIconElementHandler
	{
		public Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource, CancellationToken cancellationToken = new CancellationToken())
		{
			Windows.UI.Xaml.Media.ImageSource image = null;
			if (imagesource is FileImageSource filesource)
			{
				UpdateImageDirectory(filesource);
				string file = filesource.File;
				image = new BitmapImage(new Uri("ms-appx:///" + file));
			}

			return Task.FromResult(image);
		}

		public Task<Microsoft.UI.Xaml.Controls.IconSource> LoadIconSourceAsync(ImageSource imagesource, CancellationToken cancellationToken = default(CancellationToken))
		{
			Microsoft.UI.Xaml.Controls.IconSource image = null;

			if (imagesource is FileImageSource filesource)
			{
				UpdateImageDirectory(filesource);
				string file = filesource.File;
				image = new Microsoft.UI.Xaml.Controls.BitmapIconSource { UriSource = new Uri("ms-appx:///" + file) };
			}

			return Task.FromResult(image);
		}

		public Task<IconElement> LoadIconElementAsync(ImageSource imagesource, CancellationToken cancellationToken = default(CancellationToken))
		{
			IconElement image = null;

			if (imagesource is FileImageSource filesource)
			{
				UpdateImageDirectory(filesource);
				string file = filesource.File;
				image = new BitmapIcon { UriSource = new Uri("ms-appx:///" + file) };
			}

			return Task.FromResult(image);
		}

		void UpdateImageDirectory(FileImageSource fileSource)
		{
			if (fileSource == null || fileSource.File == null)
				return;

			var imageDirectory = Application.Current.OnThisPlatform().GetImageDirectory();

			if (!string.IsNullOrEmpty(imageDirectory))
			{
				var filePath = fileSource.File;

				var directory = IOPath.GetDirectoryName(filePath);

				if (string.IsNullOrEmpty(directory) || !IOPath.GetFullPath(directory).Equals(IOPath.GetFullPath(imageDirectory)))
				{
					filePath = IOPath.Combine(imageDirectory, filePath);
					fileSource.File = filePath;
				}
			}
		}
	}
}