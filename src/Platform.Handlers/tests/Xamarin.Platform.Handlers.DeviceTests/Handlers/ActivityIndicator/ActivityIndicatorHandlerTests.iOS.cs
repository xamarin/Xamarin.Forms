using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ActivityIndicatorHandlerTests
	{
		UIActivityIndicatorView GetNativeActivityIndicator(ActivityIndicatorHandler activityIndicatorHandler) =>
			(UIActivityIndicatorView)activityIndicatorHandler.View;

		bool GetNativeIsRunning(ActivityIndicatorHandler activityIndicatorHandler) =>
			GetNativeActivityIndicator(activityIndicatorHandler).IsAnimating;

		async Task ValidateColor(IActivityIndicator activityIndicator, Color color, Action action = null)
		{
			var expected = await GetValueAsync(activityIndicator, handler =>
			{
				var native = GetNativeActivityIndicator(handler);
				action?.Invoke();
				return native.BackgroundColor.ToColor();
			});
			Assert.Equal(expected, color);
		}
	}
}