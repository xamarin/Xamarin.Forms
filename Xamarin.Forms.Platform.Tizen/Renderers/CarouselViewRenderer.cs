using ElmSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Xamarin.Forms.Platform.Tizen
{
	class CarouselViewRenderer : ViewRenderer<CarouselView, Box>
	{
		INotifyCollectionChanged _observableSource = null;
		List<View> _createdView = new List<View>();
		List<IndexItem> _indicatorItems = new List<IndexItem>();

		Native.Canvas _innerLayout;
		Native.Scroller _scroller;
		Index _indicator;

		int _changedByScroll = 0;
		int _isLayouting = 0;

		int _prevPoint = 0;

		public CarouselViewRenderer()
		{
			RegisterPropertyHandler(CarouselView.ItemsSourceProperty, UpdateItemSource);
			RegisterPropertyHandler(CarouselView.OrientationProperty, UpdateOrientation);
			RegisterPropertyHandler(CarouselView.PositionProperty, UpdatePosition);
			RegisterPropertyHandler(CarouselView.ShowIndicatorsProperty, UpdateShowIndicators);
			RegisterPropertyHandler(CarouselView.ItemTemplateProperty, UpdateItemTemplate);
			RegisterPropertyHandler(CarouselView.IsSwipeEnabledProperty, UpdateSwipeEnable);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			if (Control == null)
			{
				InitializeNativeControl();
			}
			base.OnElementChanged(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_observableSource != null)
				{
					_observableSource.CollectionChanged -= OnCollectionChanged;
				}
			}
			base.Dispose(disposing);
		}

		void InitializeNativeControl()
		{
			SetNativeControl(new Box(Forms.NativeParent));
			Control.SetLayoutCallback(OnLayoutUpdate);

			_scroller = new Native.Scroller(Forms.NativeParent);
			_indicator = new Index(Forms.NativeParent);
			_innerLayout = new Native.Canvas(Forms.NativeParent);
			// Replace LayoutCallback because Canvas LayoutUpdated was not called when geometry was not changed, but when items order was changed, need to re-layouting
			_innerLayout.SetLayoutCallback(OnInnerLayoutUpdate);

			_scroller.HorizontalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Invisible;
			_scroller.VerticalScrollBarVisiblePolicy = ScrollBarVisiblePolicy.Invisible;
			_scroller.VerticalPageScrollLimit = 1;
			_scroller.HorizontalPageScrollLimit = 1;
			_scroller.SetPageSize(1.0, 1.0);
			_scroller.PageScrolled += OnPageScrolled;
			_scroller.Scrolled += OnScrolled;

			_indicator.AutoHide = false;
			_indicator.Style = "pagecontrol";
			_indicator.Update(0);
			_indicator.Changed += OnIndicatorChanged;

			_scroller.SetContent(_innerLayout);
			Control.PackEnd(_scroller);
			Control.PackEnd(_indicator);

			_scroller.Show();
			_innerLayout.Show();
			_indicator.Show();
		}

		void OnScrolled(object sender, EventArgs e)
		{
			var isHorizontal = Element.Orientation == CarouselOrientation.Horizontal;
			var region = _scroller.CurrentRegion;
			var pageSize = isHorizontal ? region.Width : region.Height;
			var currentPosition = isHorizontal ? region.X : region.Y;
			double percent =  (currentPosition % pageSize) / (double)pageSize * 100.0;

			ScrollDirection scrollDirection;
			if (isHorizontal)
			{
				scrollDirection = _prevPoint > currentPosition ? ScrollDirection.Left : ScrollDirection.Right;
			}
			else
			{
				scrollDirection = _prevPoint > currentPosition ? ScrollDirection.Up : ScrollDirection.Down;
			}

			_prevPoint = currentPosition;
			Element.SendScrolled(percent, scrollDirection);
		}

		void UpdateItemSource()
		{
			if (_observableSource != null)
			{
				_observableSource.CollectionChanged -= OnCollectionChanged;
				_observableSource = null;
			}
			if (Element.ItemsSource is INotifyCollectionChanged source)
			{
				_observableSource = source;
				_observableSource.CollectionChanged += OnCollectionChanged;
			}

			_innerLayout.Children.Clear();
			ClearCreatedView();
			ClearIndexItems();

			if (Element.ItemsSource != null)
			{
				foreach (var item in Element.ItemsSource)
				{
					var view = CreateContent(item);
					var nativeView = Platform.GetOrCreateRenderer(view).NativeView;
					_innerLayout.Children.Add(nativeView);
					_createdView.Add(view);
					_indicatorItems.Add(_indicator.Append(_createdView.Count.ToString()));
				}
			}
			UpdatePosition(false);
			_indicator.Update(0);
			_indicatorItems[Element.Position].Select(true);
		}

		void UpdateItemTemplate(bool init)
		{
			if (!init)
			{
				UpdateItemSource();
			}
		}

		void UpdateOrientation(bool init)
		{
			_indicator.IsHorizontal = Element.Orientation == CarouselOrientation.Horizontal;
			if (!init)
			{
				Control.MarkChanged();
			}
		}

		void UpdatePosition(bool init)
		{
			if (init || IsChangedByScroll())
				return;

			if (Element.Position >= _createdView.Count)
			{
				Element.SetValueFromRenderer(CarouselView.PositionProperty, _createdView.Count - 1);
			}
			ScrollToPosition(Element.Position);
		}

		void ScrollToPosition(int position)
		{
			int x = 0;
			int y = 0;
			var unused = (Element.Orientation == CarouselOrientation.Horizontal ? x = position : y = position);
			_scroller.ScrollTo(x, y, Element.AnimateTransition);
		}

		void UpdateShowIndicators()
		{
			if (Element.ShowIndicators)
				_indicator.Show();
			else
				_indicator.Hide();
		}

		void UpdateSwipeEnable()
		{
			_scroller.ScrollBlock = Element.IsSwipeEnabled ? ScrollBlock.None : ScrollBlock.Vertical | ScrollBlock.Horizontal;
		}

		View CreateContent(object data)
		{
			View content = null;
			object bindingContext = data;

			// Support for List<DataTemplate> as ItemsSource
			if (bindingContext is DataTemplate template)
			{
				content = (View)template.CreateContent();
			}
			else
			{
				// Support for List<View> as ItemsSource
				if (bindingContext is View view)
				{
					content = view;
				}
				else
				{
					if (Element.ItemTemplate is DataTemplateSelector selector)
						content = (View)selector.SelectTemplate(bindingContext, Element).CreateContent();
					else
						content = (View)Element.ItemTemplate.CreateContent();

					content.BindingContext = bindingContext;
				}
			}

			content.Parent = Element;
			return content;
		}
		void OnIndicatorChanged(object sender, EventArgs e)
		{
			var index = _indicatorItems.IndexOf(_indicator.SelectedItem);
			ScrollToPosition(index);
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
			{
				for (int i = 0; i < e.NewItems.Count; i++)
				{
					View view = CreateContent(e.NewItems[i]);
					var nativeView = Platform.GetOrCreateRenderer(view).NativeView;
					if (e.NewStartingIndex != -1)
					{
						_createdView.Insert(e.NewStartingIndex + i, view);
						_innerLayout.Children.Insert(e.NewStartingIndex + i, nativeView);
					}
					else
					{
						_createdView.Add(view);
						_innerLayout.Children.Add(nativeView);
					}
					_indicatorItems.Add(_indicator.Append(_createdView.Count.ToString()));
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					_innerLayout.Children.RemoveAt(e.OldStartingIndex);
					View view = _createdView[e.OldStartingIndex];
					_createdView.RemoveAt(e.OldStartingIndex);
					Platform.GetRenderer(view)?.Dispose();
					_indicatorItems[_indicatorItems.Count - 1].Delete();
					_indicatorItems.RemoveAt(_indicatorItems.Count - 1);
				}
				if (e.OldStartingIndex == Element.Position)
				{
					int newPos = Math.Max(0, Element.Position - 1);
					Element.Position = newPos;
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Move)
			{
				var view = _createdView[e.OldStartingIndex];
				_createdView.RemoveAt(e.OldStartingIndex);
				_createdView.Insert((e.NewStartingIndex > e.OldStartingIndex) ? e.NewStartingIndex - 1 : e.NewStartingIndex, view);
				var nativeView = _innerLayout.Children[e.OldStartingIndex];
				_innerLayout.Children.RemoveAt(e.OldStartingIndex);
				_innerLayout.Children.Insert((e.NewStartingIndex > e.OldStartingIndex) ? e.NewStartingIndex - 1 : e.NewStartingIndex, nativeView);
			}
			else if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				_createdView[e.NewStartingIndex].Parent = null;
				Platform.GetRenderer(_createdView[e.NewStartingIndex])?.Dispose();

				var view = CreateContent(e.NewItems[0]);
				var nativeView = Platform.GetOrCreateRenderer(view).NativeView;
				_innerLayout.Children[e.NewStartingIndex] = nativeView;
				_createdView[e.NewStartingIndex] = view;
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				_innerLayout.Children.Clear();
				ClearCreatedView();
				ClearIndexItems();
			}
			_indicator.Update(0);
		}

		void OnLayoutUpdate()
		{
			var bound = Control.Geometry;
			_scroller.Geometry = bound;

			if (Element.Orientation == CarouselOrientation.Horizontal)
			{
				bound.Y = (bound.Y + bound.Height - 100);
				bound.Height = 100;
			}
			else
			{
				bound.X = (bound.X + bound.Width - 100);
				bound.Width = 100;
			}
			_indicator.Geometry = bound;
			OnInnerLayoutUpdate();
		}

		void OnInnerLayoutUpdate()
		{
			if (_scroller.Geometry.Width == 0 || _scroller.Geometry.Height == 0)
				return;

			_isLayouting++;
			{
				int baseX = _innerLayout.Geometry.X;
				int baseY = _innerLayout.Geometry.Y;
				Rect bound = _scroller.Geometry;

				int index = 0;
				foreach (var nativeView in _innerLayout.Children)
				{
					if (Element.Orientation == CarouselOrientation.Horizontal)
					{
						bound.X = baseX + index * bound.Width;
					}
					else
					{
						bound.Y = baseY + index * bound.Height;
					}

					_createdView[index].Layout(bound.ToDP());
					nativeView.Geometry = bound;
					index++;
				}
				if (Element.Orientation == CarouselOrientation.Horizontal)
				{
					_innerLayout.MinimumWidth = _innerLayout.Children.Count * bound.Width;
					_innerLayout.MinimumHeight = bound.Height;
				}
				else
				{
					_innerLayout.MinimumWidth = bound.Width;
					_innerLayout.MinimumHeight = _innerLayout.Children.Count * bound.Height;
				}

				int x = 0;
				int y = 0;
				var unused = (Element.Orientation == CarouselOrientation.Horizontal ? x = Element.Position : y = Element.Position);
				_scroller.ScrollTo(x, y, false);
			}
			_isLayouting--;
		}

		void OnPageScrolled(object sender, System.EventArgs e)
		{
			if (IsLayouting())
				return;

			_changedByScroll++;
			{
				var index = Element.Orientation == CarouselOrientation.Horizontal ? _scroller.HorizontalPageIndex : _scroller.VerticalPageIndex;
				Element.NotifyPositionChanged(index);
				_indicatorItems[index].Select(true);
			}
			_changedByScroll--;
		}

		void ClearCreatedView()
		{
			foreach (View view in _createdView)
			{
				view.Parent = null;
				Platform.GetRenderer(view)?.Dispose();
			}
			_createdView.Clear();
		}

		void ClearIndexItems()
		{
			foreach (IndexItem item in _indicatorItems)
			{
				item.Delete();
			}
			_indicatorItems.Clear();
		}

		bool IsChangedByScroll()
		{
			return _changedByScroll > 0;
		}

		bool IsLayouting()
		{
			return _isLayouting > 0;
		}
	}
}
