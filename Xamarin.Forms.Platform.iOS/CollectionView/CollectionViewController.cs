using System;
using System.Collections;
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

		public CollectionViewController(IEnumerable itemsSource, ItemsViewLayout layout) : base(layout)
		{
			_itemsSource = itemsSource;
			_layout = layout;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CollectionView.RegisterClassForCell (_layout.CellType, _layout.CellReuseId);
			CollectionView.WeakDelegate = _layout;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return (_itemsSource as IList).Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// TODO hartez 2018/05/31 11:53:51 Find guidance on what this should do if it can't dequeue a cell	(Probably throw?)
			var cell = collectionView.DequeueReusableCell(_layout.CellReuseId, indexPath) as DefaultCell;

			if (_itemsSource is IList list)
			{
				cell.Label.Text = list[indexPath.Row].ToString();
			}

			return cell;
		}
	}
}