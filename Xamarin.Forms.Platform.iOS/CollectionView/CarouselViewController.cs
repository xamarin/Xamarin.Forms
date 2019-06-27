using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class CarouselViewController : ItemsViewController
	{
		CarouselView _carouselView;
		ItemsViewLayout _layout;
		nfloat _previousOffSetX;
		nfloat _previousOffSetY;
		object _currentItem;
		NSIndexPath _currentItemIdex;
		List<UICollectionViewCell> _cells;

		public CarouselViewController(CarouselView itemsView, ItemsViewLayout layout) : base(itemsView, layout)
		{
			_carouselView = itemsView;
			_carouselView.ScrollToRequested += ScrollToRequested;
			_layout = layout;
			Delegator.CarouselViewController = this;
			CollectionView.AllowsSelection = false;
			CollectionView.AllowsMultipleSelection = false;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{

			var cell = base.GetCell(collectionView, indexPath);

			var element = (cell as CarouselTemplatedCell)?.VisualElementRenderer?.Element;
			if (element != null)
				VisualStateManager.GoToState(element, CarouselView.DefaultItemVisualState);
			return cell;
		}



		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			UpdateIntialPosition();
			UpdateVisualStates();
		}

		protected override string DetermineCellReusedId()
		{
			if (_carouselView.ItemTemplate != null)
			{
				return CarouselTemplatedCell.ReuseId;
			}
			return base.DetermineCellReusedId();
		}

		protected override void RegisterViewTypes()
		{
			CollectionView.RegisterClassForCell(typeof(CarouselTemplatedCell), CarouselTemplatedCell.ReuseId);
		}

		internal void TeardDown()
		{
			_carouselView.ScrollToRequested -= ScrollToRequested;
		}

		public override void DecelerationStarted(UIScrollView scrollView)
		{

		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			_carouselView.SetIsDragging(true);
			UpdateVisualStates();
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			_carouselView.SetIsDragging(false);
			UpdateVisualStates();
		}

		public override void ScrollAnimationEnded(UIScrollView scrollView)
		{
			_carouselView.SetIsScrolling(false);
		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			var templatedCells = FindVisibleCells();

			//TODO: Improve storing this state here
			_currentItem = templatedCells.currentCell?.VisualElementRenderer?.Element?.BindingContext;
			_currentItemIdex = GetIndexForItem(_currentItem);

			if (_currentItem != null)
				_carouselView.SetCurrentItem(_currentItem);
		}

		public override void Scrolled(UIScrollView scrollView)
		{
			//TODO: How to handle the inertial values it would be easy for negative values but not for overscroll 
			var newOffSetX = scrollView.ContentOffset.X;
			var newOffSetY = scrollView.ContentOffset.Y;
			//TODO: rmarinho Handle RTL
			if (_layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
				_carouselView.SendScrolled(scrollView.ContentOffset.X, (_previousOffSetX > newOffSetX) ? ScrollDirection.Left : ScrollDirection.Right);
			else
				_carouselView.SendScrolled(scrollView.ContentOffset.Y, (_previousOffSetY > newOffSetY) ? ScrollDirection.Up : ScrollDirection.Down);

			UpdateVisualStateForOfScreenCell();

			UpdateVisualStates();

			_previousOffSetX = newOffSetX;
			_previousOffSetY = newOffSetY;
		}

		void UpdateVisualStateForOfScreenCell()
		{
			var newCells = CollectionView.VisibleCells.ToList();

			if (_cells != null)
			{
				foreach (var _oldCell in _cells)
				{
					if (!newCells.Contains(_oldCell))
					{
						var oldElement = (_oldCell as TemplatedCell)?.VisualElementRenderer?.Element;
						if (oldElement != null)
						{
							VisualStateManager.GoToState(oldElement, CarouselView.DefaultItemVisualState);
						}
					}

				}
			}

			_cells = newCells;
		}

		void ScrollToRequested(object sender, ScrollToRequestEventArgs e)
		{
			//We are ending dragging and position is being update
			if (e.Item == _currentItem || e.Index == _currentItemIdex.Row)
				return;
			_carouselView.SetIsScrolling(true);
		}

		void UpdateVisualStates()
		{
			var templatedCells = FindVisibleCells();

			if (templatedCells.previousCell != null)
			{
				var previousElement = templatedCells.previousCell.VisualElementRenderer?.Element;
				VisualStateManager.GoToState(previousElement, CarouselView.PreviousItemVisualState);
			}
			if (templatedCells.nextCell != null)
			{
				var nextElement = templatedCells.nextCell.VisualElementRenderer?.Element;
				VisualStateManager.GoToState(nextElement, CarouselView.NextItemVisualState);
			}
			if (templatedCells.currentCell != null)
			{
				var currentElement = templatedCells.currentCell.VisualElementRenderer?.Element;
				VisualStateManager.GoToState(currentElement, CarouselView.CurrentItemVisualState);
			}
		}

		void UpdateDefaultVisualState()
		{
			var cells = CollectionView.VisibleCells;
			for (int i = 0; i < cells.Count(); i++)
			{
				var cell = (cells[i] as TemplatedCell)?.VisualElementRenderer.Element;
				VisualStateManager.GoToState(cell, CarouselView.DefaultItemVisualState);
			}
		}

		(TemplatedCell currentCell, TemplatedCell previousCell, TemplatedCell nextCell) FindVisibleCells()
		{
			var cells = CollectionView.VisibleCells;


			TemplatedCell currentCell = null;
			TemplatedCell previousCell = null;
			TemplatedCell nextCell = null;

			var x = CollectionView.Center.X + CollectionView.ContentOffset.X;
			var y = CollectionView.Center.Y + CollectionView.ContentOffset.Y;

			var previousIndex = -1;
			var currentIndex = -1;
			var nextIndex = -1;
			for (int i = 0; i < cells.Count(); i++)
			{
				var cell = cells[i];
				if (cell.Center.X == x && cell.Center.Y == y)
				{
					currentIndex = i;
					if (i > 0)
					{
						previousIndex = currentIndex - 1;
					}
					if (i < cells.Count() - 1)
					{
						nextIndex = currentIndex + 1;
					}
				}
			}

			if (currentIndex != -1)
				currentCell = cells[currentIndex] as TemplatedCell;
			if (previousIndex != -1)
				previousCell = cells[previousIndex] as TemplatedCell;
			if (nextIndex != -1)
				nextCell = cells[nextIndex] as TemplatedCell;

			return (currentCell, previousCell, nextCell);
		}

		void UpdateIntialPosition()
		{
			if (_carouselView.Position != 0)
				_carouselView.ScrollTo(_carouselView.Position, -1, ScrollToPosition.Center);
		}
	}
}
