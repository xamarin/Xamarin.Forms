using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Android.Widget;
using Android.Graphics;
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

        PaintFlags GetNativeTextDecorations(LabelHandler labelHandler) =>
            GetNativeLabel(labelHandler).PaintFlags;

        [Fact(DisplayName = "[LabelHandler] TextDecorations Initializes Correctly")]
        public async Task TextDecorationsInitializesCorrectly()
        {
            var xplatTextDecorations = TextDecorations.Underline;

            var labelHandler = new LabelStub()
            {
                TextDecorations = xplatTextDecorations
            };

            var values = await GetValueAsync(labelHandler, (handler) =>
            {
                return new
                {
                    ViewValue = labelHandler.TextDecorations,
                    NativeViewValue = GetNativeTextDecorations(handler)
                };
            });

            PaintFlags expectedValue = PaintFlags.UnderlineText;

            Assert.Equal(xplatTextDecorations, values.ViewValue);
            Assert.True(values.NativeViewValue.HasFlag(expectedValue));
        }
    }
}