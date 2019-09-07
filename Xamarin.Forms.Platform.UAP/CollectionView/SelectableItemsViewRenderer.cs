using System;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UAP;
using UWPListViewSelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode;

namespace Xamarin.Forms.Platform.UWP
{
	public class StructuredItemsViewRenderer : ItemsViewRenderer
	{
		StructuredItemsView _structuredItemsView;
		View _currentHeader;
		View _currentFooter;

		protected override IItemsLayout Layout { get => _structuredItemsView.ItemsLayout; }

		protected override void SetUpNewElement(ItemsView newElement)
		{
			_structuredItemsView = newElement as StructuredItemsView;

			base.SetUpNewElement(newElement);

			if (newElement == null)
			{
				return;
			}

			UpdateHeader();
			UpdateFooter();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.IsOneOf(StructuredItemsView.HeaderProperty, StructuredItemsView.HeaderTemplateProperty))
			{
				UpdateHeader();
			}
			else if (changedProperty.IsOneOf(StructuredItemsView.FooterProperty, StructuredItemsView.FooterTemplateProperty))
			{
				UpdateFooter();
			}
		}

		protected override ListViewBase SelectListViewBase()
		{
			switch (Layout)
			{
				case GridItemsLayout gridItemsLayout:
					return CreateGridView(gridItemsLayout);
				case LinearItemsLayout listItemsLayout
					when listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal:
					return CreateHorizontalListView();
			}

			// Default to a plain old vertical ListView
			return new Windows.UI.Xaml.Controls.ListView();
		}

		protected virtual void UpdateHeader()
		{
			if (ListViewBase == null)
			{
				return;
			}

			if (_currentHeader != null)
			{
				Element.RemoveLogicalChild(_currentHeader);
				_currentHeader = null;
			}

			var header = _structuredItemsView.Header;

			switch (header)
			{
				case null:
					ListViewBase.Header = null;
					break;

				case string text:
					ListViewBase.HeaderTemplate = null;
					ListViewBase.Header = new TextBlock { Text = text };
					break;

				case View view:
					ListViewBase.HeaderTemplate = ViewTemplate;
					_currentHeader = view;
					Element.AddLogicalChild(_currentHeader);
					ListViewBase.Header = view;
					break;

				default:
					var headerTemplate = _structuredItemsView.HeaderTemplate;
					if (headerTemplate != null)
					{
						ListViewBase.HeaderTemplate = ItemsViewTemplate;
						ListViewBase.Header = new ItemTemplateContext(headerTemplate, header, Element);
					}
					else
					{
						ListViewBase.HeaderTemplate = null;
						ListViewBase.Header = null;
					}
					break;
			}
		}

		protected virtual void UpdateFooter()
		{
			if (ListViewBase == null)
			{
				return;
			}

			if (_currentFooter != null)
			{
				Element.RemoveLogicalChild(_currentFooter);
				_currentFooter = null;
			}

			var footer = _structuredItemsView.Footer;

			switch (footer)
			{
				case null:
					ListViewBase.Footer = null;
					break;

				case string text:
					ListViewBase.FooterTemplate = null;
					ListViewBase.Footer = new TextBlock { Text = text };
					break;

				case View view:
					ListViewBase.FooterTemplate = ViewTemplate;
					_currentFooter = view;
					Element.AddLogicalChild(_currentFooter);
					ListViewBase.Footer = view;
					break;

				default:
					var footerTemplate = _structuredItemsView.FooterTemplate;
					if (footerTemplate != null)
					{
						ListViewBase.FooterTemplate = ItemsViewTemplate;
						ListViewBase.Footer = new ItemTemplateContext(footerTemplate, footer, Element);
					}
					else
					{
						ListViewBase.FooterTemplate = null;
						ListViewBase.Footer = null;
					}
					break;
			}
		}

		protected override void HandleLayoutPropertyChange(PropertyChangedEventArgs property)
		{
			if (property.Is(GridItemsLayout.SpanProperty))
			{
				if (ListViewBase is FormsGridView formsGridView)
				{
					formsGridView.MaximumRowsOrColumns = ((GridItemsLayout)Layout).Span;
				}
			}
		}

		static ListViewBase CreateGridView(GridItemsLayout gridItemsLayout)
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

