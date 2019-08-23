

using System;

namespace Xamarin.Forms
{
	[Flags]
	public enum TouchState
	{
		Unknown = 0 << 0,
		Entered = 1 << 0,
		Exited = 1 << 2,
		Cancelled = 1 << 3,
		Failed = 1 << 4,
		Changed = 1 << 5,
		Pressed = 1 << 6,
		Released = 1 << 7,
		Hover = 1 << 8,
		Move = 1 << 9,
	}
}
