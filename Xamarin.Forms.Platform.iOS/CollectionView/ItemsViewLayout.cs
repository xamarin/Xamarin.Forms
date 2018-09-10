using System;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class ItemsViewLayout : UICollectionViewFlowLayout, IUICollectionViewDelegateFlowLayout
	{
		public nfloat ConstrainedDimension { get; set; }

		public abstract void ConstrainTo(CGSize size);

		public abstract void PrepareCellForLayout(IConstrainedCell cell);
	}
}