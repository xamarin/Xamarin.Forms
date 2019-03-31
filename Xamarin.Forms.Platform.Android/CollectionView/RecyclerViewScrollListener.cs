using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android.CollectionView
{
	public class RecyclerViewScrollListener : RecyclerView.OnScrollListener
	{
		bool _disposed;
		ItemsView _itemsView;
		ItemsViewAdapter _itemsViewAdapter;

		public RecyclerViewScrollListener(ItemsView itemsView, ItemsViewAdapter itemsViewAdapter)
		{
			_itemsView = itemsView;
			_itemsViewAdapter = itemsViewAdapter;
		}

		public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
		{
			base.OnScrollStateChanged(recyclerView, newState);
		}

		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{
			base.OnScrolled(recyclerView, dx, dy);

			var linearLayoutManager = recyclerView.GetLayoutManager() as LinearLayoutManager;
			var firstVisibleItemIndex = linearLayoutManager.FindFirstVisibleItemPosition();
			var lastVisibleItemIndex = linearLayoutManager.FindLastVisibleItemPosition();
			var scrolledEventArgs = new Core.Items.ScrolledEventArgs(dx, dy, recyclerView.ComputeHorizontalScrollOffset(), recyclerView.ComputeVerticalScrollOffset(), firstVisibleItemIndex, lastVisibleItemIndex);

			_itemsView.SendScrolled(scrolledEventArgs);

			switch (_itemsView.RemainingItemsThreshold)
			{
				case -1:
					return;
				case 0:
					if (firstVisibleItemIndex == _itemsViewAdapter.ItemCount - 1)
						_itemsView.SendRemainingItemsThresholdReached();
					break;
				default:
					if (_itemsViewAdapter.ItemCount - 1 - firstVisibleItemIndex <= _itemsView.RemainingItemsThreshold)
						_itemsView.SendRemainingItemsThresholdReached();
					break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				_itemsView = null;
				_itemsViewAdapter = null;
			}

			_disposed = true;
		}
	}
}