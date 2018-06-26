using System;

namespace Xamarin.Forms
{
	public class ScrolledDirectionEventArgs : EventArgs
	{
		public double NewValue { get; set; }
		public ScrollDirection Direction { get; set; }
	}
}