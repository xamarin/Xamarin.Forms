using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using UIKit;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class ButtonHandlerTests
	{
		UIButton GetNativeButton(ButtonHandler buttonHandler) =>
			(UIButton)buttonHandler.View;

		string GetNativeText(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).CurrentTitle;

		Color GetNativeTextColor(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).CurrentTitleColor.ToColor();

		Task PerformClick(IButton button)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeButton(CreateHandler(button)).SendActionForControlEvents(UIControlEvent.TouchUpInside);
			});
		}

		float GetNativeCornerRadius(ButtonHandler buttonHandler) =>
			(float)GetNativeButton(buttonHandler).Layer.CornerRadius;

		[Fact(DisplayName = "[ButtonHandler] CornerRadius Initializes Correctly")]
		public async Task ValueInitializesCorrectly()
		{
			var xplatCornerRadius = 12;

			var buttonStub = new ButtonStub()
			{
				BackgroundColor = Color.Red,
				CornerRadius = xplatCornerRadius,
				Text = "Test"
			};

			int expectedValue = xplatCornerRadius;

			var values = await GetValueAsync(buttonStub, (handler) =>
			{
				return new
				{
					ViewValue = buttonStub.CornerRadius,
					NativeViewValue = GetNativeCornerRadius(handler)
				};
			});

			Assert.Equal(xplatCornerRadius, values.ViewValue);
			Assert.Equal(expectedValue, values.NativeViewValue);
		}
	}
}