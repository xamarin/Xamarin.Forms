using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class BoxViewHandlerTests : HandlerTestBase<BoxViewHandler>
	{
		[Fact]
		public async Task CoolorRadiusInitializesCorrectly()
		{
			var boxViewStub = new BoxViewStub()
			{
				Color = Color.Red
			};

			await ValidatePropertyInitValue(boxViewStub, () => boxViewStub.Color, GetNativeColor, boxViewStub.Color);
		}

		[Fact]
		public async Task CornerRadiusInitializesCorrectly()
		{
			var boxViewStub = new BoxViewStub()
			{
				CornerRadius = new Forms.CornerRadius(12, 0, 0, 24)
			};

			await ValidatePropertyInitValue(boxViewStub, () => boxViewStub.CornerRadius, GetNativeCornerRadius, boxViewStub.CornerRadius);
		}
	}
}