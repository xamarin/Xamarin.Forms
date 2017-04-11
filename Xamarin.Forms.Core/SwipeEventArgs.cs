using System;

namespace Xamarin.Forms
{
	public class SwipeEventArgs : EventArgs
	{
		public SwipeEventArgs(object parameter, SwipeDirection direction)
		{
			Parameter = parameter;
			Direction = direction;
		}

		public object Parameter { get; private set; }

		public SwipeDirection Direction { get; private set; }
	}

	public enum SwipeDirection
	{
		Left,
		Up,
		Right,
		Down
	}
}