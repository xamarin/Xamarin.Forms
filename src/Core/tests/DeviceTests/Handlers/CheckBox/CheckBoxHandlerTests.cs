using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class CheckBoxHandlerTests : HandlerTestBase<CheckBoxHandler>
	{
		[Theory(DisplayName = "[CheckBoxHandler] IsChecked Initializes Correctly")]
		[InlineData(true)]
		[InlineData(false)]
		public async Task IsCheckedInitializesCorrectly(bool isChecked)
		{
			var checkBoxStub = new CheckBoxStub()
			{
				IsChecked = isChecked
			};

			await ValidatePropertyInitValue(checkBoxStub, () => checkBoxStub.IsChecked, GetNativeIsChecked, checkBoxStub.IsChecked);
		}

		[Fact(DisplayName = "[CheckBoxHandler] Color Updates Correctly")]
		public async Task ColorUpdatesCorrectly()
		{
			var checkBoxStub = new CheckBoxStub()
			{
				Color = Color.Red,
				IsChecked = true
			};

			await ValidateColor(checkBoxStub, Color.Red, () => checkBoxStub.Color = Color.Red);
		}
	}
}