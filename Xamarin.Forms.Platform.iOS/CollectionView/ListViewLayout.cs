using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	internal class ListViewLayout : UICollectionViewFlowLayout
	{
		readonly Func<nfloat> _getConstrainedDimension;

		public ListViewLayout(UICollectionViewScrollDirection scrollDirection, Func<nfloat> getConstrainedDimension)
		{
			_getConstrainedDimension = getConstrainedDimension;
			Initialize(scrollDirection);
		}

		public override CGSize ItemSize
		{
			get
			{
				// TODO hartez 2018/06/01 09:53:24 Can't use this set item size forever	

				Debug.WriteLine($">>>>> ListViewLayout ItemSize 22: _getConstrainedDimension = {_getConstrainedDimension()}");

				return ScrollDirection == UICollectionViewScrollDirection.Horizontal
					? new CGSize(40, _getConstrainedDimension())
					: new CGSize(_getConstrainedDimension(), 40);
			}
			set { base.ItemSize = value; }
		}

		void Initialize(UICollectionViewScrollDirection scrollDirection)
		{
			//EstimatedItemSize = scrollDirection == UICollectionViewScrollDirection.Horizontal
			//	? new CGSize(40, _constrainedDimension)
			//	: new CGSize(_constrainedDimension, 40);

			ScrollDirection = scrollDirection;
		}

		
	}
}