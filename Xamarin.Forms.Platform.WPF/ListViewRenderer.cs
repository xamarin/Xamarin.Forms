using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SLButton = System.Windows.Controls.Button;
using SLBinding = System.Windows.Data.Binding;
using System.Collections;
using System.Windows.Media.Animation;
using Xamarin.Forms.Internals;
using WinList=System.Windows.Controls.ListView;


namespace Xamarin.Forms.Platform.WPF
{
	
	public class LongListSelector : WinList
	{
	    public System.Windows.DataTemplate GroupHeaderTemplate { get; set; }

	    public System.Windows.DataTemplate ListHeaderTemplate { get; set; }

	    public System.Windows.DataTemplate ListFooterTemplate { get; set; }

	    public static DependencyProperty IsGroupingEnabledProperty { get; set; }

	    public Element ListHeader { get; set; }

	    public Element ListFooter { get; set; }
		
		public bool IsInPullToRefresh { get; set; }
	}

	public class ListViewRenderer : ViewRenderer<ListView, LongListSelector>
	{
		public static readonly DependencyProperty HighlightWhenSelectedProperty = DependencyProperty.RegisterAttached("HighlightWhenSelected", typeof(bool), typeof(ListViewRenderer),
			new PropertyMetadata(false));

		readonly List<Tuple<FrameworkElement, SLBinding, Brush>> _previousHighlights = new List<Tuple<FrameworkElement, SLBinding, Brush>>();

		Animatable _animatable;
		object _fromNative;
		bool _itemNeedsSelecting;
		LongListSelector _listBox;
		System.Windows.Controls.ProgressBar _progressBar;

		ScrollViewer _viewport;
		IListViewController Controller => Element;
		ITemplatedItemsView<Cell> TemplatedItemsView => Element;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
			result.Minimum = new Size(40, 40);
			return result;
		}

		public static bool GetHighlightWhenSelected(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(HighlightWhenSelectedProperty);
		}

