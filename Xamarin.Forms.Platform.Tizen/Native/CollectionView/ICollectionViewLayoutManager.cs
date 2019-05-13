using ElmSharp;
using ESize = ElmSharp.Size;

namespace Xamarin.Forms.Platform.Tizen.Native
{
	public interface ICollectionViewLayoutManager
	{
		ICollectionViewController CollectionView { get; set; }

		bool IsHorizontal { get; }

		void SizeAllocated(ESize size);

		ESize GetScrollCanvasSize();

		void LayoutItems(Rect bound, bool force = false);

		Rect GetItemBound(int index);

		void ItemInserted(int index);

		void ItemRemoved(int index);

		void ItemUpdated(int index);

		void ItemSourceUpdated();

		void Reset();

		void ItemMeasureInvalidated(int index);
	}
}
