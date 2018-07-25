using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Xamarin.Forms.Internals;
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

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.Is(ItemsView.ItemsSourceProperty))
			{
				UpdateItemsSource();
			}
			else if (changedProperty.Is(ItemsView.ItemTemplateProperty))
			{
				UpdateItemTemplate();
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

			CollectionViewSource collectionViewSource;

			var itemsSource = Element.ItemsSource;

			var itemTemplate = Element.ItemTemplate;
			if (itemTemplate != null)
			{
				// The ItemContentControls need the actual data item and the template so they can inflate the template
				// and bind the result to the data item.
				// ItemTemplateEnumerator handles pairing them up for the ItemContentControls to consume
				var itemTemplateEnumerator = new ItemTemplateEnumerator(itemsSource, itemTemplate);
				collectionViewSource = new CollectionViewSource
				{
					Source = itemTemplateEnumerator,
					IsSourceGrouped = false
				};
			}
			else
			{
				collectionViewSource = new CollectionViewSource
				{
					Source = itemsSource,
					IsSourceGrouped = false
				};	
			}

			ItemsControl.ItemsSource = collectionViewSource.View;
		}

		protected virtual void UpdateItemTemplate()
		{
			if (Element == null || ItemsControl == null)
			{
				return;
			}

			var formsTemplate = Element.ItemTemplate;
			var itemsControlItemTemplate = ItemsControl.ItemTemplate;

			if (formsTemplate == null)
			{
				ItemsControl.ItemTemplate = null;

				if (itemsControlItemTemplate != null)
				{
					// We've removed the template; the itemssource should be updated
					// TODO hartez 2018/06/25 21:25:24 I don't love that changing the template might reset the whole itemssource. We should think about a way to make that unnecessary	
					UpdateItemsSource();
				}

				return;
			}

			// TODO hartez 2018/06/23 13:47:27 Handle DataTemplateSelector case
			// Actually, DataTemplateExtensions CreateContent might handle the selector for us

			// TODO hartez 2018/06/23 13:57:05 This loads a ContentControl; what we need is a custom subclass of ContentControl	
			// which has a binding to the item and to formsTemplate and will take those two values
			// and call formsTemplate.CreateContent with the item (this may require modifying DataTemplateSelector.cs)
			// and then doing something like GetOrCreateRenderer
			// and then set the content of the ContentControl to the result

			ItemsControl.ItemTemplate =
				(Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Application.Current.Resources["ItemsViewDefaultTemplate"];

			if (itemsControlItemTemplate == null)
			{
				// We're using a data template now, so we'll need to update the itemsource
				UpdateItemsSource();
			}
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
			var horizontalListView = new Windows.UI.Xaml.Controls.ListView()
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

			UpdateItemTemplate();
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