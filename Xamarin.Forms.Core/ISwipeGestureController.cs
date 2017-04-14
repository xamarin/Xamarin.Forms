namespace Xamarin.Forms
{
	public interface ISwipeGestureController
	{
		double TotalX { get; }
		double TotalY { get; }
		void SendSwipe(Element sender, double totalX, double totalY);
	}
}