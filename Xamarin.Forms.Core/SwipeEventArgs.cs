using System;

namespace Xamarin.Forms
{
	public class SwipeLeftEventArgs : EventArgs
	{
		public SwipeLeftEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; private set; }
	}

	public class SwipeRightEventArgs : EventArgs
	{
		public SwipeRightEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; private set; }
	}

	public class SwipeUpEventArgs : EventArgs
	{
		public SwipeUpEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; private set; }
	}

	public class SwipeDownEventArgs : EventArgs
	{
		public SwipeDownEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; private set; }
	}


}