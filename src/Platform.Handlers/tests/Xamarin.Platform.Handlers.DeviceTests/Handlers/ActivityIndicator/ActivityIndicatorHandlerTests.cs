using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ActivityIndicatorHandlerTests : HandlerTestBase<ActivityIndicatorHandler>
	{
		[Fact]
		public async Task IsRunningInitializesCorrectly()
		{
			var activityIndicatorStub = new ActivityIndicatorStub()
			{
				IsRunning = true
			};

			await ValidatePropertyInitValue(activityIndicatorStub, () => activityIndicatorStub.IsRunning, GetNativeIsRunning, activityIndicatorStub.IsRunning);
		}
	}
}