using System.ComponentModel;
using Windows.UI.Xaml;
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

				UpdateItemsSource();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ItemsView.ItemsSourceProperty.PropertyName) UpdateItemsSource();
		}

		protected virtual ItemsControl SelectLayout(IItemsLayout layoutSpecification)
		{
			// TODO hartez 2018/05/29 15:04:50 Handle GridItemsLayout	
			if (layoutSpecification is GridItemsLayout gridItemsLayout)
			{
				return CreateGridView(gridItemsLayout);
			}

			if (layoutSpecification is ListItemsLayout listItemsLayout
			    && listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal)
			{
				return CreateHorizontalListView();
			}

			// Default to a plain old vertical ListView
			return new Windows.UI.Xaml.Controls.ListView();
		}

		ItemsControl CreateGridView(GridItemsLayout gridItemsLayout)
		{
			var gridView = new GridView();

			if (gridItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal)
			{
				gridView.ItemsPanel =
					(ItemsPanelTemplate)Windows.UI.Xaml.Application.Current.Resources["HorizontalGridItemsPanel"];
			}
			else
			{
				gridView.ItemsPanel =
					(ItemsPanelTemplate)Windows.UI.Xaml.Application.Current.Resources["VerticalGridItemsPanel"];
			}

			gridView.Loaded += GridViewOnLoaded;
			
			return gridView;
		}

		void GridViewOnLoaded(object sender, RoutedEventArgs e)
		{
			// Once the GridView is loaded, we can dive into its tree and find the ItemsWrapGrid so we can 
			// set the rows/columns for it. 

			// TODO hartez 2018/06/05 17:24:17 Subclass GridView as FormsGridView and add MaxRowsColumns as a property	
			// We don't want to have to do this traversal every time the GridLayout's Span changes

			var gridView = (GridView)sender;

			gridView.Loaded -= GridViewOnLoaded;

			if (Element == null)
			{
				return;
			}

			var gridItemsLayout = (GridItemsLayout)Element.ItemsLayout;

			var wrapGrid = gridView.GetFirstDescendant<ItemsWrapGrid>();
			wrapGrid.MaximumRowsOrColumns = gridItemsLayout.Span;
		}

		static ItemsControl CreateHorizontalListView()
		{
			// TODO hartez 2018/06/05 16:18:57 Is there any performance benefit to caching the ItemsPanelTemplate lookup?	
			// TODO hartez 2018/05/29 15:38:04 Make sure the ItemsViewStyles.xaml xbf gets into the nuspec	
			var horizontalListView = new Windows.UI.Xaml.Controls.ListView
			{
				ItemsPanel = (ItemsPanelTemplate)Windows.UI.Xaml.Application.Current.Resources["HorizontalListItemsPanel"]
			};

			ScrollViewer.SetHorizontalScrollMode(horizontalListView, ScrollMode.Auto);
			ScrollViewer.SetHorizontalScrollBarVisibility(horizontalListView,
				Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto);

			return horizontalListView;
		}

		protected virtual void UpdateItemsSource()
		{
			if (ItemsControl == null) return;

			// TODO hartez 2018-05-22 12:59 PM Handle grouping
			var cvs = new CollectionViewSource
			{
				Source = Element.ItemsSource,
				IsSourceGrouped = false
			};

			ItemsControl.ItemsSource = cvs.View;
		}
	}
}