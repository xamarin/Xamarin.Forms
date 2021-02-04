using Xamarin.Forms;
using UIKit;
using System.Threading.Tasks;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ProgressBarHandlerTests
	{
		UIProgressView GetNativeProgressBar(ProgressBarHandler progressBarHandler) =>
			(UIProgressView)progressBarHandler.View;

		double GetNativeProgress(ProgressBarHandler progressBarHandler) =>
			GetNativeProgressBar(progressBarHandler).Progress;

		async Task ValidateNativeProgressColor(IProgress progressBar, Color color)
		{
			var expected = await GetValueAsync(progressBar, handler => GetNativeProgressBar(handler).ProgressTintColor.ToColor());
			Assert.Equal(expected, color);
		}
	}
}