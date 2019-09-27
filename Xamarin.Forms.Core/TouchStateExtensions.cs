namespace Xamarin.Forms
{
	public static class TouchStateExtensions
	{
		public static bool IsFinishedTouch(this TouchState state)
		{
			return state == TouchState.Released || state == TouchState.Cancelled || state == TouchState.Exited || state == TouchState.Failed;
		}

		public static bool IsTouching(this TouchState state)
		{
			return state == TouchState.Pressed || state == TouchState.Move || state == TouchState.Entered || state == TouchState.Hover ||
			       state == TouchState.Changed;
		}
	}
}