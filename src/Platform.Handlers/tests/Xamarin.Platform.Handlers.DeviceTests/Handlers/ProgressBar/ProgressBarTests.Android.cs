using System.Threading.Tasks;
using Xamarin.Forms;
using AProgressBar = Android.Widget.ProgressBar;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ProgressBarHandlerTests
	{
		AProgressBar GetNativeProgressBar(ProgressBarHandler progressBarHandler) =>
			(AProgressBar)progressBarHandler.View;

		double GetNativeProgress(ProgressBarHandler progressBarHandler) =>
			GetNativeProgressBar(progressBarHandler).Progress;

		Task ValidateNativeProgressColor(IProgress progressBar, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeProgressBar(CreateHandler(progressBar)).AssertContainsColor(color);
			});
		}
	}
}