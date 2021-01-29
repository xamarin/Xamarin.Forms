using CoreGraphics;
using UIKit;

namespace Xamarin.Platform.Handlers
{
	public partial class ActivityIndicatorHandler : AbstractViewHandler<IActivityIndicator, NativeActivityIndicator>
	{
		protected override NativeActivityIndicator CreateNativeView() => new NativeActivityIndicator(CGRect.Empty, VirtualView)
		{
			ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray
		};
	}
}