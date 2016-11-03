using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	public abstract class DeviceInfo : INotifyPropertyChanged, IDisposable
	{
		DeviceOrientation _deviceOrientation;
		ScreenOrientation _screenOrientation;
		LayoutOrientation _layoutOrientation;
		bool _disposed;

		public DeviceOrientation DeviceOrientation
		{
			get { return _deviceOrientation; }
			internal set
			{
				if (Equals(_deviceOrientation, value))
					return;

				_deviceOrientation = value;
				OnPropertyChanged();
			}
		}

		public ScreenOrientation ScreenOrientation
		{
			get { return _screenOrientation; }
			internal set
			{
				if (Equals(_screenOrientation, value))
					return;

				_screenOrientation = value;
				OnPropertyChanged();
			}
		}

		public LayoutOrientation LayoutOrientation
		{
			get { return _layoutOrientation; }
			internal set
			{
				if (Equals(_layoutOrientation, value))
					return;

				_layoutOrientation = value;
				OnPropertyChanged();
			}
		}

		public abstract Size PixelScreenSize { get; }

		public abstract Size ScaledScreenSize { get; }

		public abstract double ScalingFactor { get; }

		public abstract void BeginDeviceOrientationNotifications();

		public abstract void EndDeviceOrientationNotifications();

		public void Dispose()
		{
			Dispose(true);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			_disposed = true;
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}