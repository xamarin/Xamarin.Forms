using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SwitchHandlerTests : HandlerTestBase<SwitchHandler>
	{
		[Fact(DisplayName= "Is Toggled Initializes Correctly")]
		public async Task IsToggledInitializesCorrectly()
		{
			var switchStub = new SwitchStub()
			{
				IsToggled = true
			};

			await ValidatePropertyInitValue(switchStub, () => switchStub.IsToggled, GetNativeIsChecked, switchStub.IsToggled);
		}

		[Fact(DisplayName = "On Color Initializes Correctly")]
		public async Task OnColorInitializesCorrectly()
		{
			var switchStub = new SwitchStub()
			{
				IsToggled = true,
				OnColor = Color.Red
			};

			await ValidateOnColor(switchStub, Color.Red);
		}

		[Fact(DisplayName = "ThumbColor Initializes Correctly")]
		public async Task ThumbColorInitializesCorrectly()
		{
			var switchStub = new SwitchStub()
			{
				IsToggled = true,
				ThumbColor = Color.Blue
			};

			await ValidateThumbColor(switchStub, Color.Blue);
		}
	}
}