using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.DeviceTests.Stubs;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class LabelHandlerTests : HandlerTestBase<LabelHandler>
	{
		[Fact]
		public async Task BackgroundColorInitializesCorrectly()
		{
			var labelStub = new LabelStub()
			{
				BackgroundColor = Color.Blue,
				Text = "Test"
			};

			await ValidateNativeBackgroundColor(labelStub, Color.Blue);
		}

		[Fact]
		public async Task TextInitializesCorrectly()
		{
			var labelStub = new LabelStub()
			{
				Text = "Test"
			};

			await ValidatePropertyInitValue(labelStub, () => labelStub.Text, GetNativeText, labelStub.Text);
		}

		[Fact]
		public async Task TextColorInitializesCorrectly()
		{
			var labelStub = new LabelStub()
			{
				Text = "Test",
				TextColor = Color.Red
			};

			await ValidatePropertyInitValue(labelStub, () => labelStub.TextColor, GetNativeTextColor, labelStub.TextColor);
		}
	}
}