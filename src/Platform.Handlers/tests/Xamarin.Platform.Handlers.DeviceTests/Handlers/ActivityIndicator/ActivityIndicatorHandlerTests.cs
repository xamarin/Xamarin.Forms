using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ActivityIndicatorHandlerTests : HandlerTestBase<ActivityIndicatorHandler>
	{
		[Theory(DisplayName = "IsRunning Initializes Correctly")]
		[InlineData(true)]
		[InlineData(false)]
		public async Task IsRunningInitializesCorrectly(bool isRunning)
		{
			var activityIndicator = new ActivityIndicatorStub()
			{
				IsRunning = isRunning
			};

			await ValidatePropertyInitValue(activityIndicator, () => activityIndicator.IsRunning, GetNativeIsRunning, activityIndicator.IsRunning);
		}

		[Fact(DisplayName = "BackgroundColor Updates Correctly")]
		public async Task BackgroundColorUpdatesCorrectly()
		{
			var activityIndicator = new ActivityIndicatorStub()
			{
				BackgroundColor = Color.Yellow,
				IsRunning = true
			};

			await ValidateColor(activityIndicator, Color.Yellow, () => activityIndicator.BackgroundColor = Color.Yellow);
		}
	}
}