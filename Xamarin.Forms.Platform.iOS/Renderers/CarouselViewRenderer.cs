using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
#if __UNIFIED__
using UIKit;
using Foundation;
#else
using MonoTouch.UIKit;
using MonoTouch.Foundation;
#endif
#if __UNIFIED__
using RectangleF = CoreGraphics.CGRect;
using SizeF = CoreGraphics.CGSize;
using PointF = CoreGraphics.CGPoint;
using System.Diagnostics;

#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif

namespace Xamarin.Forms.Platform.iOS
{
	/// <summary>
	///     UICollectionView visualizes a collection of data. UICollectionViews are created indirectly by first creating a
	///     CarouselViewController from which the CollectionView is accessed via the CollectionView property.
	///     The CarouselViewController functionality is exposed through a set of interfaces (aka "conforms to" in the Apple
	///     docs).
	///     When Xamarin exposed CarouselViewRenderer the following interfaces where implemented as virtual methods:
	///     UICollectionViewSource
	///     UIScrollViewDelegate
	///     UICollectionViewDelegate		Allow you to manage the selection and highlighting of items in a collection view
	///     UICollectionViewDataSource		Creation and configuration of cells and supplementary views used to display data
	///     The interfaces only implement required method while the UICollectionView exposes optional methods via
	///     ExportAttribute.
	///     The C# method name may be aliased. For example, C# "GetCell" maps to obj-C "CellForItemAtIndexPath".
#pragma warning disable 1584
	///     <seealso cref="https://developer.apple.com/library/ios/documentation/UIKit/Reference/UICollectionView_class/" />
#pragma warning restore 1584
	/// </summary>
	public class CarouselViewRenderer : ViewRenderer<CarouselView, UICollectionView>
	{
		const int DefaultItemsCount = 1;
		const int DefaultMinimumDimension = 44;

		// As on Android, ScrollToPostion from 0 to 2 should not raise OnPositionChanged for 1
		// Tracking the _targetPosition allows for skipping events for intermediate positions
		int? _scrollToTarget;

		int _position;
		bool _disposed;
		CarouselViewController _controller;
		RectangleF _lastBounds;

		ICarouselViewController Controller => Element;
		void Initialize()
		{
			// cache hit? 
			var carouselView = base.Control;
			if (carouselView != null)
				return;

			_lastBounds = Bounds;
			_controller = new CarouselViewController(
				renderer: this
			);

			// hook up on position changed event
			_controller.OnPositionChanged = OnPositionChange;

			// populate cache
			SetNativeControl(_controller.CollectionView);
		}

		void OnItemChange(int position)
		{
			var item = Controller.GetItem(position);
			Controller.SendSelectedItemChanged(item);
		}
		void OnPositionChange(int position)
		{
			// do not report intermediate positions while scrolling
			if (_scrollToTarget != null)
			{
				if (position != _scrollToTarget)
					return;
				_scrollToTarget = null;
			}

			else if (position == _position)
				return;

			_position = position;
			Element.Position = position;

			Controller.SendSelectedPositionChanged(position);
		}
		void ScrollToPosition(int position, bool animated)
		{
			if (position == _position)
				return;

			if (animated)
				_scrollToTarget = position;

			_controller.ScrollToPosition(position, animated);
		}
		void OnCollectionChanged(object source, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					_controller.ReloadData();

					if (e.NewStartingIndex <= _position)
						ShiftPosition(e.NewItems.Count);

					break;

				case NotifyCollectionChangedAction.Move:
					for (var i = 0; i < e.NewItems.Count; i++)
					{
						_controller.MoveItem(
							oldPosition: e.OldStartingIndex + i,
							newPosition: e.NewStartingIndex + i
						);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					if (Controller.Count == 0)
						throw new InvalidOperationException("CarouselView must retain a least one item.");

					if (e.OldStartingIndex == _position)
					{
						_controller.DeleteItems(
							Enumerable.Range(e.OldStartingIndex, e.OldItems.Count)
						);
						if (_position == Controller.Count)
							_position--;
						OnItemChange(_position);
					}

					else
					{
						_controller.ReloadData();

						if (e.OldStartingIndex < _position)
							ShiftPosition(-e.OldItems.Count);
					}

					break;

				case NotifyCollectionChangedAction.Replace:
					_controller.ReloadItems(
						Enumerable.Range(e.OldStartingIndex, e.OldItems.Count)
					);
					break;

				case NotifyCollectionChangedAction.Reset:
					_controller.ReloadData();
					break;

				default:
					throw new Exception();
			}
		}
		void ShiftPosition(int offset)
		{
			// By default the position remains the same which causes an animation in the case
			// of the added/removed position preceding the current position. I prefer the constructed
			// Android behavior whereby the item remains the same and the position changes.
			ScrollToPosition(_position + offset, false);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Position" && _position != Element.Position && !Controller.IgnorePositionUpdates)
				ScrollToPosition(Element.Position, animated: true);

			if (e.PropertyName == "ItemsSource")
			{
				_position = Element.Position;
				_controller.ReloadData(_position);
			}

			base.OnElementPropertyChanged(sender, e);
		}
		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			base.OnElementChanged(e);

