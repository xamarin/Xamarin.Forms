using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class TouchPointEventArgs : GestureEventArgs
	{
		public Touch TouchData { get; }

		public TouchPointEventArgs(TouchEventArgs arg) : base(arg)
		{
		}
	}
}