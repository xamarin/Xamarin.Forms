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

		[Fact(DisplayName = "Track Color Initializes Correctly")]
		public async Task TrackColorInitializesCorrectly()
		{
			var switchStub = new SwitchStub()
			{
				IsToggled = true,
				TrackColor = Color.Red
			};

			await ValidateTrackColor(switchStub, Color.Red);
		}

		[Fact(DisplayName = "Track Color Updates Correctly")]
		public async Task TrackColorUpdatesCorrectly()
		{
			var switchStub = new SwitchStub()
			{
				IsToggled = true
			};

			await ValidateTrackColor(switchStub, Color.Red, () => switchStub.TrackColor = Color.Red);
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

		[Fact(DisplayName = "Track Color Updates Correctly")]
		public async Task ThumbColorUpdatesCorrectly()
		{
			var switchStub = new SwitchStub()
			{
				IsToggled = true
			};

			await ValidateTrackColor(switchStub, Color.Red, () => switchStub.ThumbColor = Color.Red);
		}
	}
}