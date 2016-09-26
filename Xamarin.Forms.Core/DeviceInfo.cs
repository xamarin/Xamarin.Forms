using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	internal abstract class DeviceInfo : INotifyPropertyChanged, IDisposable
	{
		DeviceOrientation _currentOrientation;
		bool _disposed;

		public DeviceOrientation CurrentOrientation
		{
			get { return _currentOrientation; }
			internal set
			{
				if (Equals(_currentOrientation, value))
					return;
				_currentOrientation = value;
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