		public static void SetHighlightWhenSelected(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(HighlightWhenSelectedProperty, value);
		}

		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
		{
			System.Windows.Size result = base.ArrangeOverride(finalSize);

			_progressBar.Measure(finalSize);
			_progressBar.Arrange(new Rect(0, 0, finalSize.Width, _progressBar.DesiredSize.Height));

			return result;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			base.OnElementChanged(e);
			
			if (Element.SelectedItem != null)
				_itemNeedsSelecting = true;

			_listBox = new LongListSelector()
			{
				DataContext = Element,
				ItemsSource = (IList)TemplatedItemsView.TemplatedItems,
				ItemTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["CellTemplate"],
				GroupHeaderTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["ListViewHeader"],
				ListHeaderTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["View"],
				ListFooterTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["View"]
			};
			//_listBox.SetBinding(LongListSelector.IsGroupingEnabledProperty, new SLBinding("IsGroupingEnabled"));

		    _listBox.SelectionChanged += OnNativeSelectionChanged;
		    _listBox.SelectionChanged += OnNativeItemTapped;

		    //_listBox.ItemRealized += OnItemRealized;

			//_listBox.PullToRefreshStarted += OnPullToRefreshStarted;
			//_listBox.PullToRefreshCompleted += OnPullToRefreshCompleted;
			//_listBox.PullToRefreshCanceled += OnPullToRefreshCanceled;
			//_listBox.PullToRefreshStatusUpdated += OnPullToRefreshStatusUpdated;

			SetNativeControl(_listBox);

			_progressBar = new System.Windows.Controls.ProgressBar { Maximum = 1, Visibility = Visibility.Collapsed };
			Children.Add(_progressBar);

			UpdateHeader();
			UpdateFooter();
			UpdateJumpList();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ListView.SelectedItemProperty.PropertyName)
				OnItemSelected(Element.SelectedItem);
			else if (e.PropertyName == "HeaderElement")
				UpdateHeader();
			else if (e.PropertyName == "FooterElement")
				UpdateFooter();
			else if ((e.PropertyName == ListView.IsRefreshingProperty.PropertyName) || (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName) || (e.PropertyName == "CanRefresh"))
				UpdateIsRefreshing();
			else if (e.PropertyName == "GroupShortNameBinding")
				UpdateJumpList();
		}

		protected override void UpdateNativeWidget()
		{
			base.UpdateNativeWidget();

			if (_progressBar != null)
				_progressBar.Width = Element.Width;
		}

		static IEnumerable<T> FindDescendants<T>(DependencyObject dobj) where T : DependencyObject
		{
			int count = VisualTreeHelper.GetChildrenCount(dobj);
			for (var i = 0; i < count; i++)
			{
				DependencyObject element = VisualTreeHelper.GetChild(dobj, i);
				if (element is T)
					yield return (T)element;

				foreach (T descendant in FindDescendants<T>(element))
					yield return descendant;
			}
		}

		FrameworkElement FindElement(Cell cell)
		{
			foreach (CellControl selector in FindDescendants<CellControl>(_listBox))
			{
				if (ReferenceEquals(cell, selector.DataContext))
					return selector;
			}

			return null;
		}

		IEnumerable<FrameworkElement> FindHighlight(FrameworkElement element)
		{
			FrameworkElement parent = element;
			while (true)
			{
				element = parent;
				if (element is CellControl)
					break;

				parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
				if (parent == null)
				{
					parent = element;
					break;
				}
			}

			return FindHighlightCore(parent);
		}

		IEnumerable<FrameworkElement> FindHighlightCore(DependencyObject element)
		{
			int children = VisualTreeHelper.GetChildrenCount(element);
			for (var i = 0; i < children; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(element, i);

				var label = child as LabelRenderer;
				var childElement = child as FrameworkElement;
				if (childElement != null && (GetHighlightWhenSelected(childElement) || label != null))
				{
					if (label != null)
						yield return label.Control;
					else
						yield return childElement;
				}

				foreach (FrameworkElement recursedElement in FindHighlightCore(childElement))
					yield return recursedElement;
			}
		}
		
		void OnItemSelected(object selectedItem)
		{
			RestorePreviousSelectedVisual();

			if (selectedItem == null)
			{
				_listBox.SelectedItem = selectedItem;
				return;
			}

			IEnumerable<CellControl> items = FindDescendants<CellControl>(_listBox);

			CellControl item = items.FirstOrDefault(i =>
			{
				var cell = (Cell)i.DataContext;
				return Equals(cell.BindingContext, selectedItem);
			});

			if (item == null)
			{
				_itemNeedsSelecting = true;
				return;
			}

			SetSelectedVisual(item);
		}

		void OnNativeItemTapped(object sender, SelectionChangedEventArgs e)
		{
			var cell = (Cell)Control.SelectedItem;
			if (cell == null)
				return;

			Cell parentCell = null;

			if (Element.IsGroupingEnabled)
			{
				parentCell = cell.GetGroupHeaderContent<ItemsView<Cell>, Cell>();
			}

			_fromNative = cell.BindingContext;

		    if (Element.IsGroupingEnabled)
		    {
		        Controller.NotifyRowTapped(parentCell.GetIndex<ItemsView<Cell>, Cell>(),
		            cell.GetIndex<ItemsView<Cell>, Cell>(), null);
		    }
		    else
		    {
		        Controller.NotifyRowTapped(cell.GetIndex<ItemsView<Cell>, Cell>(), null);
		    }
			
		    

		}

		void OnNativeSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0)
				return;

			var cell = (Cell)e.AddedItems[0];

			if (cell == null)
			{
				RestorePreviousSelectedVisual();
				return;
			}

			RestorePreviousSelectedVisual();
			FrameworkElement element = FindElement(cell);
			if (element != null)
				SetSelectedVisual(element);
		}

		void RestorePreviousSelectedVisual()
		{
			foreach (Tuple<FrameworkElement, SLBinding, Brush> highlight in _previousHighlights)
			{
				if (highlight.Item2 != null)
					highlight.Item1.SetForeground(highlight.Item2);
				else
					highlight.Item1.SetForeground(highlight.Item3);
			}

			_previousHighlights.Clear();
		}

		void SetSelectedVisual(FrameworkElement element)
		{
			IEnumerable<FrameworkElement> highlightMes = FindHighlight(element);
			foreach (FrameworkElement toHighlight in highlightMes)
			{
				Brush brush = null;
				SLBinding binding = toHighlight.GetForegroundBinding();
				if (binding == null)
					brush = toHighlight.GetForeground();

				_previousHighlights.Add(new Tuple<FrameworkElement, SLBinding, Brush>(toHighlight, binding, brush));
				toHighlight.SetForeground((Brush)System.Windows.Application.Current.Resources["AccentBrush"]);
			}
		}

		void UpdateFooter()
		{
			Control.ListFooter = Controller.FooterElement;
		}

		void UpdateHeader()
		{
			Control.ListHeader = Controller.HeaderElement;
		}

		void UpdateIsRefreshing()
		{
			if (Element.IsRefreshing)
			{
				_progressBar.Visibility = Visibility.Visible;
				_progressBar.IsIndeterminate = true;
			}
			else
			{
				_progressBar.IsIndeterminate = false;
				_progressBar.Visibility = _listBox.IsInPullToRefresh && Element.IsPullToRefreshEnabled && Controller.RefreshAllowed ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		void UpdateJumpList()
		{
			//TODO: UpdateJumpList
		}
	}
}