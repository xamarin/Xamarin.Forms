using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class MacOSDeviceInfo : DeviceInfo
	{
		readonly Size _scaledScreenSize;
		readonly double _scalingFactor;

		public MacOSDeviceInfo()
		{

			_scalingFactor = NSScreen.MainScreen.BackingScaleFactor;
			_scaledScreenSize = new Size(NSScreen.MainScreen.Frame.Width, NSScreen.MainScreen.Frame.Height);
			PixelScreenSize = new Size(_scaledScreenSize.Width * _scalingFactor, _scaledScreenSize.Height * _scalingFactor);
		}

		public override Size PixelScreenSize { get; }

		public override Size ScaledScreenSize
		{
			get { return _scaledScreenSize; }
		}

		public override double ScalingFactor
		{
			get { return _scalingFactor; }
		}
	}
}