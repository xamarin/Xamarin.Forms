using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UAP;

namespace Xamarin.Forms.Platform.UWP
{
	public class FormsListItem : ListViewItem
	{
		DataTemplate FormsTemplate { get; set; }

		BindableObject _bindableObject;

		public FormsListItem(DataTemplate formsTemplate)
		{
			FormsTemplate = formsTemplate;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var content = FormsTemplate.CreateContent();

			_bindableObject = content as BindableObject;
		}



		protected override void OnContentChanged(object oldContent, object newContent)
		{
			//base.OnContentChanged(oldContent, newContent);

			if (_bindableObject != null)
			{
				BindableObject.SetInheritedBindingContext(_bindableObject, newContent);
			}

			if (_bindableObject is VisualElement visualElement)
			{
				var formsContent = visualElement.GetOrCreateRenderer().ContainerElement;
				((FormsContentControl)ContentTemplateRoot).Content = formsContent;
			}
		}

		//protected override void OnContentChanged(object oldContent, object newContent)
		//{
		//	base.OnContentChanged(oldContent, newContent);
		//	if (newContent is VisualElement visualElement)
		//	{
		//		var formsContent = visualElement.GetOrCreateRenderer().ContainerElement;
		//		Content = formsContent;
		//	}
		//}
	}

	public class FormsListView : Windows.UI.Xaml.Controls.ListView
	{
		// TODO hartez 2018/06/24 16:22:12 If this works, we need to apply it to FormsGridView	
		// TODO hartez 2018/06/24 16:54:56 If these GetContainer overrides work, I wonder if we need FormsContentControl at all	
		public DataTemplate FormsTemplate { get; set; }

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is FormsListItem;
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new FormsListItem(FormsTemplate);
		}
	}

	public class FormsContentControl : ContentControl
	{
		public FormsContentControl()
		{
			DefaultStyleKey = typeof(FormsContentControl);
			Loaded += OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			Loaded -= OnLoaded;

			//var content = FormsTemplate.CreateContent();

			//if (content is BindableObject bindableObject)
			//{
			//	BindableObject.SetInheritedBindingContext(bindableObject, DataContext);
			//}

			//if (content is VisualElement visualElement)
			//{
			//	_contentPresenter.Content = visualElement.GetOrCreateRenderer().ContainerElement;
			//	_contentPresenter.Width = 200;
			//	_contentPresenter.Height = 200;
			//}
		}

		Windows.UI.Xaml.Controls.ContentPresenter _contentPresenter;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_contentPresenter = (Windows.UI.Xaml.Controls.ContentPresenter)GetTemplateChild("ContentPresenter");
		}

		public DataTemplate FormsTemplate { get; set; }
	}


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
			return new FormsListView();
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

		//class FormsDataTemplateSelector : Windows.UI.Xaml.Controls.DataTemplateSelector
		//{
		//	public Xamarin.Forms.DataTemplate FormsTemplate { get; set; }
			
		//	protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item)
		//	{
		//		var template = (Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Application.Current.Resources["ItemsViewDefaultTemplate"];

		//		return template;
		//	}

		//	protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, DependencyObject container)
		//	{
		//		return SelectTemplateCore(item);
		//	}
		//}

		protected virtual void UpdateItemTemplate()
		{
			if (Element == null || ItemsControl == null)
			{
				return;
			}

			var formsTemplate = Element.ItemTemplate;

			if (formsTemplate == null)
			{
				ItemsControl.ItemTemplate = null;
			}

			// TODO hartez 2018/06/24 17:05:30 Should this be an else?	

			//var itemTemplateSelector = new FormsDataTemplateSelector { FormsTemplate = formsTemplate };
			//ItemsControl.ItemTemplateSelector = itemTemplateSelector;

			// TODO hartez 2018/06/23 13:47:27 Handle DataTemplateSelector case
			// Actually, DataTemplateExtensions CreateContent might handle the selector for us

			// TODO hartez 2018/06/23 13:57:05 This loads a ContentControl; what we need is a custom subclass of ContentControl	
			// which has a binding to the item and to formsTemplate and will take those two values
			// and call formsTemplate.CreateContent with the item (this may require modifying DataTemplateSelector.cs)
			// and then doing something like GetOrCreateRenderer
			// and then set the content of the ContentControl to the result

			

			ItemsControl.ItemTemplate =
				(Windows.UI.Xaml.DataTemplate)Windows.UI.Xaml.Application.Current.Resources["ItemsViewDefaultTemplate"];

			if (ItemsControl is FormsListView formsListView)
			{
				formsListView.FormsTemplate = formsTemplate;
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
			var horizontalListView = new FormsListView
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
			UpdateItemTemplate();
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