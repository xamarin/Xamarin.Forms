using System;

using Foundation;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	internal class IOSDeviceInfo : DeviceInfo
	{
		Size _pixelScreenSize;
		Size _scaledScreenSize;
		double _scalingFactor;
		bool _disposed;
		readonly NSObject _notification;

		public IOSDeviceInfo()
		{
			_notification = UIDevice.Notifications.ObserveOrientationDidChange(OrientationChanged);

			UpdateScreenSize();
		}

		public override Size PixelScreenSize => _pixelScreenSize;
		public override Size ScaledScreenSize => _scaledScreenSize;

		public override double ScalingFactor => _scalingFactor;

		void UpdateScreenSize()
		{
			_scalingFactor = UIScreen.MainScreen.Scale;

			var boundsHeight = UIScreen.MainScreen.Bounds.Height;
			var boundsWidth = UIScreen.MainScreen.Bounds.Width;

			double height;
			double width;

			// We can't rely directly on the MainScreen bounds because they may not have been updated yet
			// But CurrentOrientation is up-to-date, so we can use it to work out the dimensions
			if (CurrentOrientation == DeviceOrientation.Other)
			{
				height = boundsHeight;
				width = boundsWidth;
			}
			else
			{
				width = CurrentOrientation.IsLandscape()
					? Math.Max(boundsHeight, boundsWidth)
					: Math.Min(boundsHeight, boundsWidth);

				height = CurrentOrientation.IsPortrait()
					? Math.Max(boundsHeight, boundsWidth)
					: Math.Min(boundsHeight, boundsWidth);
			}

			_scaledScreenSize = new Size(width, height);
			_pixelScreenSize = new Size(_scaledScreenSize.Width * _scalingFactor, _scaledScreenSize.Height * _scalingFactor);
		}

		void OrientationChanged(object sender, NSNotificationEventArgs args)
		{
			CurrentOrientation = UIDevice.CurrentDevice.Orientation.ToDeviceOrientation();
			UpdateScreenSize();
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				_notification.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
