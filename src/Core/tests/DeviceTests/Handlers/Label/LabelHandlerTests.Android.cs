using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Android.Widget;
using Microsoft.Maui;
using Xunit;
using Microsoft.Maui.DeviceTests.Stubs;

namespace Microsoft.Maui.DeviceTests
{
	public partial class LabelHandlerTests
	{
		TextView GetNativeLabel(LabelHandler labelHandler) =>
			(TextView)labelHandler.View;

		string GetNativeText(LabelHandler labelHandler) =>
			GetNativeLabel(labelHandler).Text;

		Color GetNativeTextColor(LabelHandler labelHandler) =>
		   ((uint)GetNativeLabel(labelHandler).CurrentTextColor).ToColor();

		Task ValidateNativeBackgroundColor(ILabel label, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeLabel(CreateHandler(label)).AssertContainsColor(color);
			});
		}

        float GetNativeCharacterSpacing(LabelHandler labelHandler) =>
            GetNativeLabel(labelHandler).LetterSpacing;

        [Fact(DisplayName = "[LabelHandler] CharacterSpacing Initializes Correctly")]
        public async Task CharacterSpacingInitializesCorrectly()
        {
            var xplatCharacterSpacing = 4;

            var labelStub = new LabelStub()
            {
                Text = "Test CharacterSpacing",
                CharacterSpacing = xplatCharacterSpacing
            };

            float expectedValue = xplatCharacterSpacing * UnitExtensions.EmCoefficient;

            var values = await GetValueAsync(labelStub, (handler) =>
            {
                return new
                {
                    ViewValue = labelStub.CharacterSpacing,
                    NativeViewValue = GetNativeCharacterSpacing(handler)
                };
            });

            Assert.Equal(xplatCharacterSpacing, values.ViewValue);
            Assert.Equal(expectedValue, values.NativeViewValue);
        }
    }
}