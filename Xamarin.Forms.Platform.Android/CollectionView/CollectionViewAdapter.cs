using System;
using System.Collections;
using System.Diagnostics;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Internals;
using AView = Android.Views.View;

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
			if (holder is TextViewHolder textViewHolder && _itemsView.ItemsSource is IList list)
			{
				textViewHolder.TextView.Text = list[position].ToString();
			}
			else if (holder is TemplatedItemViewHolder templateViewHolder && _itemsView.ItemsSource is IList list2)
			{
				BindableObject.SetInheritedBindingContext(templateViewHolder.View, list2[position]);
				templateViewHolder.View.Platform = new Platform(_context);
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

		// TODO hartez 2018/05/29 17:08:30 Very naive implementation, obviously we'll need something better	
		public override int ItemCount => (_itemsView.ItemsSource as IList).Count;

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