using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Specialized;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Xamarin.Forms.Platform.UWP
{
	public class CarouselViewRenderer : ViewRenderer<CarouselView, UserControl>
	{
		const string PreviousButtonHorizontal = "PreviousButtonHorizontal";
		const string NextButtonHorizontal = "NextButtonHorizontal";
		const string PreviousButtonVertical = "PreviousButtonVertical";
		const string NextButtonVertical = "NextButtonVertical";
		const string DotsPanelName = "dotsPanel";
		const string DotsHPanelName = "dotsHPanel";
		const string DotsVPanelName = "dotsVPanel";
		const string IndicatorsName = "indicators";
		const string FlipViewName = "flipView";
		const string ScrollingHostName = "ScrollingHost";
		const string HPanel = "HPanel";
		const string VPanel = "VPanel";

		FlipViewControl _nativeView;
		FlipView _flipView;
		ColorConverter _converter;
		SolidColorBrush _selectedColor;
		SolidColorBrush _fillColor;
		// To hold all the rendered views
		ObservableCollection<FrameworkElement> Source;
		// To hold the indicators dots
		ObservableCollection<Shape> Dots;
		bool _disposed;
		// To avoid triggering Position changed more than once
		bool _isChangingPosition;
		double _lastOffset;

		ScrollViewer _scrollingHost;
		Windows.UI.Xaml.Controls.Button _prevBtn;
		Windows.UI.Xaml.Controls.Button _nextBtn;

		protected ITemplatedItemsView<View> TemplatedItemsView => Element;

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Unsubscribe from event handlers and cleanup any resources
				if (_flipView != null)
				{
					_flipView.Loaded -= FlipView_Loaded;
					_flipView.SelectionChanged -= FlipView_SelectionChanged;
				}

				((ITemplatedItemsView<View>)e.OldElement).TemplatedItems.CollectionChanged -= ItemsSource_CollectionChanged;
			}

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					_converter = new ColorConverter();

					if (_nativeView == null)
					{
						_nativeView = new FlipViewControl(Element.IsSwipeEnabled);
						_flipView = _nativeView.FindName(FlipViewName) as FlipView;
						_flipView.Loaded += FlipView_Loaded;
						_flipView.SelectionChanged += FlipView_SelectionChanged;

						_selectedColor = (SolidColorBrush)_converter.Convert(e.NewElement.CurrentPageIndicatorTintColor, null, null, null);
						_fillColor = (SolidColorBrush)_converter.Convert(e.NewElement.IndicatorsTintColor, null, null, null);
					}

					UpdateOrientation();

					SetNativeControl(_nativeView);

					UpdateItemsSource();
					UpdateBackgroundColor();
					UpdateIndicatorsTint();
				}

				((ITemplatedItemsView<View>)e.NewElement).TemplatedItems.CollectionChanged += ItemsSource_CollectionChanged;
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Element == null || _flipView == null) return;

			if (e.PropertyName == CarouselView.OrientationProperty.PropertyName)
			{
				UpdateOrientation();
			}
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
			{
				UpdateBackgroundColor();
			}
			else if (e.PropertyName == CarouselView.IsSwipeEnabledProperty.PropertyName)
			{
				_nativeView.IsSwipeEnabled = Element.IsSwipeEnabled;
			}
			else if (e.PropertyName == CarouselView.IndicatorsTintColorProperty.PropertyName)
			{
				_fillColor = (SolidColorBrush)_converter.Convert(Element.IndicatorsTintColor, null, null, null);
				UpdateIndicatorsTint();
			}
			else if (e.PropertyName == CarouselView.CurrentPageIndicatorTintColorProperty.PropertyName)
			{
				_selectedColor = (SolidColorBrush)_converter.Convert(Element.CurrentPageIndicatorTintColor, null, null, null);
				UpdateIndicatorsTint();
			}
			else if ((e.PropertyName == CarouselView.IndicatorsShapeProperty.PropertyName) || (e.PropertyName == CarouselView.ShowIndicatorsProperty.PropertyName))
			{
				SetIndicators();
			}
			else if (e.PropertyName == CarouselView.ItemsSourceProperty.PropertyName)
			{
				UpdateItemsSource();
			}
			else if (e.PropertyName == CarouselView.ItemTemplateProperty.PropertyName)
			{
				UpdateItemsSource();
			}
			else if (e.PropertyName == CarouselView.PositionProperty.PropertyName)
			{
				if (!_isChangingPosition)
				{
					SetCurrentPage(Element.Position);
				}
			}
			else if (e.PropertyName == CarouselView.ShowArrowsProperty.PropertyName)
			{
				UpdateArrows();
			}
		}

		protected override void UpdateBackgroundColor()
		{
			if (_flipView == null || Element == null)
				return;
			_flipView.Background = (SolidColorBrush)_converter.Convert(Element.BackgroundColor, null, null, null);
		}

		void Reset()
		{
			UpdateItemsSource();
		}

		void UpdateArrows()
		{
			FlipView_Loaded(_flipView, null);
		}

		void UpdateItemsSource()
		{
			_isChangingPosition = true;
			var source = new List<FrameworkElement>();
			var elementCount = GetItemCount();
			if (elementCount > 0)
			{
				for (int j = 0; j <= elementCount - 1; j++)
				{
					var item = GetItem(j);
					source.Add(CreateView(item));
				}
			}

			Source = new ObservableCollection<FrameworkElement>(source);
			_flipView.ItemsSource = Source;
			SetArrowsVisibility();
			SetIndicators();
			SetCurrentPage(Element.Position);
			_isChangingPosition = false;
		}

		void UpdateOrientation()
		{
			if (Element.Orientation == CarouselOrientation.Horizontal)
				_flipView.ItemsPanel = _nativeView.Resources[HPanel] as ItemsPanelTemplate;
			else
				_flipView.ItemsPanel = _nativeView.Resources[VPanel] as ItemsPanelTemplate;

			_flipView.UpdateLayout();
		}

		void ScrollingHost_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
		{
			var scrollView = (ScrollViewer)sender;

			double percentCompleted;
			ScrollDirection direction;

			// Get Horizontal or Vertical Offset depending on carousel orientation
			var currentOffset = Element.Orientation == CarouselOrientation.Horizontal ? scrollView.HorizontalOffset : scrollView.VerticalOffset;

			// Scrolling to the right
			if (currentOffset > _lastOffset)
			{
				percentCompleted = Math.Floor((currentOffset - (int)currentOffset) * 100);
				direction = Element.Orientation == CarouselOrientation.Horizontal ? ScrollDirection.Right : ScrollDirection.Down;
			}
			else
			{
				percentCompleted = Math.Floor((_lastOffset - currentOffset) * 100);
				direction = Element.Orientation == CarouselOrientation.Horizontal ? ScrollDirection.Left : ScrollDirection.Up;
			}

			if (percentCompleted <= 100)
				Element.SendScrolled(percentCompleted, direction);
		}

		void ScrollingHost_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (!e.IsIntermediate)
			{
				var scrollView = (ScrollViewer)sender;
				_lastOffset = Element.Orientation == CarouselOrientation.Horizontal ? scrollView.HorizontalOffset : scrollView.VerticalOffset;
			}
		}

		async void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Element == null || _flipView == null || Source == null) return;

			// NewItems contains the item that was added.
			// If NewStartingIndex is not -1, then it contains the index where the new item was added.
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				View item = GetItem(e.NewStartingIndex);
				InsertPage(item, e.NewStartingIndex);
			}

			// OldItems contains the item that was removed.
			// If OldStartingIndex is not -1, then it contains the index where the old item was removed.
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				await RemovePageAsync(e.OldStartingIndex);
			}

			// OldItems contains the moved item.
			// OldStartingIndex contains the index where the item was moved from.
			// NewStartingIndex contains the index where the item was moved to.
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				var obj = Source[e.OldStartingIndex];
				Source.RemoveAt(e.OldStartingIndex);
				Source.Insert(e.NewStartingIndex, obj);

				SetCurrentPage(Element.Position);
			}

			// NewItems contains the replacement item.
			// NewStartingIndex and OldStartingIndex are equal, and if they are not -1,
			// then they contain the index where the item was replaced.
			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				Source[e.OldStartingIndex] = CreateView(e.NewItems[e.NewStartingIndex]);
			}

			// No other properties are valid.
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Reset();
				SetArrowsVisibility();
			}
		}

		void FlipView_Loaded(object sender, RoutedEventArgs e)
		{
			ButtonHide(_flipView, PreviousButtonHorizontal);
			ButtonHide(_flipView, NextButtonHorizontal);
			ButtonHide(_flipView, PreviousButtonVertical);
			ButtonHide(_flipView, NextButtonVertical);

			if (_scrollingHost == null)
			{
				_scrollingHost = FindVisualChild<Windows.UI.Xaml.Controls.ScrollViewer>(_flipView, ScrollingHostName);

				_scrollingHost.ViewChanging += ScrollingHost_ViewChanging;
				_scrollingHost.ViewChanged += ScrollingHost_ViewChanged;
			}

			SetArrows();
		}

		void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Element != null && !_isChangingPosition)
			{
				var position = _flipView.SelectedIndex;
				UpdatePositionFromRenderer(position);
				UpdateIndicatorsTint();
			}
		}
		void UpdatePositionFromRenderer(int position)
		{
			if (position == -1)
				return;
			Element.NotifyPositionChanged(position);
		}

		void SetArrows()
		{
			if (Element.Orientation == CarouselOrientation.Horizontal)
			{
				_prevBtn = FindVisualChild<Windows.UI.Xaml.Controls.Button>(_flipView, PreviousButtonHorizontal);
				_nextBtn = FindVisualChild<Windows.UI.Xaml.Controls.Button>(_flipView, NextButtonHorizontal);
			}
			else
			{
				_prevBtn = FindVisualChild<Windows.UI.Xaml.Controls.Button>(_flipView, PreviousButtonVertical);
				_nextBtn = FindVisualChild<Windows.UI.Xaml.Controls.Button>(_flipView, NextButtonVertical);
			}

			// TODO: Set BackgroundColor, TintColor and Transparency
		}

		void SetArrowsVisibility()
		{
			if (_prevBtn == null || _nextBtn == null) return;
			var elementCount = GetItemCount();
			_prevBtn.Visibility = Element.Position == 0 || elementCount == 0 ? Visibility.Collapsed : Visibility.Visible;
			_nextBtn.Visibility = Element.Position == elementCount - 1 || elementCount == 0 ? Visibility.Collapsed : Visibility.Visible;
		}

		void SetIndicators()
		{
			var dotsPanel = _nativeView.FindName(DotsPanelName) as ItemsControl;
			var indicators = _nativeView.FindName(IndicatorsName) as StackPanel;

			if (Element.ShowIndicators)
			{
				if (Element.Orientation == CarouselOrientation.Horizontal)
				{
					indicators.HorizontalAlignment = HorizontalAlignment.Stretch;
					indicators.VerticalAlignment = VerticalAlignment.Bottom;
					indicators.Width = Double.NaN;
					indicators.Height = 32;
					dotsPanel.HorizontalAlignment = HorizontalAlignment.Center;
					dotsPanel.VerticalAlignment = VerticalAlignment.Bottom;
					dotsPanel.ItemsPanel = _nativeView.Resources[DotsHPanelName] as ItemsPanelTemplate;
				}
				else
				{
					indicators.HorizontalAlignment = HorizontalAlignment.Right;
					indicators.VerticalAlignment = VerticalAlignment.Center;
					indicators.Width = 32;
					indicators.Height = Double.NaN;
					dotsPanel.HorizontalAlignment = HorizontalAlignment.Center;
					dotsPanel.VerticalAlignment = VerticalAlignment.Center;
					dotsPanel.ItemsPanel = _nativeView.Resources[DotsVPanelName] as ItemsPanelTemplate;
				}

				var dots = new List<Shape>();

				var itemCount = GetItemCount();
				if (itemCount > 0)
				{
					for (int i = 0; i < itemCount; i++)
					{
						dots.Add(CreateDot(i, Element.Position));
					}
				}

				Dots = new ObservableCollection<Shape>(dots);
				dotsPanel.ItemsSource = Dots;
			}
			else
			{
				dotsPanel.ItemsSource = new List<Shape>();
			}

			// ShowIndicators BP
			indicators.Visibility = Element.ShowIndicators ? Visibility.Visible : Visibility.Collapsed;
		}

		int GetItemCount()
		{
			if (TemplatedItemsView == null)
				return -1;
			return TemplatedItemsView.TemplatedItems.Count;
		}

		View GetItem(int index)
		{
			return TemplatedItemsView.TemplatedItems[index];
		}

		void UpdateIndicatorsTint()
		{
			var dotsPanel = _nativeView.FindName(DotsPanelName) as ItemsControl;
			int i = 0;
			foreach (var item in dotsPanel.Items)
			{
				((Shape)item).Fill = i == Element.Position ? _selectedColor : _fillColor;
				i++;
			}
		}

		void InsertPage(object item, int position)
		{
			if (Element == null || _flipView == null || Source == null) return;

			if (position <= Element.Position)
			{
				_isChangingPosition = true;
				var newPosition = Element.Position + 1;
				UpdatePositionFromRenderer(newPosition);
				_isChangingPosition = false;
			}

			Source.Insert(position, CreateView(item));
			Dots?.Insert(position, CreateDot(position, Element.Position));

			_flipView.SelectedIndex = Element.Position;
		}

		async Task RemovePageAsync(int position)
		{
			if (Element == null || _flipView == null || Source == null) return;

			if (Source?.Count > 0)
			{
				// To remove latest page, rebuild flipview or the page wont disappear
				if (Source.Count == 1)
				{
					Reset();
				}
				else
				{

					_isChangingPosition = true;

					// To remove current page
					if (position == Element.Position)
					{
						// Swipe animation at position 0 doesn't work :(
						/*if (position == 0)
                        {
                            flipView.SelectedIndex = 1;
                        }
                        else
                        {*/
						if (position > 0)
						{
							var newPos = position - 1;
							if (newPos == -1)
								newPos = 0;

							_flipView.SelectedIndex = newPos;
						}

						// With a swipe transition
						if (Element.AnimateTransition)
							await Task.Delay(100);
					}

					Source.RemoveAt(position);

					UpdatePositionFromRenderer(_flipView.SelectedIndex);

					Dots?.RemoveAt(position);
					UpdateIndicatorsTint();

					_isChangingPosition = false;
				}
			}
		}

		void SetCurrentPage(int position)
		{
			if (_flipView == null || TemplatedItemsView == null) return;

			var elementCount = GetItemCount();

			if (position < 0 || position > elementCount - 1)
				return;

			if (elementCount > 0)
				_flipView.SelectedIndex = position;
		}

		FrameworkElement CreateView(object item)
		{
			Xamarin.Forms.View formsView = null;
			var bindingContext = item;

			var dt = bindingContext as Xamarin.Forms.DataTemplate;
			var view = bindingContext as View;

			// Support for List<DataTemplate> as ItemsSource
			if (dt != null)
			{
				formsView = (Xamarin.Forms.View)dt.CreateContent();
			}
			else
			{
				if (view != null)
				{
					formsView = view;
				}
				else
				{
					var selector = Element.ItemTemplate as Xamarin.Forms.DataTemplateSelector;
					if (selector != null)
						formsView = (Xamarin.Forms.View)selector.SelectTemplate(bindingContext, Element).CreateContent();
					else
						formsView = (Xamarin.Forms.View)Element.ItemTemplate.CreateContent();

					formsView.BindingContext = bindingContext;
				}
			}

			var element = formsView.ToWindows(new Xamarin.Forms.Rectangle(0, 0, Element.Bounds.Width, Element.Bounds.Height));

			return element;
		}

		Shape CreateDot(int i, int position)
		{
			if (Element.IndicatorsShape == IndicatorsShape.Circle)
			{
				return new Ellipse()
				{
					Fill = i == position ? _selectedColor : _fillColor,
					Height = 7,
					Width = 7,
					Margin = new Windows.UI.Xaml.Thickness(4, 12, 4, 12)
				};
			}
			else
			{
				return new Windows.UI.Xaml.Shapes.Rectangle()
				{
					Fill = i == position ? _selectedColor : _fillColor,
					Height = 6,
					Width = 6,
					Margin = new Windows.UI.Xaml.Thickness(4, 12, 4, 12)
				};
			}
		}

		void ButtonHide(FlipView f, string name)
		{
			var b = FindVisualChild<Windows.UI.Xaml.Controls.Button>(f, name);
			if (b != null)
			{
				b.Opacity = Element.ShowArrows ? 1.0 : 0.0;
				b.IsHitTestVisible = Element.ShowArrows;
			}
		}

		ChildItemType FindVisualChild<ChildItemType>(DependencyObject obj, string name) where ChildItemType : FrameworkElement
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child is ChildItemType && ((FrameworkElement)child).Name == name)
					return (ChildItemType)child;
				else
				{
					ChildItemType childOfChild = FindVisualChild<ChildItemType>(child, name);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		List<Control> AllChildren(DependencyObject parent)
		{
			var _list = new List<Control>();
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				var _child = VisualTreeHelper.GetChild(parent, i);
				if (_child is Control)
					_list.Add(_child as Control);
				_list.AddRange(AllChildren(_child));
			}
			return _list;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_prevBtn = null;
				_nextBtn = null;

				Source = null;
				Dots = null;

				if (_flipView != null)
				{
					_flipView.Loaded -= FlipView_Loaded;
					_flipView.SelectionChanged -= FlipView_SelectionChanged;
					_flipView = null;
				}

				if (Element != null)
				{
					((ITemplatedItemsView<View>)Element).TemplatedItems.CollectionChanged -= ItemsSource_CollectionChanged;
				}

				_nativeView = null;

				_disposed = true;
			}

			base.Dispose(disposing);
		}

	}

	// UWP DataTemplate doesn't support loadTemplate function as parameter
	// Having that, rendering all the views ahead of time is not needed
	/*public class MyTemplateSelector : DataTemplateSelector
    {
        CarouselView Element;

        public MyTemplateSelector(CarouselView element)
        {
            Element = element;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Xamarin.Forms.View formsView = null;
            var bindingContext = item;

            var dt = bindingContext as Xamarin.Forms.DataTemplate;

            // Support for List<DataTemplate> as ItemsSource
            if (dt != null)
            {
                formsView = (Xamarin.Forms.View)dt.CreateContent();
            }
            else {

                var selector = Element.ItemTemplate as Xamarin.Forms.DataTemplateSelector;
                if (selector != null)
                    formsView = (Xamarin.Forms.View)selector.SelectTemplate(bindingContext, Element).CreateContent();
                else
                    formsView = (Xamarin.Forms.View)Element.ItemTemplate.CreateContent();

                formsView.BindingContext = bindingContext;
            }

            formsView.Parent = this.Element;


            var element = FormsViewToNativeUWP.ConvertFormsToNative(formsView, new Xamarin.Forms.Rectangle(0, 0, ElementWidth, ElementHeight));

            var template = new DataTemplate(() => return element; ); // THIS IS NOT SUPPORTED :(

            return template;
        }
    }*/
}