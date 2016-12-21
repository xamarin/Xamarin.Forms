using System;

namespace Xamarin.Forms
{
	public class SwipeEventArgs : EventArgs
	{
		public SwipeEventArgs(object parameter)
		{
			Parameter = parameter;
		}

		public object Parameter { get; private set; }
	}
}
