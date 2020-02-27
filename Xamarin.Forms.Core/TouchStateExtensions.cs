namespace Xamarin.Forms
{
	public static class TouchStateExtensions
	{
		public static bool IsFinishedTouch(this TouchState state)
		{
			return state == TouchState.Release || state == TouchState.Cancel || state == TouchState.Exit || state == TouchState.Fail;
		}

		public static bool IsTouching(this TouchState state)
		{
			return state == TouchState.Press || state == TouchState.Move || state == TouchState.Enter || state == TouchState.Hover ||
			       state == TouchState.Change;
		}
	}
}