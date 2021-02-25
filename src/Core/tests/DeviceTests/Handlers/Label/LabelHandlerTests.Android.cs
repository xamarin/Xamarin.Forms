using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Android.Views;
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

		GravityFlags GetNativeGravity(LabelHandler labelHandler) =>
			GetNativeLabel(labelHandler).Gravity;

		Android.Views.TextAlignment GetNativeTextAlignment(LabelHandler labelHandler) =>
			GetNativeLabel(labelHandler).TextAlignment;

		[Fact(DisplayName = "[LabelHandler] HorizontalTextAlignment Updates Correctly")]
		public async Task HorizontalTextAlignmentInitializesCorrectly()
		{
			var xplatHorizontalTextAlignment = TextAlignment.End;

			var labelStub = new LabelStub()
			{
				Text = "Test",
				HorizontalTextAlignment = xplatHorizontalTextAlignment
			};

			Android.Views.TextAlignment expectedValue = Android.Views.TextAlignment.ViewEnd;

			var values = await GetValueAsync(labelStub, (handler) =>
			{
				return new
				{
					ViewValue = labelStub.HorizontalTextAlignment,
					NativeViewValue = GetNativeTextAlignment(handler)
				};
			});

			Assert.Equal(xplatHorizontalTextAlignment, values.ViewValue);
			Assert.True(values.NativeViewValue == expectedValue);
		}

		[Fact(DisplayName = "[LabelHandler] VerticalTextAlignment Updates Correctly")]
		public async Task VerticalTextAlignmentInitializesCorrectly()
		{
			var xplatVerticalTextAlignment = TextAlignment.End;

			var labelStub = new LabelStub()
			{
				Text = "Test",
				VerticalTextAlignment = xplatVerticalTextAlignment
			};

			GravityFlags expectedValue = GravityFlags.Bottom;

			var values = await GetValueAsync(labelStub, (handler) =>
			{
				return new
				{
					ViewValue = labelStub.VerticalTextAlignment,
					NativeViewValue = GetNativeGravity(handler)
				};
			});

			Assert.Equal(xplatVerticalTextAlignment, values.ViewValue);
			Assert.True(values.NativeViewValue.HasFlag(expectedValue));
		}
	}
}