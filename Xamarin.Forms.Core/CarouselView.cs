using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CarouselViewRenderer))]
	public class CarouselView : ItemsView, ICarouselViewController
	{
		public static readonly BindableProperty PositionProperty =
			BindableProperty.Create(
				propertyName: nameof(Position), 
				returnType: typeof(int), 
				declaringType: typeof(CarouselView), 
				defaultValue: 0, 
				defaultBindingMode: BindingMode.TwoWay,
				validateValue: (b, o) => ((CarouselView)b).OnValidatePosition((int)o),
				propertyChanged: (b, o, n) => ((CarouselView)b).OnPositionChanged()
			);

		public static readonly BindableProperty ItemProperty =
			BindableProperty.Create(
				propertyName: nameof(Item),
				returnType: typeof(object),
				declaringType: typeof(CarouselView),
				defaultValue: null,
				defaultBindingMode: BindingMode.TwoWay,
				coerceValue: (b, o) => ((CarouselView)b).OnCoerceItem(o)
			);

		static object s_defaultItem = new object();
		static object s_defaultView = new Label();

		readonly DataTemplate _defaultDataTemplate;
		CarouselViewItemSource _itemsSource;
		object _lastItem;
		int _lastPosition;
		bool _ignorePositionUpdate;

		public CarouselView()
		{
			_lastPosition = 0;
			_lastItem = null;
			_defaultDataTemplate = new DataTemplate(() => DefaultView ?? s_defaultView);

			VerticalOptions = LayoutOptions.FillAndExpand;
			HorizontalOptions = LayoutOptions.FillAndExpand;
		}

		public int Position
		{
			get { return (int)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		public object Item
		{
			get { return GetValue(ItemProperty); }
			internal set { SetValue(ItemProperty, value); }
		}
		View DefaultView { get; set; } // vNext feature

		public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
		public event EventHandler<SelectedPositionChangedEventArgs> PositionSelected;

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var minimumSize = new Size(40, 40);
			return new SizeRequest(minimumSize, minimumSize);
		}
		protected override DataTemplate GetDataTemplate(object item)
		{
			if (item == s_defaultItem)
				return _defaultDataTemplate;

			return base.GetDataTemplate(item);
		}
		protected override IReadOnlyList<object> OnInitializeItemSource()
		{
			return _itemsSource = new CarouselViewItemSource();
		}
		protected override IReadOnlyList<object> OnItemsSourceChanging(
			IReadOnlyList<object> itemsSource,
			ref NotifyCollectionChangedEventHandler collectionChanged)
		{
			// when ItemsSource is null any initial position can be selected
			if (itemsSource != null)
			{
				// tell renderer to ignore Position updates while we whack positions
				_itemsSource.ItemsSource = null;

				// when ItemsSource is empty position can and must be zero
				if (itemsSource.Count == 0)
				{
					Item = null;
					Position = 0;
				}

				// we're short on items
				else if (itemsSource.Count <= Position)
				{
					_ignorePositionUpdate = true;
					Position = itemsSource.Count - 1;
					_ignorePositionUpdate = false;
				}
			}

			// intercept calls to ItemSource
			_itemsSource.ItemsSource = itemsSource;

			if (itemsSource == null)
				return _itemsSource;

			// intercept calls from CollectionChanged
			var baseCollectionChanged = collectionChanged;
			collectionChanged = (s, e) =>
			{
				// when user itemsSource is empty provide a default view
				var removeLast = _itemsSource.ItemsSource.Count == 0 && e.Action == NotifyCollectionChangedAction.Remove;

				// when user itemsSource adds first item then reset to clear default view
				var addFirst = _itemsSource.ItemsSource.Count == 1 && e.Action == NotifyCollectionChangedAction.Add;

				if (addFirst || removeLast)
				{
					e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

					// not ideal; happens before default item appears or disappears
					Item = Controller.GetItem(Position);
					SendChangedEvents();
				}

				baseCollectionChanged(s, e);
			};

			return _itemsSource;
		}
		internal override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			Item = Controller.GetItem(Position);

			// notify app of position changes
			Controller.SendSelectedPositionChanged(Position);

			base.OnItemsSourceChanged(oldValue, newValue);
		}

		ICarouselViewController Controller => this;
		bool ICarouselViewController.IgnorePositionUpdates => _itemsSource.IsNull;
		void ICarouselViewController.SendSelectedItemChanged(object item)
		{
			Item = item;
			SendChangedEvents();
		}
		void ICarouselViewController.SendSelectedPositionChanged(int position)
		{
			Position = position;
			Item = Controller.GetItem(position);
			SendChangedEvents();
		}
		void SendChangedEvents()
		{
			if (_lastPosition != Position)
				PositionSelected?.Invoke(this, new SelectedPositionChangedEventArgs(Position));
			_lastPosition = Position;

			if (!Equals(_lastItem, Item))
				ItemSelected?.Invoke(this, new SelectedItemChangedEventArgs(Item));
			_lastItem = Item;
		}

		object OnCoerceItem(object item) => item == s_defaultItem ? null : item;
		void OnPositionChanged()
		{
			// if renderer is ignoring position updates then manually update position
			if (Controller.IgnorePositionUpdates)
			{
				if (!_ignorePositionUpdate)
					SendChangedEvents();
			}
		}
		bool OnValidatePosition(int value)
		{
			if (value < 0)
				return false;

			if (_itemsSource.IsNull)
				return true;

			if (_itemsSource.IsEmpty)
				return value == 0;

			return value < Controller.Count;
		}

		sealed class CarouselViewItemSource : IReadOnlyList<object>
		{
			IReadOnlyList<object> _itemsSource;

			public int Count
			{
				get
				{
					// allow any initial value
					// renderers see infinite list items all of which are s_defaultView
					if (IsNull)
						return int.MaxValue;

					// Position will have been set to 0
					// renderers see a list of a single item which is s_defaultView
					if (IsEmpty)
						return 1;

					return _itemsSource.Count;
				}
			}
			public object this[int index]
			{
				get
				{
					if (IsNullOrEmpty)
						return s_defaultItem;

					return _itemsSource[index];
				}
			}
			public IEnumerator<object> GetEnumerator()
			{
				// ItemsView never actually uses GetEnumerator
				throw new NotSupportedException();
			}
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			internal IReadOnlyList<object> ItemsSource
			{
				get { return _itemsSource; }
				set { _itemsSource = value; }
			}
			internal bool IsNull => _itemsSource == null;
			internal bool IsEmpty => !IsNull && _itemsSource.Count == 0;
			internal bool IsNullOrEmpty => IsNull || IsEmpty;
		}
	}
}