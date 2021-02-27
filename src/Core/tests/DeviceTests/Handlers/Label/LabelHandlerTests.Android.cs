using System.Threading.Tasks;
using Android.Text;
using Android.Widget;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Handlers;
using Xunit;

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

		TextUtils.TruncateAt GetNativeLineBreakMode(LabelHandler labelHandler) =>
			GetNativeLabel(labelHandler).Ellipsize;

		[Fact]
		public async Task NegativeMaxValueWithWrapIsCorrect()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				MaxLines = -1,
				LineBreakMode = LineBreakMode.WordWrap,
			};

			var nativeValue = await GetValueAsync(label, GetNativeMaxLines);

			Assert.Equal(int.MaxValue, nativeValue);
		}
	}
}