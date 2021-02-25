using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Android.Widget;
using Android.Text;
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

        int GetNativeMaxLines(LabelHandler labelHandler) =>
            GetNativeLabel(labelHandler).MaxLines;

        TextUtils.TruncateAt GetNativeEllipsize(LabelHandler labelHandler) =>
           GetNativeLabel(labelHandler).Ellipsize;

        [Fact(DisplayName = "[LabelHandler] LineBreakMode Initializes Correctly")]
        public async Task LineBreakModeInitializesCorrectly()
        {
            var xplatLineBreakMode = LineBreakMode.TailTruncation;

            var labelStub = new LabelStub()
            {
                LineBreakMode = xplatLineBreakMode
            };

            var expectedValue = TextUtils.TruncateAt.End;

            var values = await GetValueAsync(labelStub, (handler) =>
            {
                return new
                {
                    ViewValue = labelStub.LineBreakMode,
                    NativeViewValue = GetNativeEllipsize(handler)
                };
            });

            Assert.Equal(xplatLineBreakMode, values.ViewValue);
            Assert.Equal(expectedValue, values.NativeViewValue);
        }
    }
}