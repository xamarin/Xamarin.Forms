using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	// TODO hartez 2018/05/31 16:29:30 Implement dispose override	
	// TODO hartez 2018/05/30 08:58:42 This follows the same basic scheme as RecyclerView.Adapter; you should be able to reuse the same wrapper class for the IEnumerable	
	//// TODO hartez 2018/05/30 09:05:38 Think about whether this Controller and/or the new Adapter should be internal or public
	public class CollectionViewRenderer : ViewRenderer<CollectionView, UICollectionView>
	{
		CollectionViewController _collectionViewController;

		public override UIViewController ViewController => _collectionViewController;

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return Control.GetSizeRequest(widthConstraint, heightConstraint, 0, 0);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<CollectionView> e)
		{
			if (e.NewElement != null)
			{
				// TODO hartez 2018/05/31 15:01:19 Check for known layout types	
				// That check should be done in its own protected virtual method (see UWP implementation)

				// TODO hartez 2018/05/31 15:06:28 Set the scroll direction and item width/height	

				var layout = new ListViewLayout();
				_collectionViewController = new CollectionViewController(e.NewElement.ItemsSource, layout);
				SetNativeControl(_collectionViewController.CollectionView);

				_collectionViewController.CollectionView.BackgroundColor = UIColor.Clear;
			}

			base.OnElementChanged(e);
		}
	}
}