		static ListViewBase CreateHorizontalListView()
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
	}

	public class SelectableItemsViewRenderer : StructuredItemsViewRenderer
	{
		SelectableItemsView _selectableItemsView;
		bool _ignoreNativeSelectionChange;

		protected override void TearDownOldElement(ItemsView oldElement)
		{
			var oldListViewBase = ListViewBase;
			if (oldListViewBase != null)
			{
				oldListViewBase.ClearValue(ListViewBase.SelectionModeProperty);
				oldListViewBase.SelectionChanged -= OnNativeSelectionChanged;
			}

			if (_selectableItemsView != null)
			{
				_selectableItemsView.SelectionChanged -= OnSelectionChanged;
			}

			base.TearDownOldElement(oldElement);
		}

		protected override void SetUpNewElement(ItemsView newElement)
		{
			base.SetUpNewElement(newElement);

			if (newElement == null)
			{
				return;
			}

			_selectableItemsView = newElement as SelectableItemsView;

			if (_selectableItemsView != null)
			{
				_selectableItemsView.SelectionChanged += OnSelectionChanged;
			}

			var newListViewBase = ListViewBase;

			if (newListViewBase != null)
			{
				newListViewBase.SetBinding(ListViewBase.SelectionModeProperty,
						new Windows.UI.Xaml.Data.Binding
						{
							Source = _selectableItemsView,
							Path = new Windows.UI.Xaml.PropertyPath("SelectionMode"),
							Converter = new SelectionModeConvert(),
							Mode = Windows.UI.Xaml.Data.BindingMode.TwoWay
						});

				newListViewBase.SelectionChanged += OnNativeSelectionChanged;
			}

			UpdateNativeSelection();
		}

		protected override void UpdateItemsSource()
		{
			_ignoreNativeSelectionChange = true;

			base.UpdateItemsSource();

			_ignoreNativeSelectionChange = false;
		}

		void UpdateNativeSelection()
		{
			_ignoreNativeSelectionChange = true;

			switch (ListViewBase.SelectionMode)
			{
				case UWPListViewSelectionMode.None:
					break;
				case UWPListViewSelectionMode.Single:
					if (_selectableItemsView != null)
					{
						if (_selectableItemsView.SelectedItem == null)
						{
							ListViewBase.SelectedItem = null;
						}
						else
						{
							ListViewBase.SelectedItem =
								ListViewBase.Items.First(item =>
								{
									if (item is ItemTemplateContext itemPair)
									{
										return itemPair.Item == _selectableItemsView.SelectedItem;
									}
									else
									{
										return item == _selectableItemsView.SelectedItem;
									}
								});
						}
					}
					
					break;
				case UWPListViewSelectionMode.Multiple:
					ListViewBase.SelectedItems.Clear();
					foreach (var nativeItem in ListViewBase.Items)
					{
						if (nativeItem is ItemTemplateContext itemPair && _selectableItemsView.SelectedItems.Contains(itemPair.Item))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
						else if (_selectableItemsView.SelectedItems.Contains(nativeItem))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
					}
					break;
				case UWPListViewSelectionMode.Extended:
					break;
				default:
					break;
			}

			_ignoreNativeSelectionChange = false;
		}

		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateNativeSelection();
		}

		void OnNativeSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			if (_ignoreNativeSelectionChange)
			{
				return;
			}

			if (Element is SelectableItemsView selectableItemsView)
			{
				switch (ListViewBase.SelectionMode)
				{
					case UWPListViewSelectionMode.None:
						break;
					case UWPListViewSelectionMode.Single:
						var selectedItem = 
							ListViewBase.SelectedItem is ItemTemplateContext itemPair ? itemPair.Item : ListViewBase.SelectedItem;
						selectableItemsView.SelectionChanged -= OnSelectionChanged;
						selectableItemsView.SetValueFromRenderer(SelectableItemsView.SelectedItemProperty, selectedItem);
						selectableItemsView.SelectionChanged += OnSelectionChanged;
						break;
					case UWPListViewSelectionMode.Multiple:
						selectableItemsView.SelectionChanged -= OnSelectionChanged;

						_selectableItemsView.SelectedItems.Clear();
						var selectedItems =
							ListViewBase.SelectedItems
								.Select(a =>
								{
									var item = a is ItemTemplateContext itemPair1 ? itemPair1.Item : a;
									return item;
								})
								.ToList();

						foreach (var item in selectedItems)
						{
							_selectableItemsView.SelectedItems.Add(item);
						}

						selectableItemsView.SelectionChanged += OnSelectionChanged;
						break;

					case UWPListViewSelectionMode.Extended:
						break;

					default:
						break;
				}
			}
		}

		class SelectionModeConvert : Windows.UI.Xaml.Data.IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, string language)
			{
				var formSelectionMode = (SelectionMode)value;
				switch (formSelectionMode)
				{
					case SelectionMode.None:
						return UWPListViewSelectionMode.None;
					case SelectionMode.Single:
						return UWPListViewSelectionMode.Single;
					case SelectionMode.Multiple:
						return UWPListViewSelectionMode.Multiple;
					default:
						return UWPListViewSelectionMode.None;
				}
			}

			public object ConvertBack(object value, Type targetType, object parameter, string language)
			{
				var uwpListViewSelectionMode = (UWPListViewSelectionMode)value;
				switch (uwpListViewSelectionMode)
				{
					case UWPListViewSelectionMode.None:
						return SelectionMode.None;
					case UWPListViewSelectionMode.Single:
						return SelectionMode.Single;
					case UWPListViewSelectionMode.Multiple:
						return SelectionMode.Multiple;
					case UWPListViewSelectionMode.Extended:
						return SelectionMode.None;
					default:
						return SelectionMode.None;
				}
			}
		}

	}
}
