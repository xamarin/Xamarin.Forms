using System;
using System.Collections;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class CollectionViewController : UICollectionViewController
	{
		readonly IEnumerable _itemsSource;

		public CollectionViewController(IEnumerable itemsSource, UICollectionViewLayout layout) : base(layout)
		{
			_itemsSource = itemsSource;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CollectionView.RegisterClassForCell (typeof(DefaultCell), DefaultCell.ReuseId);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return (_itemsSource as IList).Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// TODO hartez 2018/05/31 11:53:51 Find guidance on what this should do if it can't dequeue a cell	(Probably throw?)
			var cell = collectionView.DequeueReusableCell(DefaultCell.ReuseId, indexPath) as DefaultCell;

			if (_itemsSource is IList list)
			{
				cell.Label.Text = list[indexPath.Row].ToString();
			}

			return cell;
		}
	}
}