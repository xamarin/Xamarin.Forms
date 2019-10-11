using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WList = System.Windows.Controls.ListView;
using WpfScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility;

namespace Xamarin.Forms.Platform.WPF
{
	public class ListViewRenderer : ViewRenderer<ListView, WList>
	{
		ITemplatedItemsView<Cell> TemplatedItemsView => Element;
		WpfScrollBarVisibility? _defaultHorizontalScrollVisibility;
		WpfScrollBarVisibility? _defaultVerticalScrollVisibility;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
			result.Minimum = new Size(40, 40);
			return result;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
		{
			if (e.OldElement != null) // Clear old element event
			{
				e.OldElement.ItemSelected -= OnElementItemSelected;
				e.OldElement.ScrollToRequested -= OnElementScrollToRequested;

				var templatedItems = ((ITemplatedItemsView<Cell>)e.OldElement).TemplatedItems;
				templatedItems.CollectionChanged -= OnCollectionChanged;
				templatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
			}

			if (e.NewElement != null)
			{
				e.NewElement.ItemSelected += OnElementItemSelected;
				e.NewElement.ScrollToRequested += OnElementScrollToRequested;

				if (Control == null) // Construct and SetNativeControl and suscribe control event
				{
					var listView = new WList
					{
						DataContext = Element,
						ItemTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["CellTemplate"],
						Style = (System.Windows.Style)System.Windows.Application.Current.Resources["ListViewTemplate"]
					};

					SetNativeControl(listView);

					Control.MouseUp += OnNativeMouseUp;
					Control.KeyUp += OnNativeKeyUp;
					Control.TouchUp += OnNativeTouchUp;
					Control.StylusUp += OnNativeStylusUp;
				}

				// Suscribe element events
				var templatedItems = TemplatedItemsView.TemplatedItems;
				templatedItems.CollectionChanged += OnCollectionChanged;
				templatedItems.GroupedCollectionChanged += OnGroupedCollectionChanged;

				// Update control properties
				UpdateItemSource();
				UpdateHorizontalScrollBarVisibility();
				UpdateVerticalScrollBarVisibility();

				if (Element.SelectedItem != null)
					OnElementItemSelected(null, new SelectedItemChangedEventArgs(Element.SelectedItem, -1));
			}

			base.OnElementChanged(e);
		}
		void OnElementScrollToRequested(object sender, ScrollToRequestedEventArgs e)
		{
			var scrollArgs = (ITemplatedItemsListScrollToRequestedEventArgs)e;
			ScrollTo(scrollArgs.Group, scrollArgs.Item, e.Position, e.ShouldAnimate);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == ScrollView.VerticalScrollBarVisibilityProperty.PropertyName)
			{
				UpdateVerticalScrollBarVisibility();
			}
			else if (e.PropertyName == ScrollView.HorizontalScrollBarVisibilityProperty.PropertyName)
			{
				UpdateHorizontalScrollBarVisibility();
			}
		}

		void UpdateItemSource()
		{
			List<object> items = new List<object>();

			if (Element.IsGroupingEnabled)
			{
				int index = 0;
				foreach (var groupItem in TemplatedItemsView.TemplatedItems)
				{
					var group = TemplatedItemsView.TemplatedItems.GetGroup(index);

					if (group.Count != 0)
					{
						items.Add(group.HeaderContent);

						items.AddRange(group);
					}

					index++;
				}

				Control.ItemsSource = items;
			}
			else
			{
				foreach (var item in TemplatedItemsView.TemplatedItems)
				{
					items.Add(item);
				}

				Control.ItemsSource = items;
			}
		}

		void UpdateVerticalScrollBarVisibility()
		{
			if (_defaultVerticalScrollVisibility == null)
				_defaultVerticalScrollVisibility = ScrollViewer.GetVerticalScrollBarVisibility(Control);

			switch (Element.VerticalScrollBarVisibility)
			{
				case (ScrollBarVisibility.Always):
					ScrollViewer.SetVerticalScrollBarVisibility(Control, WpfScrollBarVisibility.Visible);
					break;
				case (ScrollBarVisibility.Never):
					ScrollViewer.SetVerticalScrollBarVisibility(Control, WpfScrollBarVisibility.Hidden);
					break;
				case (ScrollBarVisibility.Default):
					ScrollViewer.SetVerticalScrollBarVisibility(Control, (WpfScrollBarVisibility)_defaultVerticalScrollVisibility);
					break;
			}
		}

		void UpdateHorizontalScrollBarVisibility()
		{
			if (_defaultHorizontalScrollVisibility == null)
				_defaultHorizontalScrollVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(Control);

			switch (Element.HorizontalScrollBarVisibility)
			{
				case (ScrollBarVisibility.Always):
					ScrollViewer.SetHorizontalScrollBarVisibility(Control, WpfScrollBarVisibility.Visible);
					break;
				case (ScrollBarVisibility.Never):
					ScrollViewer.SetHorizontalScrollBarVisibility(Control, WpfScrollBarVisibility.Hidden);
					break;
				case (ScrollBarVisibility.Default):
					ScrollViewer.SetHorizontalScrollBarVisibility(Control, (WpfScrollBarVisibility)_defaultHorizontalScrollVisibility);
					break;
			}
		}

		void OnNativeKeyUp(object sender, KeyEventArgs e)
			=> Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

		void OnNativeMouseUp(object sender, MouseButtonEventArgs e)
			=> Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

		void OnNativeTouchUp(object sender, TouchEventArgs e)
			=> Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

