using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Platform.Handlers.DeviceTests.Stubs;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class PickerHandlerTests : HandlerTestBase<PickerHandler>
	{
		[Fact(DisplayName = "[PickerHandler] Title Initializes Correctly")]
		public async Task TitleInitializesCorrectly()
		{
			var pickerStub = new PickerStub
			{
				Title = "Select an Item"
			};

			await ValidatePropertyInitValue(pickerStub, () => pickerStub.Title, GetNativeTitle, pickerStub.Title);
		}

		[Fact(DisplayName = "[PickerHandler] Title Color Initializes Correctly")]
		public async Task TitleColorInitializesCorrectly()
		{
			var pickerStub = new PickerStub
			{
				Title = "Select an Item",
				TitleColor = Color.CadetBlue
			};

			await ValidatePropertyInitValue(pickerStub, () => pickerStub.TitleColor, GetNativeTitleColor, pickerStub.TitleColor);
		}
	}
}