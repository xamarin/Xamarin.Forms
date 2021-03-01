using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class EntryHandlerTests : HandlerTestBase<EntryHandler>
	{
		[Fact(DisplayName = "[EntryHandler] Is Text Initializes Correctly")]
		public async Task TextInitializesCorrectly()
		{
			var entry = new EntryStub()
			{
				Text = "Test"
			};

			await ValidatePropertyInitValue(entry, () => entry.Text, GetNativeText, entry.Text);
		}

		[Fact(DisplayName = "[EntryHandler] Is TextColor Initializes Correctly")]
		public async Task TextColorInitializesCorrectly()
		{
			var entry = new EntryStub()
			{
				Text = "Test",
				TextColor = Color.Yellow
			};

			await ValidatePropertyInitValue(entry, () => entry.TextColor, GetNativeTextColor, entry.TextColor);
		}

		[Fact(DisplayName = "[EntryHandler] IsPassword Initializes Correctly")]
		public async Task IsPasswordInitializesCorrectly()
		{
			var entry = new EntryStub()
			{
				IsPassword = true
			};

			await ValidatePropertyInitValue(entry, () => entry.IsPassword, GetNativeIsPassword, entry.IsPassword);
		}
	}
}
