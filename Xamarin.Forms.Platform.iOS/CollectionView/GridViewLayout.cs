using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CoreGraphics;
using Foundation;
using UIKit;
using Debug = System.Diagnostics.Debug;

namespace Xamarin.Forms.Platform.iOS
{
	internal class GridViewLayout : ItemsViewLayout
	{
		readonly int _span;

		//static int _called;

		[Export("collectionView:layout:insetForSectionAtIndex:"), CompilerGenerated]
		public virtual UIEdgeInsets GetInsetForSection(UICollectionView collectionView, UICollectionViewLayout layout, int section)
		{
			return UIEdgeInsets.Zero;
		}

		[Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:"), CompilerGenerated]
		public virtual nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, int section)
		{
			return (nfloat)0.0;
		}

		[Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:"), CompilerGenerated]
		public virtual nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, int section)
		{
			return (nfloat)0.0;
		}

		//[Export("collectionView:layout:sizeForItemAtIndexPath:"), CompilerGenerated]
		//public virtual CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		//{
		//	Debug.WriteLine($">>>>> GridViewLayout GetSizeForItem 26: {_called++}");
			
		//	var x = ScrollDirection == UICollectionViewScrollDirection.Horizontal 
		//		? new CGSize(100, ConstrainedDimension)
		//		: new CGSize(ConstrainedDimension, 100);

		//	//Debug.WriteLine($">>>>> GridViewLayout GetSizeForItem 32: CGSize is {x}");

		//	return x;
		//}

		CGSize _defaultEstimate = new CGSize(200,200);
		bool _needEstimate = true;
		public bool HasUpdatedEstimate = false;
		public CGSize UpdatedEstimate;

		public GridViewLayout(UICollectionViewScrollDirection scrollDirection, int span)
		{
			_span = span;
			Initialize(scrollDirection);
			EstimatedItemSize = _defaultEstimate;
		}

		public override void ConstrainTo(CGSize size)
		{
			Debug.WriteLine($">>>>> GridViewLayout ConstrainT: size = {size}");

			ConstrainedDimension =
				ScrollDirection == UICollectionViewScrollDirection.Vertical ? size.Width / _span : size.Height / _span;

			Debug.WriteLine($">>>>> GridViewLayout ConstrainTo: ConstrainedDimension = {ConstrainedDimension}");
		}

		public override void PrepareCellForLayout(IConstrainedCell cell)
		{
			cell.SetConstrainedDimension(ConstrainedDimension);

			// TODO hartez 2018/09/10 19:13:10 This cast is not great, the text-only cells could probably benefit from estimate updates, too.	
			if (_needEstimate && cell is TemplatedCell templatedCell) 
			{
				var x = templatedCell.GetEstimate();
				Debug.WriteLine($">>>>> GridViewLayout PrepareCellForLayout: updated estimate: {x}");

				UpdatedEstimate = new CGSize(x.Width, x.Height);
				HasUpdatedEstimate = true;

				

				_needEstimate = false;
			}
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
		}

		
	}
}