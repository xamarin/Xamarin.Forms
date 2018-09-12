using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class GridViewLayout : ItemsViewLayout
	{
		readonly int _span;

		// TODO hartez 2018/09/12 12:59:42 Take the GridItemsLayout so you can watch for span changes	
		public GridViewLayout(UICollectionViewScrollDirection scrollDirection, int span)
		{
			_span = span;
			Initialize(scrollDirection);
		}

		public override void ConstrainTo(CGSize size)
		{
			ConstrainedDimension =
				ScrollDirection == UICollectionViewScrollDirection.Vertical ? size.Width / _span : size.Height / _span;
		}

		public override void PrepareCellForLayout(IConstrainedCell cell)
		{
			if (EstimatedItemSize == CGSize.Empty)
			{
				cell.Constrain(ItemSize);
			}
			else
			{
				cell.Constrain(ConstrainedDimension);
			}
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
		}
	}
}