using UIKit;
using Embedding.XF;
using Xamarin.Forms.Platform.iOS;

namespace Embedding.iOS
{
    public class ContainingViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Yellow;

            var tansparentController = new TransparentPage().CreateViewController();
            AddChildViewController(tansparentController);
            tansparentController.View.Frame = View.Bounds;
            View.AddSubview(tansparentController.View);
            tansparentController.DidMoveToParentViewController(this);
        }
    }
}