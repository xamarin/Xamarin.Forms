using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android
{
	// TODO hartez 2018/07/25 14:39:29 Split up CollectionViewAdapter into one for templates, one for text	
	// TODO hartez 2018/07/25 14:43:04 Experiment with an ItemSource property change as _adapter.notifyDataSetChanged	
	// TODO hartez 2018/07/25 14:44:15 Template property changed should do a whole new adapter; and that way we can cache the template

	internal class CollectionViewAdapter : RecyclerView.Adapter
	{
		readonly ItemsView _itemsView;
		readonly Context _context;
		readonly ICollectionViewSource _itemSource;

		// TODO hartez 2018/05/29 17:06:45 Reconcile the type/name mismatch here	
		// This class should probably be something like ItemsViewAdapter
		internal CollectionViewAdapter(ItemsView itemsView, Context context)
		{
			_itemsView = itemsView;
			_context = context;
			_itemSource = ItemsSourceFactory.Create(itemsView.ItemsSource, this);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			switch (holder)
			{
				case TextViewHolder textViewHolder:
					textViewHolder.TextView.Text = _itemSource[position].ToString();
					break;
				case TemplatedItemViewHolder templateViewHolder:
					BindableObject.SetInheritedBindingContext(templateViewHolder.View, _itemSource[position]);
					// TODO hartez 2018/07/25 16:12:30 Remove this next line once the platform PRs go through and we can rebase	
					templateViewHolder.View.Platform = new Platform(_context);
					break;
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			// Does the ItemsView have a DataTemplate?
			var template = _itemsView.ItemTemplate;
			if (template == null)
			{
				// No template, just use the ToString view
				var view = new TextView(_context);
				return new TextViewHolder(view);
			}

			// Realize the content, create a renderer out of it, and use that
			var templateElement = template.CreateContent() as View;

			var itemContentControl = new ItemContentControl(CreateRenderer(templateElement), _context);

			return new TemplatedItemViewHolder(itemContentControl, templateElement);
		}

		public IVisualElementRenderer CreateRenderer(View view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));

			var renderer = Platform.CreateRenderer(view, _context);
			Platform.SetRenderer(view, renderer);

			return renderer;
		}

		public override int ItemCount => _itemSource.Count;

		public override int GetItemViewType(int position)
		{
			// TODO hartez We might be able to turn this to our own purposes
			// We might be able to have the CollectionView signal the adapter if the ItemTemplate property changes
			// And as long as it's null, we return a value to that effect here
			// Then we don't have to check _itemsView.ItemTemplate == null in OnCreateViewHolder, we can just use
			// the viewType parameter.
			return 42;
		}

		internal class TextViewHolder : RecyclerView.ViewHolder
		{
			public TextView TextView { get; }

			public TextViewHolder(TextView itemView) : base(itemView)
			{
				TextView = itemView;
			}
		}

		internal class TemplatedItemViewHolder : RecyclerView.ViewHolder
		{
			public View View { get; }

			public TemplatedItemViewHolder(AView itemView, View rootElement) : base(itemView)
			{
				View = rootElement;
			}
		}

	}
}