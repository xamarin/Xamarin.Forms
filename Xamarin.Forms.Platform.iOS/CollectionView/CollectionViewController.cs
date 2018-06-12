using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	// TODO hartez 2018/06/01 14:17:00 Implement Dispose override ?	
	// TODO hartez 2018/06/01 14:21:24 Add a method for updating the layout	
	internal class CollectionViewController : UICollectionViewController
	{
		readonly IEnumerable _itemsSource;
		readonly ItemsViewLayout _layout;
		bool _initialEstimateMade;

		public CollectionViewController(IEnumerable itemsSource, ItemsViewLayout layout) : base(layout)
		{
			_itemsSource = itemsSource;
			_layout = layout;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
			AutomaticallyAdjustsScrollViewInsets = false;
			CollectionView.RegisterClassForCell(_layout.CellType, _layout.CellReuseId);
			CollectionView.WeakDelegate = _layout;

			// TODO hartez 2018/06/10 14:29:22 Does ItemsSize need to be set at all when we're doing auto layout? If not, can we leave it default/-1,-1 until UniformSize is set?	
			_layout.ItemSize = new CGSize(64, 64);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			// TODO hartez 2018/06/10 17:31:32 Do we just need to do this the first time through?	
			// TODO hartez 2018/06/10 17:31:50 Now that we've got this firing, see if it all still works even if you get rid of the viewcontroller adjustments higher up the chain	
			if (!_initialEstimateMade)
			{
				_layout.UpdateItemSizeEstimate(CollectionView.Bounds.Size);

				_layout.ConstrainTo(CollectionView.Bounds.Size);

				_initialEstimateMade = true;
			}
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO hartez 2018/06/07 17:06:18 Obviously this needs to handle things which are not ILists	
			return (_itemsSource as IList).Count;
		}

		public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize(toSize, coordinator);

			_layout.ShrinkConstraints(toSize, CollectionView.VisibleCells);

			coordinator.AnimateAlongsideTransition(
				context => { },
				context =>
				{
					_layout.ForceConstraints(View.Bounds.Size, CollectionView.VisibleCells);
				});
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			// TODO hartez 2018/05/31 11:53:51 Find guidance on what this should do if it can't dequeue a cell	(Probably throw?)
			var cell = collectionView.DequeueReusableCell(_layout.CellReuseId, indexPath) as DefaultCell;
			cell.UpdateConstrainedDimension(_layout.ConstrainedDimension);

			if (_itemsSource is IList list)
			{
				cell.Label.Text = list[indexPath.Row].ToString();
			}

			return cell;
		}
	}
}