			CarouselView oldElement = e.OldElement;
			CarouselView newElement = e.NewElement;
			if (oldElement != null)
				((IItemViewController)oldElement).CollectionChanged -= OnCollectionChanged;

			if (newElement != null)
			{
				if (Control == null)
					Initialize();

				// initialize properties
				_position = Element.Position;

				// hook up crud events
				((IItemViewController)newElement).CollectionChanged += OnCollectionChanged;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				if (Element != null)
					((IItemViewController)Element).CollectionChanged -= OnCollectionChanged;
			}

			base.Dispose(disposing);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (_lastBounds == Bounds)
				return;

			base.Control.ReloadData();
			_lastBounds = Bounds;

			var wasPortrait = _lastBounds.Height > _lastBounds.Width;
			var nowPortrait = Bounds.Height > Bounds.Width;
			if (wasPortrait != nowPortrait)
				_controller.ScrollToPosition(_position, false);
		}
		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return Control.GetSizeRequest(widthConstraint, heightConstraint, DefaultMinimumDimension, DefaultMinimumDimension);
		}
	}

	internal sealed class CarouselViewController : UICollectionViewController
	{
		new sealed class Layout : UICollectionViewFlowLayout
		{
			static readonly nfloat ZeroMinimumInteritemSpacing = 0;
			static readonly nfloat ZeroMinimumLineSpacing = 0;

			public Layout(UICollectionViewScrollDirection scrollDirection)
			{
				ScrollDirection = scrollDirection;
				MinimumInteritemSpacing = ZeroMinimumInteritemSpacing;
				MinimumLineSpacing = ZeroMinimumLineSpacing;
			}
		}
		sealed class Cell : UICollectionViewCell
		{
			IItemViewController _controller;
			int _position;
			IVisualElementRenderer _renderer;
			View _view;

			void Bind(object item, int position)
			{
				_position = position;
				_controller.BindView(_view, item);
			}

			internal int Position => _position;
			[Export("initWithFrame:")]
			internal Cell(RectangleF frame) : base(frame)
			{
				_position = -1;
			}
			internal void Initialize(IItemViewController controller, object itemType, object item, int position)
			{
				_position = position;

				if (_controller == null)
				{
					_controller = controller;

					// create view
					_view = controller.CreateView(itemType);

					// bind view
					Bind(item, _position);

					// render view
					_renderer = Platform.CreateRenderer(_view);
					Platform.SetRenderer(_view, _renderer);

					// attach view
					var uiView = _renderer.NativeView;
					ContentView.AddSubview(uiView);
				}
				else
					Bind(item, _position);
			}

			public override void LayoutSubviews()
			{
				base.LayoutSubviews();
				_renderer.Element.Layout(new Rectangle(0, 0, ContentView.Frame.Width, ContentView.Frame.Height));
			}
		}

		readonly Dictionary<object, int> _typeIdByType;
		CarouselViewRenderer _renderer;
		int _nextItemTypeId;
		int? _initialPosition;
		int _lastPosition;

		internal CarouselViewController(
			CarouselViewRenderer renderer)
			: base(new Layout(UICollectionViewScrollDirection.Horizontal))
		{
			_renderer = renderer;
			_typeIdByType = new Dictionary<object, int>();
			_nextItemTypeId = 0;
			_lastPosition = 0;
		}

		CarouselViewRenderer Renderer => _renderer;
		CarouselView Element => _renderer.Element;
		ICarouselViewController Controller => Element;

		[Export("collectionView:layout:sizeForItemAtIndexPath:")]
		SizeF GetSizeForItem(
			UICollectionView collectionView,
			UICollectionViewLayout layout,
			NSIndexPath indexPath)
		{
			return collectionView.Frame.Size;
		}
		void DisplayCell()
		{
			if (CollectionView.VisibleCells.Length == 0)
				return;

			// only ever seems to be a single cell visible at a time
			var visibleCell = (Cell)CollectionView.VisibleCells[0];
			var position = visibleCell.Position;
			if (position == _lastPosition)
				return;

			_lastPosition = position;
			OnPositionChanged(position);
		}

		internal Action<int> OnPositionChanged;

		public override void CellDisplayingEnded(
			UICollectionView collectionView,
			UICollectionViewCell cell,
			NSIndexPath indexPath)
		{
			if (_initialPosition != null)
				return;

			DisplayCell();
		}
		public override void WillDisplayCell(
			UICollectionView collectionView,
			UICollectionViewCell cell,
			NSIndexPath indexPath)
		{
			// silently scroll to initial position
			if (_initialPosition != null)
			{
				ScrollToPosition((int)_initialPosition, false);
				_initialPosition = null;
				return;
			}

			DisplayCell();
		}
		public override nint NumberOfSections(UICollectionView collectionView) => 1;
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			CollectionView.PagingEnabled = true;
			CollectionView.BackgroundColor = UIColor.Clear;
			CollectionView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
		}
		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			var count = Controller.Count;

			// this happens when CarouselView has a null ItemsSource. CarouselView is *trying* to tell iOS
			// that all positions are valid by saying Count is int.MaxValue and then when iOS asks for any position 
			// the default view can be returned. Unfortunetly, iOS allocates memory upfront for all positions
			// so will hang trying to allocate int.MaxValue slots.

			// Android works because our bespoke renderer lazily allocates memory so can start at any position; 
			// its is more memory efficient that the stock iOS or even Android renderer in this regard. Yea us.
			if (count == int.MaxValue)
				count = _initialPosition + 1 ?? 0;

			return count;
		}
		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var index = indexPath.Row;

			// load initial position then silently scroll to position (see WillDisplayCell)
			if (_initialPosition != null)
			{
				index = (int)_initialPosition;

				// no need to scroll if we're already at the inital position
				if (_initialPosition == _lastPosition)
					_initialPosition = null;
			}

			var item = Controller.GetItem(index);
			var itemType = Controller.GetItemType(item);

			var itemTypeId = default(int);
			if (!_typeIdByType.TryGetValue(itemType, out itemTypeId))
			{
				_typeIdByType[itemType] = itemTypeId = _nextItemTypeId++;
				CollectionView.RegisterClassForCell(typeof(Cell), itemTypeId.ToString());
			}

			var cell = (Cell)CollectionView.DequeueReusableCell(itemTypeId.ToString(), indexPath);
			cell.Initialize(Element, itemType, item, index);

			return cell;
		}

		internal void ReloadData(int? initialPosition = null)
		{
			if (initialPosition == null)
				initialPosition = _lastPosition;

			_initialPosition = initialPosition;
			CollectionView.ReloadData();
		}
		internal void ReloadItems(IEnumerable<int> positions)
		{
			var indices = positions.Select(o => NSIndexPath.FromRowSection(o, 0)).ToArray();
			CollectionView.ReloadItems(indices);
		}
		internal void DeleteItems(IEnumerable<int> positions)
		{
			var indices = positions.Select(o => NSIndexPath.FromRowSection(o, 0)).ToArray();
			CollectionView.DeleteItems(indices);
		}
		internal void MoveItem(int oldPosition, int newPosition)
		{
			base.MoveItem(
				CollectionView,
				NSIndexPath.FromRowSection(oldPosition, 0),
				NSIndexPath.FromRowSection(newPosition, 0)
			);
		}
		internal void ScrollToPosition(int position, bool animated = true)
		{
			CollectionView.ScrollToItem(
				indexPath: NSIndexPath.FromRowSection(position, 0),
				scrollPosition: UICollectionViewScrollPosition.CenteredHorizontally,
				animated: animated
			);
		}
	}
}