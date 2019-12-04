using System.Collections;
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
			_oldViews = new List<View>();
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

				_viewInitialized = true;
			}
			UpdateCellStates();
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

		internal void TearDown()
		{
			_carouselView.PropertyChanged -= CarouselViewPropertyChanged;
			_carouselView = null;
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

			if (_carouselView.Position != 0)
				_carouselView.ScrollTo(_carouselView.Position, -1, ScrollToPosition.Center, false);
		}

		void CarouselViewPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.Is(CarouselView.PositionProperty))
				UpdateCellStates();
		}

		void UpdateCellStates()
		{
			var cells = CollectionView.VisibleCells;

			var newViews = new List<View>();

			foreach (var cell in cells)
			{
				var itemView = (cell as CarouselTemplatedCell)?.VisualElementRenderer?.Element as View;
				var item = itemView.BindingContext;
				var pos = CarouselView.GetPositionForItem(_carouselView, item);

				if (pos == _carouselView.Position)
				{
					VisualStateManager.GoToState(itemView, CarouselView.CurrentItemVisualState);
				}
				else
				{
					if (pos == _carouselView.Position - 1)
					{
						VisualStateManager.GoToState(itemView, CarouselView.PreviousItemVisualState);
					}
					else if (pos == _carouselView.Position + 1)
					{
						VisualStateManager.GoToState(itemView, CarouselView.NextItemVisualState);
					}
					else
					{
						VisualStateManager.GoToState(itemView, CarouselView.DefaultItemVisualState);
					}
				}
				newViews.Add(itemView);
			}

			foreach (var item in _oldViews)
			{
				if (!newViews.Contains(item))
				{
					VisualStateManager.GoToState(item, CarouselView.DefaultItemVisualState);
				}
			}

			_oldViews = newViews;
		}
	}
}