using ElmSharp;
using EScroller = ElmSharp.Scroller;

namespace Xamarin.Forms.Platform.Tizen.Native
{
	public class CarouselView : CollectionView, ICollectionViewController
	{
		public CarouselView(EvasObject parent) : base(parent)
		{
			SelectionMode = CollectionViewSelectionMode.Single;
			Scroll.ScrollBlock = ScrollBlock.None;
			Scroll.HorizontalPageScrollLimit = 1;
			Scroll.VerticalPageScrollLimit = 1;
			Scroll.SetPageSize(1.0, 1.0);
		}

		public EScroller Scroll => base.Scroller;
	}
}
