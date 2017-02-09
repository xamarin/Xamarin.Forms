using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SLBinding = System.Windows.Data.Binding;
using WList= System.Windows.Controls.ListView;
namespace Xamarin.Forms.Platform.WPF
{

    public class ListViewRenderer : ViewRenderer<ListView, WList>
    {
        public static readonly DependencyProperty HighlightWhenSelectedProperty = DependencyProperty.RegisterAttached("HighlightWhenSelected", typeof(bool), typeof(ListViewRenderer),
            new PropertyMetadata(false));

        readonly List<Tuple<FrameworkElement, SLBinding, Brush>> _previousHighlights = new List<Tuple<FrameworkElement, SLBinding, Brush>>();

        Animatable _animatable;
        object _fromNative;
        bool _itemNeedsSelecting;
        WList _listBox;
        System.Windows.Controls.ProgressBar _progressBar;

        //ViewportControl _viewport;

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

            //Element.ScrollToRequested += OnScrollToRequested;

            if (Element.SelectedItem != null)
                _itemNeedsSelecting = true;

            _listBox = new WList
            {
                DataContext = Element,
                ItemsSource = Element.TemplatedItems,
                ItemTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["CellTemplate"],
                BorderThickness = new System.Windows.Thickness(0),
                Cursor = Cursors.Hand,
                //GroupHeaderTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["ListViewHeader"],
                //ListHeaderTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["View"],
                //ListFooterTemplate = (System.Windows.DataTemplate)System.Windows.Application.Current.Resources["View"]
            };
            _listBox.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            _listBox.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            //_listBox.SetBinding(LongListSelector.IsGroupingEnabledProperty, new SLBinding("IsGroupingEnabled"));

            _listBox.SelectionChanged += OnNativeSelectionChanged;
            //_listBox.Tap += OnNativeItemTapped;
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

        //Cell FindCell(GestureEventArgs e, out FrameworkElement element)
        //{
        //    Cell cell = null;
        //    element = e.OriginalSource as FrameworkElement;
        //    if (element != null)
        //        cell = element.DataContext as Cell;

        //    if (cell == null)
        //    {
        //        System.Windows.Point pos = e.GetPosition(_listBox);
        //        IEnumerable<UIElement> elements = VisualTreeHelper.FindElementsInHostCoordinates(pos, _listBox);
        //        foreach (FrameworkElement frameworkElement in elements.OfType<FrameworkElement>())
        //        {
        //            if ((cell = frameworkElement.DataContext as Cell) != null)
        //            {
        //                element = frameworkElement;
        //                break;
        //            }
        //        }
        //    }

        //    return cell;
        //}

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

        double GetHeight(Dictionary<System.Windows.DataTemplate, FrameworkElement> reusables, System.Windows.DataTemplate template, object bindingContext)
        {
            double width = Control.ActualWidth;

            FrameworkElement content;
            if (!reusables.TryGetValue(template, out content))
            {
                content = (FrameworkElement)template.LoadContent();

                // Windows Phone refuses to properly bind things on a first pass or even a second pass unless it has different content
                // so we'll force it to cycle here the first time for each template.
                content.DataContext = bindingContext;
                content.Measure(new System.Windows.Size(width, double.PositiveInfinity));
                content.DataContext = null;
                content.Measure(new System.Windows.Size(width, double.PositiveInfinity));

                var control = content as Control;
                if (control != null)
                {
                    // Since we're not adding to the visual tree, we need to inherit the font to measure correctly.
                    control.FontFamily = Control.FontFamily;
                }

                reusables[template] = content;
            }

            content.DataContext = bindingContext;
            content.Measure(new System.Windows.Size(width, double.PositiveInfinity));
            return content.DesiredSize.Height;
        }

        //void OnItemRealized(object sender, ItemRealizationEventArgs e)
        //{
        //    if (!_itemNeedsSelecting)
        //        return;

        //    var cell = e.Container.DataContext as Cell;
        //    if (cell == null || !Equals(cell.BindingContext, Element.SelectedItem))
        //        return;

        //    _itemNeedsSelecting = false;
        //    OnItemSelected(Element.SelectedItem);
        //}

