using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Xamarin.Forms.Platform.UWP
{
	public class CollectionViewRenderer : ViewRenderer<CollectionView, ItemsControl>
	{
		protected ItemsControl ItemsControl { get; private set; }

		protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> args)
		{
			base.OnElementChanged(args);

			// TODO hartez 2018-05-22 12:58 PM Unhook old element stuff
			//if (args.OldElement != null)
			//{
			//}

			if (args.NewElement != null)
			{
				if (ItemsControl == null)
				{
					ItemsControl = SelectLayout(args.NewElement.ItemsLayout);
					SetNativeControl(ItemsControl);
				}

				// TODO hartez 2018-05-22 12:59 PM Handle grouping
				var cvs = new CollectionViewSource
				{
					Source = Element.ItemsSource, IsSourceGrouped = false
				};

				ItemsControl.ItemsSource = cvs.View;
			}
		}

		protected virtual ItemsControl SelectLayout(IItemsLayout layoutSpecification)
		{
			// TODO hartez 2018/05/29 15:04:50 Handle GridItemsLayout	

			if (layoutSpecification is ListItemsLayout listItemsLayout 
			    && listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal)
			{
				var horizontalListView = new Windows.UI.Xaml.Controls.ListView
				{
					// TODO hartez 2018/05/29 15:38:04 Make sure the ItemsViewStyles.xaml xbf gets into the nuspec	
					ItemsPanel = (ItemsPanelTemplate)Windows.UI.Xaml.Application.Current.Resources["HorizontalListItemsPanel"]
				};

				ScrollViewer.SetHorizontalScrollMode(horizontalListView, ScrollMode.Auto);
				ScrollViewer.SetHorizontalScrollBarVisibility(horizontalListView, 
					Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto);

				return horizontalListView;
			}

			// Default to a plain old vertical ListView
			return new Windows.UI.Xaml.Controls.ListView();
		}
	}
}