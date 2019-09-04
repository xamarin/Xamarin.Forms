using System;

namespace Xamarin.Forms
{
	[Flags]
	public enum TouchState
	{
		Default = 0 << 0,
		Pressed = 1 << 1,
		Released = 1 << 2,
		Move = 1 << 3,
		Cancelled = 1 << 4,
		Failed = 1 << 5,
		Changed = 1 << 6,
		Entered = 1 << 7,
		Exited = 1 << 8,
		Hover = 1 << 9
	}

	public static class TouchStateExtensions
	{
		public static bool IsStartedTouch(this TouchState state)
		{
			return state == TouchState.Pressed || state == TouchState.Move || state == TouchState.Entered || state == TouchState.Hover || state == TouchState.Changed;
		}

		public static bool IsFinishedTouch(this TouchState state)
		{
			return state == TouchState.Released || state == TouchState.Cancelled || state == TouchState.Exited || state == TouchState.Failed;
		}
	}
}