using System.Collections;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Xamarin.Forms.Platform.UWP
{
	public class GroupableItemsViewRenderer<TItemsView> : SelectableItemsViewRenderer<TItemsView>
		where TItemsView : GroupableItemsView
	{
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.IsOneOf(GroupableItemsView.IsGroupedProperty,
				GroupableItemsView.GroupFooterTemplateProperty, GroupableItemsView.GroupHeaderTemplateProperty))
			{
				UpdateItemsSource();
			}
		}

		protected override CollectionViewSource CreateCollectionViewSource()
		{
			if (ItemsView != null && ItemsView.IsGrouped)
			{
				var itemTemplate = Element.ItemTemplate;
				var itemsSource = Element.ItemsSource;

				return new CollectionViewSource
				{
					Source = TemplatedItemSourceFactory.CreateGrouped(itemsSource, itemTemplate,
					ItemsView.GroupHeaderTemplate, ItemsView.GroupFooterTemplate, Element),
					IsSourceGrouped = true,
					ItemsPath = new Windows.UI.Xaml.PropertyPath(nameof(GroupTemplateContext.Items))
				};
			}
			else
			{
				return base.CreateCollectionViewSource();
			}
		}

		protected override void UpdateItemTemplate()
		{
			base.UpdateItemTemplate();

			ListViewBase.GroupStyleSelector = new GroupHeaderStyleSelector();
		}

		protected override object FindBoundItem(ScrollToRequestEventArgs args)
		{
			if (!ItemsView.IsGrouped || args.Mode == ScrollToMode.Element)
			{
				return base.FindBoundItem(args);
			}

			var groups = CollectionViewSource.Source as GroupedItemTemplateCollection;

			if (groups == null || args.GroupIndex >= groups.Count)
			{
				return null;
			}

			if (!(groups[args.GroupIndex].Items is IEnumerable group))
			{
				return null;
			}

			var index = args.Index;

			foreach (var item in group)
			{
				if (index == 0)
				{
					return item;
				}

				index -= 1;
			}

			return null;
		}
	}
}
