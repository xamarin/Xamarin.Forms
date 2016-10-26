using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	internal class WindowsDeviceInfo : DeviceInfo
	{
		DisplayInformation _information;
		bool _isDisposed;

		public WindowsDeviceInfo()
		{
			// TODO: Screen size and DPI can change at any time
			_information = DisplayInformation.GetForCurrentView();
			ScreenOrientation = GetScreenOrientation(ApplicationView.GetForCurrentView().Orientation);
			_information.OrientationChanged += OnOrientationChanged;
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

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				_information.OrientationChanged -= OnOrientationChanged;
				_information = null;
			}

			_isDisposed = true;

			base.Dispose(disposing);
		}

		static ScreenOrientation GetScreenOrientation(ApplicationViewOrientation orientations)
		{
			switch (orientations)
			{
				case ApplicationViewOrientation.Landscape:
					return ScreenOrientation.Landscape;

				case ApplicationViewOrientation.Portrait:
					return ScreenOrientation.Portrait;

				default:
					return ScreenOrientation.Other;
			}
		}

		void OnOrientationChanged(DisplayInformation sender, object args)
		{
			ScreenOrientation = GetScreenOrientation(ApplicationView.GetForCurrentView().Orientation);
		}
	}
}