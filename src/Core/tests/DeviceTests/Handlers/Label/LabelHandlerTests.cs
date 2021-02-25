using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.DeviceTests.Stubs;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class LabelHandlerTests : HandlerTestBase<LabelHandler>
	{
		[Fact(DisplayName = "[LabelHandler] Background Color Initializes Correctly")]
		public async Task BackgroundColorInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				BackgroundColor = Color.Blue,
				Text = "Test"
			};

			await ValidateNativeBackgroundColor(label, Color.Blue);
		}

		[Fact(DisplayName = "[LabelHandler] Text Initializes Correctly")]
		public async Task TextInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test"
			};

			await ValidatePropertyInitValue(label, () => label.Text, GetNativeText, label.Text);
		}

		[Fact(DisplayName = "[LabelHandler] Text Color Initializes Correctly")]
		public async Task TextColorInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test",
				TextColor = Color.Red
			};

			await ValidatePropertyInitValue(label, () => label.TextColor, GetNativeTextColor, label.TextColor);
		}
	}
}