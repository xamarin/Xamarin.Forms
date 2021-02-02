using UIKit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ProgressBarHandlerTests
	{
		UIProgressView GetNativeProgressBar(ProgressBarHandler progressBarHandler) =>
			(UIProgressView)progressBarHandler.View;

		double GetNativeProgress(ProgressBarHandler progressBarHandler) =>
			GetNativeProgressBar(progressBarHandler).Progress;
	}
}