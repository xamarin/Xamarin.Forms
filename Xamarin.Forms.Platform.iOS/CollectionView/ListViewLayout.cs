using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		public abstract string CellReuseId { get; }

		public abstract Type CellType { get; }

		public abstract void UpdateBounds(CGRect bounds);
	}

	internal class ListViewLayout : ItemsViewLayout
	{
		public ListViewLayout(UICollectionViewScrollDirection scrollDirection)
		{
			Initialize(scrollDirection);
		}

		//[Export("collectionView:layout:sizeForItemAtIndexPath:"), CompilerGenerated]
		//public virtual CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		//{
		//	Debug.WriteLine($">>>>> sizeForItemAtIndexPath: indexPath.Row is {indexPath.Row}");
		//	return ScrollDirection == UICollectionViewScrollDirection.Horizontal
		//				? new CGSize(40, CollectionView.Bounds.Height)
		//				: new CGSize(CollectionView.Bounds.Width, 40);
		//}

		//public override CGSize ItemSize
		//{
		//	get
		//	{
		//		// TODO hartez 2018/06/01 09:53:24 Can't use this set item size forever	

		//		return ScrollDirection == UICollectionViewScrollDirection.Horizontal
		//			? new CGSize(40, _getConstrainedDimension())
		//			: new CGSize(_getConstrainedDimension(), 40);
		//	}
		//	set => base.ItemSize = value;
		//}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
			
			//EstimatedItemSize = ScrollDirection == UICollectionViewScrollDirection.Horizontal
			//	? new CGSize(40, 500)
			//	: new CGSize(500, 40);

			//EstimatedItemSize = new CGSize(20, 20);
			EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
		}

		public override string CellReuseId
		{
			get
			{
				if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
				{
					return DefaultHorizontalListCell.ReuseId;
				}

				return DefaultVerticalListCell.ReuseId;
			}
		}

		public override Type CellType
		{
			get
			{
				if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
				{
					return typeof(DefaultHorizontalListCell);
				}

				return typeof(DefaultVerticalListCell);
			}
		}

		public override void UpdateBounds(CGRect bounds)
		{
			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				EstimatedItemSize = new CGSize(1, bounds.Height);
			}

			EstimatedItemSize = new CGSize(bounds.Width, 1);
		}
	}
}