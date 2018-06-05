using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		public abstract string CellReuseId { get; }

		public abstract Type CellType { get; }

		public abstract void UpdateBounds(CGSize size);
	}

	internal class ListViewLayout : ItemsViewLayout
	{
		public ListViewLayout(UICollectionViewScrollDirection scrollDirection)
		{
			Initialize(scrollDirection);
		}

		public override string CellReuseId => ScrollDirection == UICollectionViewScrollDirection.Horizontal
			? DefaultHorizontalListCell.ReuseId
			: DefaultVerticalListCell.ReuseId;

		public override Type CellType => ScrollDirection == UICollectionViewScrollDirection.Horizontal
			? typeof(DefaultHorizontalListCell)
			: typeof(DefaultVerticalListCell);

		public override void UpdateBounds(CGSize size)
		{
			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal)
			{
				EstimatedItemSize = new CGSize(1, size.Height);
				return;
			}

			EstimatedItemSize = new CGSize(size.Width, 1);
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			ScrollDirection = scrollDirection;
		}
	}
}