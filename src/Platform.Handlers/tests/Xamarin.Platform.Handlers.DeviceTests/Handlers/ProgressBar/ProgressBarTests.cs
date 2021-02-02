using System.Threading.Tasks;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ProgressBarHandlerTests : HandlerTestBase<ProgressBarHandler>
	{
		[Fact]
		public async Task ProgressInitializesCorrectly()
		{
			var progressBar = new ProgressBarStub()
			{
				Progress = 0.5
			};

			await ValidatePropertyInitValue(progressBar, () => progressBar.Progress, GetNativeProgress, progressBar.Progress);
		}
	}
}