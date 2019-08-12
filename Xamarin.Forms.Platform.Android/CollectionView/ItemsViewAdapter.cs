using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Widget;
using Object = Java.Lang.Object;
using ViewGroup = Android.Views.ViewGroup;

namespace Xamarin.Forms.Platform.Android
{
	public class ItemsViewAdapter : RecyclerView.Adapter
	{
		protected readonly ItemsView ItemsView;
		readonly Func<View, Context, ItemContentView> _createItemContentView;
		internal readonly IItemsViewSource ItemsSource;

		bool _disposed;
		Size? _size;

		bool _usingItemTemplate = false;

		internal ItemsViewAdapter(ItemsView itemsView, Func<View, Context, ItemContentView> createItemContentView = null)
		{
			Xamarin.Forms.CollectionView.VerifyCollectionViewFlagEnabled(nameof(ItemsViewAdapter));

			ItemsView = itemsView ?? throw new ArgumentNullException(nameof(itemsView));

			UpdateUsingItemTemplate();

			ItemsView.PropertyChanged += ItemsViewPropertyChanged;

			_createItemContentView = createItemContentView;
			ItemsSource = ItemsSourceFactory.Create(itemsView.ItemsSource, this);

			UpdateHasHeader();
			UpdateHasFooter();

			if (_createItemContentView == null)
			{
				_createItemContentView = (view, context) => new ItemContentView(context);
			}
		}

		protected virtual void ItemsViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs property)
		{
			if (property.Is(ItemsView.HeaderProperty))
			{
				UpdateHasHeader();
			}
			else if (property.Is(ItemsView.ItemTemplateProperty))
			{
				UpdateUsingItemTemplate();
			}
			else if (property.Is(ItemsView.FooterProperty))
			{
				UpdateHasFooter();
			}
		}

		public override void OnViewRecycled(Object holder)
		{
			if (holder is TemplatedItemViewHolder templatedItemViewHolder)
			{
				templatedItemViewHolder.Recycle(ItemsView);
			}

			base.OnViewRecycled(holder);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			if (IsHeader(position))
			{
				if (holder is TemplatedItemViewHolder templatedItemViewHolder)
				{
					BindTemplatedItemViewHolder(templatedItemViewHolder, ItemsView.Header);
				}

				return;
			}

			if (IsFooter(position))
			{
				if (holder is TemplatedItemViewHolder templatedItemViewHolder)
				{
					BindTemplatedItemViewHolder(templatedItemViewHolder, ItemsView.Footer);
				}

				return;
			}

			var itemsSourcePosition = position;

			switch (holder)
			{
				case TextViewHolder textViewHolder:
					textViewHolder.TextView.Text = ItemsSource.GetItem(itemsSourcePosition).ToString();
					break;
				case TemplatedItemViewHolder templatedItemViewHolder:
					BindTemplatedItemViewHolder(templatedItemViewHolder, ItemsSource.GetItem(itemsSourcePosition));
					break;
			}
		}

		void BindTemplatedItemViewHolder(TemplatedItemViewHolder templatedItemViewHolder, object context)
		{
			if (ItemsView.ItemSizingStrategy == ItemSizingStrategy.MeasureFirstItem)
			{
				templatedItemViewHolder.Bind(context, ItemsView, SetStaticSize, _size);
			}
			else
			{
				templatedItemViewHolder.Bind(context, ItemsView);
			}
		}

		void SetStaticSize(Size size)
		{
			_size = size;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var context = parent.Context;

			if (viewType == ItemViewType.Header)
			{
				return CreateHeaderFooterViewHolder(ItemsView.Header, ItemsView.HeaderTemplate, context);
			}

			if (viewType == ItemViewType.Footer)
			{
				return CreateHeaderFooterViewHolder(ItemsView.Footer, ItemsView.FooterTemplate, context);
			}

			if (viewType == ItemViewType.TextItem)
			{
				var view = new TextView(context);
				return new TextViewHolder(view);
			}

			var itemContentView = new ItemContentView(context);
			return new TemplatedItemViewHolder(itemContentView, ItemsView.ItemTemplate);
		}

		public override int ItemCount => ItemsSource.Count;

		public override int GetItemViewType(int position)
		{
			if (IsHeader(position))
			{
				return ItemViewType.Header;
			}

			if (IsFooter(position))
			{
				return ItemViewType.Footer;
			}

			if (_usingItemTemplate)
			{
				return ItemViewType.TemplatedItem;
			}
		
			// No template, just use the Text view
			return ItemViewType.TextItem;
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					ItemsSource?.Dispose();
					ItemsView.PropertyChanged -= ItemsViewPropertyChanged;
				}

				_disposed = true;

				base.Dispose(disposing);
			}
		}

		public virtual int GetPositionForItem(object item)
		{
			return ItemsSource.GetPosition(item);
		}

		void UpdateUsingItemTemplate()
		{
			_usingItemTemplate = ItemsView.ItemTemplate != null;
		}

		void UpdateHasHeader()
		{
			ItemsSource.HasHeader = ItemsView.Header != null;
		}

		void UpdateHasFooter()
		{
			ItemsSource.HasFooter = ItemsView.Footer != null;
		}

		bool IsHeader(int position)
		{
			return ItemsSource.IsHeader(position);
		}

		bool IsFooter(int position)
		{
			return ItemsSource.IsFooter(position);
		}

		RecyclerView.ViewHolder CreateHeaderFooterViewHolder(object content, DataTemplate template, Context context)
		{
			if (template != null)
			{
				var footerContentView = new ItemContentView(context);
				return new TemplatedItemViewHolder(footerContentView, template);
			}

			if (content is View formsView)
			{
				return SimpleViewHolder.FromFormsView(formsView, context);
			}

			// No template, Footer is not a Forms View, so just display Footer.ToString
			return SimpleViewHolder.FromText(content?.ToString(), context, fill: false);
		}
	}
}