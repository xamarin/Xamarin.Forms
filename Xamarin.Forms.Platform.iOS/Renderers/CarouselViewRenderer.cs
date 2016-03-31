﻿using System;
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

#else
using nfloat=System.Single;
using nint=System.Int32;
using nuint=System.UInt32;
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
	///     <seealso cref="https://developer.apple.com/library/ios/documentation/UIKit/Reference/UICollectionView_class/" />
	/// </summary>
	public class CarouselViewRenderer : ViewRenderer<CarouselView, UICollectionView>
	{
		#region Static Fields
		const int DefaultMinimumDimension = 44;
		static readonly UIColor DefaultBackgroundColor = UIColor.White;
		#endregion

		#region Fields
		CarouselViewController.Layout _layout;
		int _position;
		CarouselViewController _controller;
		#endregion

		new UIScrollView Control
		{
			get
			{
				Initialize();
				return base.Control;
			}
		}
		ICarouselViewController Controller
		{
			get { return Element; }
		}
		void Initialize()
		{
			// cache hit? 
			var carouselView = base.Control;
			if (carouselView != null)
				return;

			_controller = new CarouselViewController(
				renderer: this, 
				layout: _layout = new CarouselViewController.Layout(
					UICollectionViewScrollDirection.Horizontal
				),
				initialPosition: Element.Position
			);

			// hook up on position changed event
			// not ideal; the event is raised upon releasing the swipe instead of animation completion
			//_layout.OnSwipeOffsetChosen += o => OnPositionChange(o);
			_controller.OnWillDisplayCell += o => OnPositionChange(o);

			// hook up crud events
			Element.CollectionChanged += OnCollectionChanged;

			// populate cache
			SetNativeControl(_controller.CollectionView);
		}

		void OnItemChange(int position)
		{
			var item = Controller.GetItem(position);
			Controller.SendSelectedItemChanged(item);
		}
		bool OnPositionChange(int position)
		{
			if (position == _position)
				return false;

			_position = position;
			Element.Position = _position;

			Controller.SendSelectedPositionChanged(position);
			OnItemChange(position);
			return true;
		}
		void ScrollToPosition(int position, bool animated = true)
		{
			if (!OnPositionChange(position))
				return;

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
					if (Element.Count == 0)
						throw new InvalidOperationException("CarouselView must retain a least one item.");

					if (e.OldStartingIndex == _position)
					{
						_controller.DeleteItems(
							Enumerable.Range(e.OldStartingIndex, e.OldItems.Count)
						);
						if (_position == Element.Count)
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
			if (e.PropertyName == "Position" && _position != Element.Position)
				// not ideal; the event is raised before the animation to move completes (or even starts)
				ScrollToPosition(Element.Position);

			base.OnElementPropertyChanged(sender, e);
		}
		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			base.OnElementChanged(e);
			Initialize();
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return Control.GetSizeRequest(widthConstraint, heightConstraint, DefaultMinimumDimension, DefaultMinimumDimension);
		}
	}

	internal sealed class CarouselViewController : UICollectionViewController
	{
		internal new sealed class Layout : UICollectionViewFlowLayout
		{
			static readonly nfloat ZeroMinimumInteritemSpacing = 0;
			static readonly nfloat ZeroMinimumLineSpacing = 0;

			int _width;

			public Layout(UICollectionViewScrollDirection scrollDirection)
			{
				ScrollDirection = scrollDirection;
				MinimumInteritemSpacing = ZeroMinimumInteritemSpacing;
				MinimumLineSpacing = ZeroMinimumLineSpacing;
			}

			public Action<int> OnSwipeOffsetChosen;

			public override UICollectionViewLayoutAttributes InitialLayoutAttributesForAppearingItem(NSIndexPath itemIndexPath)
			{
				return base.InitialLayoutAttributesForAppearingItem(itemIndexPath);
			}
			public override UICollectionViewLayoutAttributes FinalLayoutAttributesForDisappearingItem(NSIndexPath itemIndexPath)
			{
				return base.FinalLayoutAttributesForDisappearingItem(itemIndexPath);
			}
			public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(RectangleF rect)
			{
				// couldn't figure a way to use these values to compute when an element appeared to disappeared. YMMV
				var result = base.LayoutAttributesForElementsInRect(rect);
				foreach (var item in result)
				{
					var index = item.IndexPath;
					var category = item.RepresentedElementCategory;
					var kind = item.RepresentedElementKind;

					var hidden = item.Hidden;
					var bounds = item.Bounds;
					var frame = item.Frame;
					var center = item.Center;

					_width = (int)item.Bounds.Width;
				}
				return result;
			}
			public override SizeF CollectionViewContentSize
			{
				get
				{
					var result = base.CollectionViewContentSize;
					return result;
				}
			}
			public override NSIndexPath GetTargetIndexPathForInteractivelyMovingItem(NSIndexPath previousIndexPath, PointF position)
			{
				var result = base.GetTargetIndexPathForInteractivelyMovingItem(previousIndexPath, position);
				return result;
			}
			public override PointF TargetContentOffsetForProposedContentOffset(PointF proposedContentOffset)
			{
				var result = base.TargetContentOffsetForProposedContentOffset(proposedContentOffset);
				return result;
			}
			public override PointF TargetContentOffset(PointF proposedContentOffset, PointF scrollingVelocity)
			{
				var result = base.TargetContentOffset(proposedContentOffset, scrollingVelocity);
				OnSwipeOffsetChosen?.Invoke((int)result.X / _width);
				return result;
			}
			public override bool ShouldInvalidateLayoutForBoundsChange(RectangleF newBounds)
			{
				return true;
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
				//if (position != this.position)
				//	controller.SendPositionDisappearing (this.position);

				_position = position;
				OnBind?.Invoke(position);

				_controller.BindView(_view, item);
			}

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
					Bind(item, position);

					// render view
					_renderer = Platform.CreateRenderer(_view);
					Platform.SetRenderer(_view, _renderer);

					// attach view
					var uiView = _renderer.NativeView;
					ContentView.AddSubview(uiView);
				}
				else
					Bind(item, position);
			}

			public Action<int> OnBind;
			public override void LayoutSubviews()
			{
				base.LayoutSubviews();

				_renderer.Element.Layout(new Rectangle(0, 0, ContentView.Frame.Width, ContentView.Frame.Height));
			}
		}

		readonly Dictionary<object, int> _typeIdByType;
		UICollectionViewLayout _layout;
		CarouselViewRenderer _renderer;
		int _nextItemTypeId;
		int _initialPosition;

		public CarouselViewController(CarouselViewRenderer renderer, UICollectionViewLayout layout, int initialPosition) : base(layout)
		{
			_renderer = renderer;
			_typeIdByType = new Dictionary<object, int>();
			_nextItemTypeId = 0;
			_layout = layout;
			_initialPosition = initialPosition;
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

		public Action<int> OnBind;
		public Action<int> OnWillDisplayCell;

		public override void WillDisplayCell(UICollectionView collectionView, UICollectionViewCell cell, NSIndexPath indexPath)
		{
			if (_initialPosition != 0) {
				ScrollToPosition(_initialPosition, false);
				_initialPosition = 0;
				return;
			}

			var index = indexPath.Row;
			OnWillDisplayCell?.Invoke(index);
		}
		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			CollectionView.PagingEnabled = true;
			CollectionView.BackgroundColor = UIColor.Clear;
		}
		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			var result = Element.Count;
			return result;
		}
		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var index = indexPath.Row;

			if (_initialPosition != 0)
				index = _initialPosition;

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

			// a semantically weak approach to OnAppearing; decided not to expose as such
			if (cell.OnBind == null)
				cell.OnBind += o => OnBind?.Invoke(o);

			return cell;
		}

		public void ReloadData() => CollectionView.ReloadData();
		public void ReloadItems(IEnumerable<int> positions)
		{
			var indices = positions.Select(o => NSIndexPath.FromRowSection(o, 0)).ToArray();
			CollectionView.ReloadItems(indices);
		}
		public void DeleteItems(IEnumerable<int> positions)
		{
			var indices = positions.Select(o => NSIndexPath.FromRowSection(o, 0)).ToArray();
			CollectionView.DeleteItems(indices);
		}
		public void MoveItem(int oldPosition, int newPosition)
		{
			base.MoveItem(
				CollectionView, 
				NSIndexPath.FromRowSection(oldPosition, 0), 
				NSIndexPath.FromRowSection(newPosition, 0)
			);
		}
		public void ScrollToPosition(int position, bool animated = true)
		{
			CollectionView.ScrollToItem(
				indexPath: NSIndexPath.FromRowSection(position, 0), 
				scrollPosition: UICollectionViewScrollPosition.CenteredHorizontally, 
				animated: animated
			);
		}
	}
}