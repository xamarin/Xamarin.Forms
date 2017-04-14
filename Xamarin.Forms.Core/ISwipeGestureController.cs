namespace Xamarin.Forms
{
	public interface ISwipeGestureController
	{
		void SendSwipe(Element sender, double totalX, double totalY, int gestureId);

		void SendSwipeCanceled(Element sender, int gestureId);

		void SendSwipeCompleted(Element sender, int gestureId);

		void SendSwipeStarted(Element sender, int gestureId);
	}
}