        void OnItemSelected(object selectedItem)
        {
            if (_fromNative != null && Equals(selectedItem, _fromNative))
            {
                _fromNative = null;
                return;
            }

            RestorePreviousSelectedVisual();

            if (selectedItem == null)
            {
                //HACK: not implemented exception on Xamarin.Forms.Core
                try
                {
                    _listBox.SelectedIndex = -1;
                }
                catch (Exception)
                {
                    
                }   
                
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

        //void OnNativeItemTapped(object sender, GestureEventArgs e)
        //{
        //    var cell = (Cell)Control.SelectedItem;
        //    if (cell == null)
        //        return;

        //    Cell parentCell = null;

        //    if (Element.IsGroupingEnabled)
        //    {
        //        TemplatedItemsList<ItemsView<Cell>, Cell> til = TemplatedItemsList<ItemsView<Cell>, Cell>.GetGroup(cell);
        //        parentCell = til.HeaderContent;
        //    }

        //    _fromNative = cell.BindingContext;

        //    if (Element.IsGroupingEnabled)
        //    {
        //        Element.NotifyRowTapped(TemplatedItemsList<ItemsView<Cell>, Cell>.GetIndex(parentCell), TemplatedItemsList<ItemsView<Cell>, Cell>.GetIndex(cell));
        //    }
        //    else
        //        Element.NotifyRowTapped(TemplatedItemsList<ItemsView<Cell>, Cell>.GetIndex(cell));
        //}

        void OnNativeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            var cell = (Cell)e.AddedItems[0];

            if (cell == null)
            {
                return;
            }


            Cell parentCell = null;

            if (Element.IsGroupingEnabled)
            {
                TemplatedItemsList<ItemsView<Cell>, Cell> til = TemplatedItemsList<ItemsView<Cell>, Cell>.GetGroup(cell);
                parentCell = til.HeaderContent;
            }

            _fromNative = cell.BindingContext;

            if (Element.IsGroupingEnabled)
            {
                Element.NotifyRowTapped(TemplatedItemsList<ItemsView<Cell>, Cell>.GetIndex(parentCell), TemplatedItemsList<ItemsView<Cell>, Cell>.GetIndex(cell));
            }
            else
                Element.NotifyRowTapped(TemplatedItemsList<ItemsView<Cell>, Cell>.GetIndex(cell));
        }

        void OnPullToRefreshCanceled(object sender, EventArgs args)
        {
            if (Element.IsPullToRefreshEnabled && ((IListViewController)Element).RefreshAllowed)
                _progressBar.Visibility = Visibility.Collapsed;
        }

        void OnPullToRefreshCompleted(object sender, EventArgs args)
        {
            if (Element.IsPullToRefreshEnabled && ((IListViewController)Element).RefreshAllowed)
            {
                _progressBar.IsIndeterminate = true;
                ((IListViewController)Element).SendRefreshing();
            }
        }

        //void OnPullToRefreshStarted(object sender, EventArgs args)
        //{
        //    if (Element.IsPullToRefreshEnabled && ((IListViewController)Element).RefreshAllowed)
        //    {
        //        _progressBar.Visibility = Visibility.Visible;
        //        _progressBar.IsIndeterminate = false;
        //        _progressBar.Value = Math.Max(0, Math.Min(1, _listBox.PullToRefreshStatus));
        //    }
        //}

        //void OnPullToRefreshStatusUpdated(object sender, EventArgs eventArgs)
        //{
        //    if (Element.IsPullToRefreshEnabled && ((IListViewController)Element).RefreshAllowed)
        //        _progressBar.Value = Math.Max(0, Math.Min(1, _listBox.PullToRefreshStatus));
        //}

        //void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        //{
        //    if (_animatable == null && e.ShouldAnimate)
        //        _animatable = new Animatable();

        //    if (_viewport == null)
        //    {
        //        // Making sure we're actually loaded
        //        if (VisualTreeHelper.GetChildrenCount(_listBox) == 0)
        //        {
        //            RoutedEventHandler handler = null;
        //            handler = (o, args) =>
        //            {
        //                Control.Loaded -= handler;
        //                OnScrollToRequested(sender, e);
        //            };
        //            Control.Loaded += handler;

        //            return;
        //        }
        //        _viewport = (ViewportControl)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(_listBox, 0), 0), 0);
        //        if (_viewport.Viewport.Bottom == 0)
        //        {
        //            EventHandler<ViewportChangedEventArgs> viewportChanged = null;
        //            viewportChanged = (o, args) =>
        //            {
        //                if (_viewport.Viewport.Bottom == 0)
        //                    return;

        //                _viewport.ViewportChanged -= viewportChanged;
        //                OnScrollToRequested(sender, e);
        //            };
        //            _viewport.ViewportChanged += viewportChanged;
        //            return;
        //        }
        //    }

        //    double y = 0;
        //    double targetHeight = 0;
        //    double targetHeaderHeight = 0;

        //    var templateReusables = new Dictionary<System.Windows.DataTemplate, FrameworkElement>();

        //    var found = false;

        //    if (Element.IsGroupingEnabled)
        //    {
        //        for (var g = 0; g < Element.TemplatedItems.Count; g++)
        //        {
        //            if (found)
        //                break;

        //            TemplatedItemsList<ItemsView<Cell>, Cell> til = Element.TemplatedItems.GetGroup(g);

        //            double headerHeight = GetHeight(templateReusables, Control.GroupHeaderTemplate, til);
        //            y += headerHeight;

        //            for (var i = 0; i < til.Count; i++)
        //            {
        //                Cell cell = til[i];

        //                double contentHeight = GetHeight(templateReusables, Control.ItemTemplate, cell);

        //                if ((ReferenceEquals(til.BindingContext, e.Group) || e.Group == null) && ReferenceEquals(cell.BindingContext, e.Item))
        //                {
        //                    targetHeaderHeight = headerHeight;
        //                    targetHeight = contentHeight;
        //                    found = true;
        //                    break;
        //                }

        //                y += contentHeight;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (var i = 0; i < Element.TemplatedItems.Count; i++)
        //        {
        //            Cell cell = Element.TemplatedItems[i];

        //            double height = GetHeight(templateReusables, Control.ItemTemplate, cell);

        //            if (ReferenceEquals(cell.BindingContext, e.Item))
        //            {
        //                found = true;
        //                targetHeight = height;
        //                break;
        //            }

        //            y += height;
        //        }
        //    }

        //    if (!found)
        //        return;

        //    ScrollToPosition position = e.Position;
        //    if (position == ScrollToPosition.MakeVisible)
        //    {
        //        if (y >= _viewport.Viewport.Top && y <= _viewport.Viewport.Bottom)
        //            return;
        //        if (y > _viewport.Viewport.Bottom)
        //            position = ScrollToPosition.End;
        //        else
        //            position = ScrollToPosition.Start;
        //    }

        //    if (position == ScrollToPosition.Start && Element.IsGroupingEnabled)
        //        y = y - targetHeaderHeight;
        //    else if (position == ScrollToPosition.Center)
        //        y = y - (_viewport.ActualHeight / 2 + targetHeight / 2);
        //    else if (position == ScrollToPosition.End)
        //        y = y - _viewport.ActualHeight + targetHeight;

        //    double startY = _viewport.Viewport.Y;
        //    double distance = y - startY;

        //    if (e.ShouldAnimate)
        //    {
        //        var animation = new Animation(v => { _viewport.SetViewportOrigin(new System.Windows.Point(0, startY + distance * v)); });

        //        animation.Commit(_animatable, "ScrollTo", length: 500, easing: Easing.CubicInOut);
        //    }
        //    else
        //        _viewport.SetViewportOrigin(new System.Windows.Point(0, y));
        //}

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
                toHighlight.SetForeground((Brush)System.Windows.Application.Current.Resources["PhoneAccentBrush"]);
            }
        }

        void UpdateFooter()
        {
            //Control.ListFooter = ((IListViewController)Element).FooterElement;
        }

        void UpdateHeader()
        {
            //Control.ListHeader = ((IListViewController)Element).HeaderElement;
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
                //_progressBar.Visibility = _listBox.IsInPullToRefresh && Element.IsPullToRefreshEnabled && ((IListViewController)Element).RefreshAllowed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void UpdateJumpList()
        {
            //if (_listBox.IsGroupingEnabled && Element.GroupShortNameBinding == null)
            //    _listBox.JumpListStyle = null;
            //else
            //    _listBox.JumpListStyle = (System.Windows.Style)System.Windows.Application.Current.Resources["HeaderJumpStyle"];
        }
    }
}