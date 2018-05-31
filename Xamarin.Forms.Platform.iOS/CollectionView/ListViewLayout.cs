using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	// TODO hartez 2018/05/31 11:50:26 Add a property for scroll direction	
	// TODO hartez 2018/05/31 11:50:43 Add a property for the fixed width/height and use it in ItemSize	
	internal class ListViewLayout : UICollectionViewFlowLayout
	{
		public ListViewLayout()
		{
			Initialize();
		}
		
		void Initialize()
		{
			EstimatedItemSize = new CGSize(200, 40);
			ScrollDirection = UICollectionViewScrollDirection.Vertical;
		}

		public override CGSize ItemSize {
			get
			{
				// TODO hartez 2018/05/30 12:32:16 This itemheight is very obviously not what we want	
				var x = new CGSize(200, 40);
				return x;
			}
			set { base.ItemSize = value; }
		}
	}
}