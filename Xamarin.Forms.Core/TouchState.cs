using System;

namespace Xamarin.Forms
{
	[Flags]
	public enum TouchState
	{
		Default = 0 << 0,
		Press = 1 << 1,
		Release = 1 << 2,
		Move = 1 << 3,
		Cancel = 1 << 4,
		Fail = 1 << 5,
		Change = 1 << 6,
		Enter = 1 << 7,
		Exit = 1 << 8,
		Hover = 1 << 9
	}
}