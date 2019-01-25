using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal abstract class NongreedySnapHelper : LinearSnapHelper
	{
		// Flag to indicate that the user has scrolled the view, so we can start snapping
		// (otherwise, this would start trying to snap the view as soon as we attached it to the RecyclerView)
		protected bool CanSnap { get; set; }

		public override int FindTargetSnapPosition(RecyclerView.LayoutManager layoutManager, int velocityX, int velocityY)
		{
			CanSnap = true;
			return base.FindTargetSnapPosition(layoutManager, velocityX, velocityY);
		}

		public override int[] CalculateScrollDistance(int velocityX, int velocityY)
		{
			CanSnap = true;
			return base.CalculateScrollDistance(velocityX, velocityY);
		}
	}
}