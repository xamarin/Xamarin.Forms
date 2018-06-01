using System.Collections;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	internal class CollectionViewAdapter : RecyclerView.Adapter
	{
		readonly ItemsView _itemsView;
		readonly Context _context;

		// TODO hartez 2018/05/29 17:06:45 Reconcile the type/name mismatch here	
		// This class should probably be something like ItemsViewAdapter
		// TODO hartez 2018/05/30 08:54:46 Instead of taking the ItemsView, this should take the ItemsSource directly	
		// TODO hartez 2018/05/30 08:55:12 The renderer should be watching the ItemsView for the ItemsSource being reset and create a new adapter	
		internal CollectionViewAdapter(ItemsView itemsView, Context context)
		{
			_itemsView = itemsView;
			_context = context;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			// TODO hartez 2018/05/30 08:41:05 This needs a wrapper which can interpret the ItemsSource as an IList for us	
			// TODO hartez 2018/05/31 09:26:18 Can we just use CollectionViewSource everywhere?	
			if (holder is ViewHolder textViewHolder && _itemsView.ItemsSource is IList list)
			{
				textViewHolder.TextView.Text = list[position].ToString();
			}
		}

		internal class ViewHolder : RecyclerView.ViewHolder
		{
			public TextView TextView { get; set; }

			public ViewHolder(TextView itemView) : base(itemView)
			{
				TextView = itemView;
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var view = new TextView(_context);
			return new ViewHolder(view);
		}

		// TODO hartez 2018/05/29 17:08:30 Very naive implementation, obviously we'll need something better	
		public override int ItemCount => (_itemsView.ItemsSource as IList).Count;
	}
}