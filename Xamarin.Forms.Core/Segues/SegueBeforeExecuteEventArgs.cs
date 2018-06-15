using System;
namespace Xamarin.Forms
{
	public class SegueBeforeExecuteEventArgs : EventArgs
	{
		public SegueTarget Target { get; }
		public bool Cancelled { get; set; }
		public SegueBeforeExecuteEventArgs(SegueTarget target)
		{
			Target = target;
		}
	}
}
