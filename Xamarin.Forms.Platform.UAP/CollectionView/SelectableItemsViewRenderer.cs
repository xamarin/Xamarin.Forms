using System;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using UWPListViewSelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode;

namespace Xamarin.Forms.Platform.UWP
{
	public class SelectableItemsViewRenderer : ItemsViewRenderer
	{
		SelectableItemsView _selectableItemsView;

		protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> args)
		{
			var oldListViewBase = ListViewBase;
			if (oldListViewBase != null)
			{
				oldListViewBase.ClearValue(ListViewBase.SelectionModeProperty);
				oldListViewBase.ClearValue(ListViewBase.SelectedItemProperty);
				oldListViewBase.SelectionChanged -= OnNativeSelectionChanged;
			}

			if (args.OldElement != null)
			{
				((SelectableItemsView)args.OldElement).SelectionChanged -= OnSelectionChanged;
			}

			base.OnElementChanged(args);
			_selectableItemsView = args.NewElement;
			_selectableItemsView.SelectionChanged += OnSelectionChanged;

			var newListViewBase = ListViewBase;

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

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

		}

		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (ListViewBase.SelectionMode)
			{
				case UWPListViewSelectionMode.None:
					break;
				case UWPListViewSelectionMode.Single:
					ListViewBase.SelectionChanged -= OnNativeSelectionChanged;
					ListViewBase.SelectedItem =
						ListViewBase.Items.First(item =>
							((ItemTemplatePair)item).Item ==  _selectableItemsView.SelectedItem
						);
					ListViewBase.SelectionChanged += OnNativeSelectionChanged;
					break;
				case UWPListViewSelectionMode.Multiple:
					break;
				case UWPListViewSelectionMode.Extended:
					ListViewBase.SelectionChanged -= OnNativeSelectionChanged;
					var nativeSelectedItems = ListViewBase.SelectedItems.ToList();
					ListViewBase.SelectedItems.Clear();
					foreach (var nativeItem in nativeSelectedItems)
					{
						var itemPair = (ItemTemplatePair)nativeItem;
						if (_selectableItemsView.SelectedItems.Contains(itemPair.Item))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
					}
					ListViewBase.SelectionChanged += OnNativeSelectionChanged;
					break;
				default:
					break;
			}
		}

		void OnNativeSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			switch (ListViewBase.SelectionMode)
			{
				case UWPListViewSelectionMode.None:
					break;
				case UWPListViewSelectionMode.Single:
					var selectedItem = (ItemTemplatePair)ListViewBase.SelectedItem;
					Element.SetValueFromRenderer(SelectableItemsView.SelectedItemProperty, selectedItem.Item);
					break;
				case UWPListViewSelectionMode.Multiple:
					break;
				case UWPListViewSelectionMode.Extended:
					var selectedItems = 
						ListViewBase.SelectedItems
							.Cast<ItemTemplatePair>()
							.Select(a => a.Item)
							.ToList();
					Element.SetValueFromRenderer(SelectableItemsView.SelectedItemsProperty, selectedItems);
					break;
				default:
					break;
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
						return UWPListViewSelectionMode.Extended;
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
						/// xamarin forms currently only have three states selectionMode;
						return SelectionMode.None;
					case UWPListViewSelectionMode.Extended:
						return SelectionMode.Multiple;
					default:
						return SelectionMode.None;
				}
			}
		}

	}
}