		void OnNativeStylusUp(object sender, StylusEventArgs e)
			=> Element.NotifyRowTapped(Control.SelectedIndex, cell: null);

		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.MouseUp -= OnNativeMouseUp;
					Control.KeyUp -= OnNativeKeyUp;
					Control.TouchUp -= OnNativeTouchUp;
					Control.StylusUp -= OnNativeStylusUp;
				}

				if (Element != null)
				{
					TemplatedItemsView.TemplatedItems.CollectionChanged -= OnCollectionChanged;
					TemplatedItemsView.TemplatedItems.GroupedCollectionChanged -= OnGroupedCollectionChanged;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		void OnElementItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (Element == null)
				return;

			if (e.SelectedItem == null)
			{
				Control.SelectedIndex = -1;
				return;
			}

			var templatedItems = TemplatedItemsView.TemplatedItems;
			var index = 0;

			if (Element.IsGroupingEnabled)
			{
				int selectedItemIndex = templatedItems.GetGlobalIndexOfItem(e.SelectedItem);
				var leftOver = 0;
				int groupIndex = templatedItems.GetGroupIndexFromGlobal(selectedItemIndex, out leftOver);

				index = selectedItemIndex - (groupIndex + 1);
			}
			else
			{
				index = templatedItems.GetGlobalIndexOfItem(e.SelectedItem);
			}

			Control.SelectedIndex = index;
		}

		void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateItemSource();
		}

		void OnGroupedCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateItemSource();
		}
		static ScrollViewer GetScrollViewer(DependencyObject o)
		{
			if (o is ScrollViewer viewer)
			{
				//viewer.CanContentScroll = false;
				return viewer;
			}

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
			{
				var child = VisualTreeHelper.GetChild(o, i);

				var result = GetScrollViewer(child);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}
		void ScrollTo(object group, object item, ScrollToPosition toPosition, bool shouldAnimate, bool includeGroup = false, bool previouslyFailed = false)
		{
			var viewer = GetScrollViewer(Control);
			if (viewer == null)
			{
				RoutedEventHandler loadedHandler = null;
				loadedHandler = (o, e) =>
				{
					Control.Loaded -= loadedHandler;

					// Here we try to avoid an exception, see explanation at bottom
					Device.BeginInvokeOnMainThread(() => { ScrollTo(group, item, toPosition, shouldAnimate, includeGroup); });
				};
				Control.Loaded += loadedHandler;
				return;
			}
			var templatedItems = TemplatedItemsView.TemplatedItems;
			Tuple<int, int> location = templatedItems.GetGroupAndIndexOfItem(group, item);
			if (location.Item1 == -1 || location.Item2 == -1)
				return;

			var t = templatedItems.GetGroup(location.Item1).ToArray();
			var c = t[location.Item2];
			
			Device.BeginInvokeOnMainThread(() =>
			{
				switch (toPosition)
				{
					case ScrollToPosition.Start:
					{
						viewer.ScrollToBottom();
						Control.ScrollIntoView(c);
						return;
					}

					case ScrollToPosition.MakeVisible:
					{
						Control.ScrollIntoView(c);
						return;
					}
					case ScrollToPosition.End:
					{
						viewer.ScrollToTop();
						Control.ScrollIntoView(c);
						return;
					}
					case ScrollToPosition.Center:
					{
						ScrollToCenterOfView(Control, c);
						return;
					}
				}
			});
		}
		static void ScrollToCenterOfView(WList control, object item)
		{
			// Scroll immediately if possible
			if (!TryScrollToCenterOfView(control, item))
			{
				control.ScrollIntoView(item);
				control.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
				{
					TryScrollToCenterOfView(control, item);
				}));
			}
		}

		static bool TryScrollToCenterOfView(ItemsControl itemsControl, object item)
		{
			// Find the container
			var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
			if (container == null)
				return false;

			// Find the ScrollContentPresenter
			ScrollContentPresenter presenter = null;
			for (Visual vis = container; vis != null && vis != itemsControl; vis = VisualTreeHelper.GetParent(vis) as Visual)
				if ((presenter = vis as ScrollContentPresenter) != null)
					break;
			if (presenter == null)
				return false;

			// Find the IScrollInfo
			var scrollInfo =
				!presenter.CanContentScroll ? presenter :
				presenter.Content as IScrollInfo ??
				FirstVisualChild(presenter.Content as ItemsPresenter) as IScrollInfo ??
				presenter;

			// Compute the center point of the container relative to the scrollInfo
			var size = container.RenderSize;
			var center = container.TransformToAncestor((Visual)scrollInfo).Transform(new System.Windows.Point(size.Width / 2, size.Height / 2));
			center.Y += scrollInfo.VerticalOffset;
			center.X += scrollInfo.HorizontalOffset;

			// Adjust for logical scrolling
			if (scrollInfo is StackPanel || scrollInfo is VirtualizingStackPanel)
			{
				double logicalCenter = itemsControl.ItemContainerGenerator.IndexFromContainer(container) + 0.5;
				Orientation orientation = scrollInfo is StackPanel ? ((StackPanel)scrollInfo).Orientation : ((VirtualizingStackPanel)scrollInfo).Orientation;
				if (orientation == Orientation.Horizontal)
					center.X = logicalCenter;
				else
					center.Y = logicalCenter;
			}

			// Scroll the center of the container to the center of the viewport
			if (scrollInfo.CanVerticallyScroll)
				scrollInfo.SetVerticalOffset(CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));
			if (scrollInfo.CanHorizontallyScroll)
				scrollInfo.SetHorizontalOffset(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth));
			return true;
		}

		static double CenteringOffset(double center, double viewport, double extent)
		{
			return Math.Min(extent - viewport, Math.Max(0, center - viewport / 2));
		}
		static DependencyObject FirstVisualChild(Visual visual)
		{
			if (visual == null)
				return null;
			if (VisualTreeHelper.GetChildrenCount(visual) == 0)
				return null;
			return VisualTreeHelper.GetChild(visual, 0);
		}
	}
}
