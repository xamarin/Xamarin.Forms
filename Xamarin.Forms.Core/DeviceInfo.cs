using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	public abstract class DeviceInfo : INotifyPropertyChanged, IDisposable
	{
		DeviceOrientation _currentOrientation;
		ScreenOrientation _screenOrientation;
		bool _disposed;

		internal DeviceOrientation CurrentOrientation
		{
			get { return _currentOrientation; }
			set
			{
				if (Equals(_currentOrientation, value))
					return;
				_currentOrientation = value;

				switch (value)
				{
					case DeviceOrientation.Portrait:
						ScreenOrientation = ScreenOrientation.Portrait;
						break;
					case DeviceOrientation.Landscape:
						ScreenOrientation = ScreenOrientation.Landscape;
						break;
					default:
						ScreenOrientation = ScreenOrientation.Other;
						break;
				}

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

	public enum ScreenOrientation
	{
		Other,
		Portrait,
		Landscape
	}
}