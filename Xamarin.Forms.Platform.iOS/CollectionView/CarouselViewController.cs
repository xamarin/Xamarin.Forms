using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewController : ItemsViewController<CarouselView>
	{
		readonly CarouselView _carouselView;
		bool _viewInitialized;
		List<View> _oldViews;

		public CarouselViewController(CarouselView itemsView, ItemsViewLayout layout) : base(itemsView, layout)
		{
			_carouselView = itemsView;
			CollectionView.AllowsSelection = false;
			CollectionView.AllowsMultipleSelection = false;
			_carouselView.PropertyChanged += CarouselViewPropertyChanged;
			_carouselView.Scrolled += CarouselViewScrolled;
			_oldViews = new List<View>();
		}

		void CarouselViewScrolled(object sender, ItemsViewScrolledEventArgs e)
		{
			UpdateVisualStates();
		}

		protected override UICollectionViewDelegateFlowLayout CreateDelegator()
		{
			return new CarouselViewDelegator(ItemsViewLayout, this);
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = base.GetCell(collectionView, indexPath);

			var element = (cell as CarouselTemplatedCell)?.VisualElementRenderer?.Element;
			if (element != null)
				VisualStateManager.GoToState(element, CarouselView.DefaultItemVisualState);
			return cell;
		}

		// Here because ViewDidAppear (and associates) are not fired consistently for this class
		// See a more extensive explanation in the ItemsViewController.ViewWillLayoutSubviews method
		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			if (!_viewInitialized)
			{
				UpdateInitialPosition();

				_carouselView.PlatformInitialized();
				_viewInitialized = true;
			}
			UpdateVisualStates();
		}

		protected override bool IsHorizontal => (_carouselView?.ItemsLayout as ItemsLayout)?.Orientation == ItemsLayoutOrientation.Horizontal;

		protected override string DetermineCellReuseId()
		{
			if (_carouselView.ItemTemplate != null)
			{
				return CarouselTemplatedCell.ReuseId;
			}
			return base.DetermineCellReuseId();
		}

		protected override void RegisterViewTypes()
		{
			CollectionView.RegisterClassForCell(typeof(CarouselTemplatedCell), CarouselTemplatedCell.ReuseId);
			base.RegisterViewTypes();
		}

		protected override void BoundsSizeChanged()
		{
			base.BoundsSizeChanged();
			_carouselView.ScrollTo(_carouselView.Position, position: ScrollToPosition.Center, animate: false);
		}
		
		protected override IItemsViewSource CreateItemsViewSource()
		{
			var itemsSource = base.CreateItemsViewSource();
			SubscribeCollectionItemsSourceChanged(itemsSource);
			return itemsSource;
		}

		public override void UpdateItemsSource()
		{
			UnsubscribeCollectionItemsSourceChanged(ItemsSource);
			base.UpdateItemsSource();
			SubscribeCollectionItemsSourceChanged(ItemsSource);
		}

		void CollectionItemsSourceChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var centerItemIndex = -1;

			var indexPathsForVisibleItems = CollectionView.IndexPathsForVisibleItems.OrderBy(x => x.Row).ToList();

			if (indexPathsForVisibleItems.Count == 0)
				return;

			var collectionView = CollectionView;

			var contentInset = collectionView.ContentInset;
			var contentOffsetX = collectionView.ContentOffset.X + contentInset.Left;
			var contentOffsetY = collectionView.ContentOffset.Y + contentInset.Top;

			var firstVisibleItemIndex = (int)indexPathsForVisibleItems.First().Item;

			var centerPoint = new CGPoint(collectionView.Center.X + collectionView.ContentOffset.X, collectionView.Center.Y + collectionView.ContentOffset.Y);
			var centerIndexPath = collectionView.IndexPathForItemAtPoint(centerPoint);
			centerItemIndex = centerIndexPath?.Row ?? firstVisibleItemIndex;

			_carouselView.SetCurrentItem(null, centerItemIndex);
		}

		void SubscribeCollectionItemsSourceChanged(IItemsViewSource itemsSource)
		{
			if (itemsSource is ObservableItemsSource newItemsSource)
				newItemsSource.CollectionItemsSourceChanged += CollectionItemsSourceChanged;
		}

		void UnsubscribeCollectionItemsSourceChanged(IItemsViewSource oldItemsSource)
		{
			if (oldItemsSource is ObservableItemsSource oldObservableItemsSource)
				oldObservableItemsSource.CollectionItemsSourceChanged -= CollectionItemsSourceChanged;
		}

		internal void TearDown()
		{
			_carouselView.PropertyChanged -= CarouselViewPropertyChanged;
			UnsubscribeCollectionItemsSourceChanged(ItemsSource);
		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			_carouselView.SetIsDragging(true);
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			_carouselView.SetIsDragging(false);
		}

		internal void UpdateIsScrolling(bool isScrolling)
		{
			_carouselView.IsScrolling = isScrolling;
		}

		void UpdateInitialPosition()
		{
			if (_carouselView.CurrentItem != null)
			{
				int position = 0;

				var items = _carouselView.ItemsSource as IList;

				for (int n = 0; n < items?.Count; n++)
				{
					if (items[n] == _carouselView.CurrentItem)
					{
						position = n;
						break;
					}
				}

				var initialPosition = position;
				_carouselView.Position = initialPosition;
			}

			while (_carouselView.ScrollToActions.Count > 0)
			{
				var action = _carouselView.ScrollToActions.Dequeue();
				action();
			}
		}

		void CarouselViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.Is(CarouselView.PositionProperty))
				UpdateVisualStates();
		}

		void UpdateVisualStates()
		{
			var cells = CollectionView.VisibleCells;

			var newViews = new List<View>();

			var carouselPosition = _carouselView.Position;
			var previousPosition = carouselPosition - 1;
			var nextPosition = carouselPosition + 1;

			foreach (var cell in cells)
			{
				if (!((cell as CarouselTemplatedCell)?.VisualElementRenderer?.Element is View itemView))
					return;

				var item = itemView.BindingContext;
				var pos = GetPosition(item);

				if (pos == carouselPosition)
				{
					VisualStateManager.GoToState(itemView, CarouselView.CurrentItemVisualState);
				}
				else if (pos == previousPosition)
				{
					VisualStateManager.GoToState(itemView, CarouselView.PreviousItemVisualState);
				}
				else if (pos == nextPosition)
				{
					VisualStateManager.GoToState(itemView, CarouselView.NextItemVisualState);
				}
				else
				{
					VisualStateManager.GoToState(itemView, CarouselView.DefaultItemVisualState);
				}

				newViews.Add(itemView);

				if (!_carouselView.VisibleViews.Contains(itemView))
				{
					_carouselView.VisibleViews.Add(itemView);
				}
			}

			foreach (var itemView in _oldViews)
			{
				if (!newViews.Contains(itemView))
				{
					VisualStateManager.GoToState(itemView, CarouselView.DefaultItemVisualState);
					if (_carouselView.VisibleViews.Contains(itemView))
					{
						_carouselView.VisibleViews.Remove(itemView);
					}
				}
			}

			_oldViews = newViews;
		}

		int GetPosition(object item)
		{
			int position = 0;

			var items = _carouselView.ItemsSource as IList;

			for (int n = 0; n < items?.Count; n++)
			{
				if (items[n] == item)
				{
					position = n;
					break;
				}
			}

			return position;

		}
	}
}