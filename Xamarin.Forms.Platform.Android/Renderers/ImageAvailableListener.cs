using System;

using Android.Media;
using AImage = Android.Media.Image;

namespace Xamarin.Forms.Platform.Android
{
	class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
	{
		public event EventHandler<byte[]> Photo;

		public void OnImageAvailable(ImageReader reader)
		{
			AImage image = null;

			try
			{
				image = reader.AcquireNextImage();
				var buffer = image.GetPlanes()[0].Buffer;
				var imageData = new byte[buffer.Capacity()];
				buffer.Get(imageData);

				Photo?.Invoke(this, imageData);
				buffer.Clear();
			}
			catch (Exception)
			{

			}
			finally
			{
				image?.Close();
			}
		}
	}
}