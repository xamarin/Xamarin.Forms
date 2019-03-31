using Android.Support.V7.Widget;

namespace Xamarin.Forms.Platform.Android.CollectionView
{
	public class RecyclerViewScrollListener : RecyclerView.OnScrollListener
	{
		bool _disposed;
		int _horizontallOffset, _verticalOffset;
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

			// TODO: These offsets will be incorrect upon orientation or row dimension/count change.
			// They are currently provided in place of LinearLayoutManager's default offset calculation
			// because it does not report accurate values in the presence of uneven rows.
			// See https://stackoverflow.com/questions/27507715/android-how-to-get-the-current-x-offset-of-recyclerview
			_horizontallOffset += dx;
			_verticalOffset += dy;
			
			var linearLayoutManager = recyclerView.GetLayoutManager() as LinearLayoutManager;
			var firstVisibleItemIndex = linearLayoutManager.FindFirstVisibleItemPosition();
			var lastVisibleItemIndex = linearLayoutManager.FindLastVisibleItemPosition();
			var scrolledEventArgs = new Core.Items.ScrolledEventArgs(dx, dy, _horizontallOffset, _verticalOffset, firstVisibleItemIndex, lastVisibleItemIndex);

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