using UIKit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ActivityIndicatorHandlerTests
	{
		UIActivityIndicatorView GetNativeActivityIndicator(ActivityIndicatorHandler activityIndicatorHandler) =>
			(UIActivityIndicatorView)activityIndicatorHandler.View;

		bool GetNativeIsRunning(ActivityIndicatorHandler activityIndicatorHandler) =>
			GetNativeActivityIndicator(activityIndicatorHandler).IsAnimating;
	}
}