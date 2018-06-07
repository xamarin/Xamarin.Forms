using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Xamarin.Forms.Platform.UAP;

namespace Xamarin.Forms.Platform.UWP
{
	public class CollectionViewRenderer : ViewRenderer<CollectionView, ItemsControl>
	{
		IItemsLayout _layout;

		protected ItemsControl ItemsControl { get; private set; }

		protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> args)
		{
			base.OnElementChanged(args);
			TearDownOldElement(args.OldElement);
			SetUpNewElement(args.NewElement);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ItemsView.ItemsSourceProperty.PropertyName)
			{
				UpdateItemsSource();
			}
		}

		protected virtual ItemsControl SelectLayout(IItemsLayout layoutSpecification)
		{
			switch (layoutSpecification)
			{
				case GridItemsLayout gridItemsLayout:
					return CreateGridView(gridItemsLayout);
				case ListItemsLayout listItemsLayout
					when listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal:
					return CreateHorizontalListView();
			}

			// Default to a plain old vertical ListView
			return new Windows.UI.Xaml.Controls.ListView();
		}

		protected virtual void UpdateItemsSource()
		{
			if (ItemsControl == null)
			{
				return;
			}

			// TODO hartez 2018-05-22 12:59 PM Handle grouping
			var cvs = new CollectionViewSource
			{
				Source = Element.ItemsSource,
				IsSourceGrouped = false
			};

			ItemsControl.ItemsSource = cvs.View;
		}

		static ItemsControl CreateGridView(GridItemsLayout gridItemsLayout)
		{
			var gridView = new FormsGridView();

			if (gridItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal)
			{
				gridView.UseHorizontalItemsPanel();

				// TODO hartez 2018/06/06 12:13:38 Should this logic just be built into FormsGridView?	
				ScrollViewer.SetHorizontalScrollMode(gridView, ScrollMode.Auto);
				ScrollViewer.SetHorizontalScrollBarVisibility(gridView,
					Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto);
			}
			else
			{
				gridView.UseVerticalalItemsPanel();
			}

			gridView.MaximumRowsOrColumns = gridItemsLayout.Span;

			return gridView;
		}

		static ItemsControl CreateHorizontalListView()
		{
			// TODO hartez 2018/06/05 16:18:57 Is there any performance benefit to caching the ItemsPanelTemplate lookup?	
			// TODO hartez 2018/05/29 15:38:04 Make sure the ItemsViewStyles.xaml xbf gets into the nuspec	
			var horizontalListView = new Windows.UI.Xaml.Controls.ListView
			{
				ItemsPanel =
					(ItemsPanelTemplate)Windows.UI.Xaml.Application.Current.Resources["HorizontalListItemsPanel"]
			};

			ScrollViewer.SetHorizontalScrollMode(horizontalListView, ScrollMode.Auto);
			ScrollViewer.SetHorizontalScrollBarVisibility(horizontalListView,
				Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto);

			return horizontalListView;
		}

		void LayoutOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == GridItemsLayout.SpanProperty.PropertyName)
			{
				if (ItemsControl is FormsGridView formsGridView)
				{
					formsGridView.MaximumRowsOrColumns = ((GridItemsLayout)_layout).Span;
				}
			}
		}

		void SetUpNewElement(ItemsView newElement)
		{
			if (newElement == null)
			{
				return;
			}

			if (ItemsControl == null)
			{
				ItemsControl = SelectLayout(newElement.ItemsLayout);

				_layout = newElement.ItemsLayout;
				_layout.PropertyChanged += LayoutOnPropertyChanged;

				SetNativeControl(ItemsControl);
			}

			UpdateItemsSource();
		}

		void TearDownOldElement(ItemsView oldElement)
		{
			if (oldElement == null)
			{
				return;
			}

			if (_layout != null)
			{
				// Stop tracking the old layout
				_layout.PropertyChanged -= LayoutOnPropertyChanged;
				_layout = null;
			}
		}
	}
}