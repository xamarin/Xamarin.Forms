namespace Xamarin.Forms
{
	public interface ICarouselViewController : IViewController
	{
		void NotifyPositionChanged(int newPosition);
		void SendScrolled(double value, ScrollDirection direction);
	}
}
