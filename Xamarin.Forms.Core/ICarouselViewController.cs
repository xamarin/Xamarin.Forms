namespace Xamarin.Forms
{
	public interface ICarouselViewController : IItemViewController
	{
		bool IgnorePositionUpdates { get; }
		void SendSelectedItemChanged(object item);
		void SendSelectedPositionChanged(int position);
	}
}