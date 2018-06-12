using System;
using System.Collections;
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

			// TODO hartez 2018/06/10 14:29:22 Does ItemsSize need to be set at all when we're doing auto layout?
			// If not, can we leave it default/-1,-1 until UniformSize is set?	
			_layout.ItemSize = new CGSize(64, 64);
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			// We can't set this constraint up on ViewDidLoad, because Forms does other stuff that resizes the view
			// and we end up with massive layout errors. And View[Will/Did]Appear do not fire for this controller
			// reliably. So until one of those options is cleared up, we set this flag so that the initial constraints
			// are set up the first time this method is called.
			if (!_initialEstimateMade)
			{
				_layout.ConstrainTo(CollectionView.Bounds.Size);
				_initialEstimateMade = true;
			}
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			// TODO hartez 2018/06/07 17:06:18 Obviously this needs to handle things which are not ILists	
			return (_itemsSource as IList).Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell(_layout.CellReuseId, indexPath) as UICollectionViewCell;

			if (cell is DefaultCell defaultCell)
			{
				_layout.PrepareCellForLayout(defaultCell);
			
				if (_itemsSource is IList list)
				{
					defaultCell.Label.Text = list[indexPath.Row].ToString();
				}
			}

			return cell;
		}
	}
}