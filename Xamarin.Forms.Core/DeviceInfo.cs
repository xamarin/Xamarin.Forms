using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	public abstract class DeviceInfo : INotifyPropertyChanged, IDisposable
	{
		ScreenOrientation _screenOrientation;
		bool _disposed;

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

		public abstract Size PixelScreenSize { get; }

		public abstract Size ScaledScreenSize { get; }

		public abstract double ScalingFactor { get; }

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