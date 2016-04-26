using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WFlipView = Windows.UI.Xaml.Controls.FlipView;
using WBinding = Windows.UI.Xaml.Data.Binding;
using WApp = Windows.UI.Xaml.Application;
using WSize = Windows.Foundation.Size;
using WDataTemplate = Windows.UI.Xaml.DataTemplate;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	internal class ItemsSource :
		// must derive from Collection for WindowsPhone to hookup CollectionChanged
		Collection<object>, 
		INotifyCollectionChanged,
		IDisposable
	{

		internal ItemsSource(IList<object> controller)
			: base(controller)
		{
			((ControllerAsList)Items).CollectionChanged += OnCollectionChanged;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public void Dispose()
		{
			var list = (ControllerAsList)Items;
			list.CollectionChanged -= OnCollectionChanged;
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(sender, e);
		}
	}

	internal class ControllerAsList :
		IList<object>,
		INotifyCollectionChanged,
		IDisposable
	{
		// windows phone shouldn't hang if the count is int.MaxValue but it does
		const int s_hack = 5000;

		ICarouselViewController _controller;

		internal ControllerAsList(ICarouselViewController controller)
		{
			_controller = controller;
			_controller.CollectionChanged += OnCollectionChanged;
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(sender, e);
		}

		public object this[int index]
		{
			get {
				return _controller.GetItem(index);
			}
			set {
				throw new NotImplementedException();
			}
		}
		public int Count => _controller.Count == int.MaxValue ? s_hack : _controller.Count;

		public bool IsReadOnly => false;
		public int IndexOf(object item)
		{
			throw new NotSupportedException();
		}
		public void Insert(int index, object item)
		{
			throw new NotSupportedException();
		}
		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
		public void Add(object item)
		{
			throw new NotSupportedException();
		}
		public void Clear()
		{
			throw new NotSupportedException();
		}
		public bool Contains(object item)
		{
			throw new NotSupportedException();
		}
		public void CopyTo(object[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}
		public bool Remove(object item)
		{
			throw new NotSupportedException();
		}
		public IEnumerator<object> GetEnumerator()
		{
			throw new NotSupportedException();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public void Dispose()
		{
			_controller.CollectionChanged -= OnCollectionChanged;
		}
	}

	public class CarouselViewRenderer : ViewRenderer<CarouselView, FrameworkElement>
	{
		WFlipView _flipView;
		ControllerAsList _itemsSource;

		bool _disposed;
		bool _leftAdd;
		int? _initialPosition;

		ICarouselViewController Controller => Element;

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Controller.CollectionChanged -= OnCollectionChanged;

				_flipView.SelectionChanged -= SelectionChanged;

				var itemsSource = (ItemsSource)_flipView.ItemsSource;
				_flipView.ItemsSource = null;
				itemsSource?.Dispose();

				_itemsSource.Dispose();
				_itemsSource = null;
			}

			if (e.NewElement != null)
			{
				if (Element != null)
				{
					if (_flipView == null)
					{
						_flipView = new FlipView 
						{
							IsSynchronizedWithCurrentItem = false,
							ItemTemplate = (WDataTemplate)WApp.Current.Resources["ItemTemplate"]
						};

						_flipView.LayoutUpdated += (o, a) => 
						{
							if (_initialPosition == null)
								return;

							_flipView.SelectedIndex = (int)_initialPosition;
							_initialPosition = null;
						};
					}

					_itemsSource = new ControllerAsList(Element);
					_flipView.ItemsSource = new ItemsSource(_itemsSource);
					_flipView.SelectionChanged += SelectionChanged;
					_initialPosition = Element.Position;

					// Controller.CollectionChanged += OnCollectionChanged;
				}

				if (_flipView != Control)
					SetNativeControl(_flipView);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				if (Element != null)
					Controller.CollectionChanged -= OnCollectionChanged;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Element.Position) && _flipView.SelectedIndex != Element.Position)
			{
				if (!_leftAdd)
					_flipView.SelectedIndex = Element.Position;
				_leftAdd = false;
			}

			if (e.PropertyName == nameof(Element.ItemsSource))
			{
				var itemsSource = (ItemsSource)_flipView.ItemsSource;
				_initialPosition = Element.Position;
				_flipView.ItemsSource = new ItemsSource(_itemsSource);
				itemsSource?.Dispose();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Controller.SendSelectedPositionChanged(_flipView.SelectedIndex);

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex <= Element.Position)
					{
						_leftAdd = true;
						int position = Element.Position + e.NewItems.Count;
						PositionChanged(position);
					}
					break;

				case NotifyCollectionChangedAction.Move:
					break;

				case NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex < Element.Position)
						PositionChanged(Element.Position - e.OldItems.Count);
					break;

				case NotifyCollectionChangedAction.Replace:
					break;

				case NotifyCollectionChangedAction.Reset:
					break;

				default:
					throw new Exception($"Enum value '{(int)e.Action}' is not a member of NotifyCollectionChangedAction enumeration.");
			}
		}

		void PositionChanged(int position)
		{
			if (!_leftAdd)
				_flipView.SelectedIndex = position;
			Element.Position = position;
			Controller.SendSelectedPositionChanged(position);
		}

		void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object[] addedItems = e.AddedItems.ToArray();
			object[] removedItems = e.RemovedItems.ToArray();

			object addedItem = addedItems.SingleOrDefault();
			if (addedItem != null)
				PositionChanged(_flipView.SelectedIndex);
		}
	}

	public class ItemControl : ContentControl
	{
		CarouselView _carouselView;
		object _item;
		View _view;

		public ItemControl()
		{
			DataContextChanged += OnDataContextChanged;
		}

		CarouselView CarouselView => LoadCarouselView();

		IItemViewController Controller => CarouselView;

		protected override WSize ArrangeOverride(WSize finalSize)
		{
			_view.Layout(new Rectangle(0, 0, CarouselView.Width, CarouselView.Height));
			return base.ArrangeOverride(finalSize);
		}

		protected override WSize MeasureOverride(WSize availableSize)
		{
			LoadCarouselView();

			if (_item != null)
			{
				SetDataContext(_item);
				_item = null;
			}

			return base.MeasureOverride(availableSize);
		}

		CarouselView LoadCarouselView()
		{
			if (_carouselView != null)
				return _carouselView;

			DependencyObject parent = VisualTreeHelper.GetParent(this);
			CarouselViewRenderer renderer = default(CarouselViewRenderer);

			do
			{
				if (parent == null)
					return null;

				renderer = parent as CarouselViewRenderer;
				if (renderer != null)
					break;

				parent = VisualTreeHelper.GetParent(parent);
			} while (true);

			_carouselView = renderer.Element;
			return _carouselView;
		}

		void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
		{
			object item = args.NewValue;

			if (_carouselView != null)
				SetDataContext(item);

			else if (item != null)
				_item = item;
		}

		void SetDataContext(object item)
		{
			// type item
			object type = Controller.GetItemType(item);

			// activate item
			_view = Controller.CreateView(type);
			_view.Parent = CarouselView;
			_view.Layout(new Rectangle(0, 0, CarouselView.Width, CarouselView.Height));

			// render item
			IVisualElementRenderer renderer = Platform.CreateRenderer(_view);
			Platform.SetRenderer(_view, renderer);
			Content = renderer;

			// bind item
			Controller.BindView(_view, item);
		}
	}
}