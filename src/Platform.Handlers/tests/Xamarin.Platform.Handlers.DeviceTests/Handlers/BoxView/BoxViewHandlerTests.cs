using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class BoxViewHandlerTests : HandlerTestBase<BoxViewHandler>
	{
		[Fact]
		public async Task ColorInitializesCorrectly()
		{
			var boxView = new BoxViewStub()
			{
				Color = Color.Red
			};

			await ValidatePropertyInitValue(boxView, () => boxView.Color, GetNativeColor, boxView.Color);
		}

		[Fact]
		public async Task CornerRadiusInitializesCorrectly()
		{
			var boxView = new BoxViewStub()
			{
				CornerRadius = new CornerRadius(12, 0, 0, 24)
			};

			await ValidatePropertyInitValue(boxView, () => boxView.CornerRadius, GetNativeCornerRadius, boxView.CornerRadius);
		}
	}
}