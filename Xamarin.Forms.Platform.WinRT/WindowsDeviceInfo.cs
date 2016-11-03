using System;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	class WindowsDeviceInfo : DeviceInfo
	{
		DisplayInformation _information;
		SimpleOrientationSensor _simpleOrientationSensor;
		bool _isDisposed;

		public WindowsDeviceInfo()
		{
			// TODO: Screen size and DPI can change at any time
			_information = DisplayInformation.GetForCurrentView();

			// initialize screen orientation
			SetScreenOrientation(ApplicationView.GetForCurrentView().Orientation);
			_information.OrientationChanged += OnScreenOrientationChanged;
		}

		public override Size PixelScreenSize
		{
			get
			{
				double scaling = ScalingFactor;
				Size scaled = ScaledScreenSize;
				double width = Math.Round(scaled.Width * scaling);
				double height = Math.Round(scaled.Height * scaling);

				return new Size(width, height);
			}
		}

		public override Size ScaledScreenSize
		{
			get
			{
				Rect windowSize = Window.Current.Bounds;
				return new Size(windowSize.Width, windowSize.Height);
			}
		}

		public override double ScalingFactor
		{
			get
			{
				ResolutionScale scale = _information.ResolutionScale;
				switch (scale)
				{
					case ResolutionScale.Scale120Percent:
						return 1.2;
					case ResolutionScale.Scale140Percent:
						return 1.4;
					case ResolutionScale.Scale150Percent:
						return 1.5;
					case ResolutionScale.Scale160Percent:
						return 1.6;
					case ResolutionScale.Scale180Percent:
						return 1.8;
					case ResolutionScale.Scale225Percent:
						return 2.25;
					case ResolutionScale.Scale100Percent:
					default:
						return 1;
				}
			}
		}

		public override void BeginDeviceOrientationNotifications()
		{
			if (_simpleOrientationSensor != null)
				return;

			_simpleOrientationSensor = SimpleOrientationSensor.GetDefault();
			_simpleOrientationSensor.OrientationChanged += OnDeviceOrientationChanged;
		}

		public override void EndDeviceOrientationNotifications()
		{
			if (_simpleOrientationSensor == null)
				return;

			_simpleOrientationSensor.OrientationChanged -= OnDeviceOrientationChanged;
			_simpleOrientationSensor = null;
		}

		void OnDeviceOrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
		{
			SetDeviceOrientation(args.Orientation);
		}

		void OnScreenOrientationChanged(DisplayInformation sender, object args)
		{
			SetScreenOrientation(ApplicationView.GetForCurrentView().Orientation);
		}

		async void SetDeviceOrientation(SimpleOrientation orientation)
		{
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
			() =>
			{
				switch (orientation)
				{
					case SimpleOrientation.Rotated90DegreesCounterclockwise:
						DeviceOrientation = _information.NativeOrientation == DisplayOrientations.Portrait ? DeviceOrientation.Landscape : DeviceOrientation.PortraitFlipped;
						break;
					case SimpleOrientation.Rotated180DegreesCounterclockwise:
						DeviceOrientation = _information.NativeOrientation == DisplayOrientations.Portrait ? DeviceOrientation.PortraitFlipped : DeviceOrientation.LandscapeFlipped;
						break;
					case SimpleOrientation.Rotated270DegreesCounterclockwise:
						DeviceOrientation = _information.NativeOrientation == DisplayOrientations.Portrait ? DeviceOrientation.LandscapeFlipped : DeviceOrientation.Portrait;
						break;
					case SimpleOrientation.NotRotated:
						DeviceOrientation = _information.NativeOrientation == DisplayOrientations.Portrait ? DeviceOrientation.Portrait : DeviceOrientation.Landscape;
						break;
					// there is no "Unknown" state currently so we will ignore this check
					default:
						DeviceOrientation = DeviceOrientation.Other;
						break;
				}
			}
			);
		}

		void SetScreenOrientation(ApplicationViewOrientation orientations)
		{
			switch (orientations)
			{
				case ApplicationViewOrientation.Landscape:
					ScreenOrientation = ScreenOrientation.Landscape;
					break;
				case ApplicationViewOrientation.Portrait:
					ScreenOrientation = ScreenOrientation.Portrait;
					break;
				// there is no "Unknown" state currently so we will ignore this check
				default:
					ScreenOrientation = ScreenOrientation.Other;
					break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				_information.OrientationChanged -= OnScreenOrientationChanged;
				_information = null;
				EndDeviceOrientationNotifications();
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}
	}
}