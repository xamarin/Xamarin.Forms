using Microsoft.Maui.Handlers;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category("LabelHandler")]
	public partial class LabelHandlerTests : HandlerTestBase<LabelHandler>
	{
		[Fact]
		public async Task BackgroundColorInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				BackgroundColor = Color.Blue,
				Text = "Test"
			};

			await ValidateNativeBackgroundColor(label, Color.Blue);
		}

		[Fact]
		public async Task TextInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test"
			};

			await ValidatePropertyInitValue(label, () => label.Text, GetNativeText, label.Text);
		}

		[Fact]
		public async Task TextColorInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test",
				TextColor = Color.Red
			};

			await ValidatePropertyInitValue(label, () => label.TextColor, GetNativeTextColor, label.TextColor);
		}

		[Fact(DisplayName = "MaxLines Initializes Correctly")]
		public async Task MaxLinesInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				MaxLines = 2
			};

			await ValidatePropertyInitValue(label, () => label.MaxLines, GetNativeMaxLines, label.MaxLines);
		}

		[Fact]
		public async Task MaxLinesDoesNotAffectLineBreakMode()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				LineBreakMode = LineBreakMode.NoWrap,
				MaxLines = 0,
			};

			var handler = await CreateHandlerAsync(label);
			var nativeLabel = GetNativeLabel(handler);

			Assert.Equal(1, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.NoWrap.ToNative(), GetNativeLineBreakMode(handler));

			label.MaxLines = 1;
			nativeLabel.UpdateMaxLines(label);

			Assert.Equal(1, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.NoWrap.ToNative(), GetNativeLineBreakMode(handler));
		}

		[Fact]
		public async Task SettingMaxLinesOutsideOfHandlerDoesNotAffectLineBreakMode()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				LineBreakMode = LineBreakMode.NoWrap,
				MaxLines = 0,
			};

			var handler = await CreateHandlerAsync(label);
			var nativeLabel = GetNativeLabel(handler);

			Assert.Equal(1, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.NoWrap.ToNative(), GetNativeLineBreakMode(handler));

			label.MaxLines = 5;
			nativeLabel.UpdateMaxLines(label);

			Assert.Equal(1, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.NoWrap.ToNative(), GetNativeLineBreakMode(handler));
		}

		[Fact]
		public async Task LineBreakModeDoesNotAffectMaxLines()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				MaxLines = 3,
				LineBreakMode = LineBreakMode.WordWrap,
			};

			var handler = await CreateHandlerAsync(label);
			var nativeLabel = GetNativeLabel(handler);

			Assert.Equal(3, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.WordWrap.ToNative(), GetNativeLineBreakMode(handler));

			label.LineBreakMode = LineBreakMode.CharacterWrap;
			nativeLabel.UpdateLineBreakMode(label);

			Assert.Equal(3, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.CharacterWrap.ToNative(), GetNativeLineBreakMode(handler));
		}

		[Fact]
		public async Task SingleLineBreakModeChangesMaxLines()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				MaxLines = 3,
				LineBreakMode = LineBreakMode.WordWrap,
			};

			var handler = await CreateHandlerAsync(label);
			var nativeLabel = GetNativeLabel(handler);

			Assert.Equal(3, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.WordWrap.ToNative(), GetNativeLineBreakMode(handler));

			label.LineBreakMode = LineBreakMode.HeadTruncation;
			nativeLabel.UpdateLineBreakMode(label);

			Assert.Equal(1, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.HeadTruncation.ToNative(), GetNativeLineBreakMode(handler));
		}

		[Fact]
		public async Task UnsettingSingleLineBreakModeResetsMaxLines()
		{
			var label = new LabelStub()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				MaxLines = 3,
				LineBreakMode = LineBreakMode.WordWrap,
			};

			var handler = await CreateHandlerAsync(label);
			var nativeLabel = GetNativeLabel(handler);

			Assert.Equal(3, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.WordWrap.ToNative(), GetNativeLineBreakMode(handler));

			label.LineBreakMode = LineBreakMode.HeadTruncation;
			nativeLabel.UpdateLineBreakMode(label);

			Assert.Equal(1, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.HeadTruncation.ToNative(), GetNativeLineBreakMode(handler));

			label.LineBreakMode = LineBreakMode.WordWrap;
			nativeLabel.UpdateLineBreakMode(label);

			Assert.Equal(3, GetNativeMaxLines(handler));
			Assert.Equal(LineBreakMode.WordWrap.ToNative(), GetNativeLineBreakMode(handler));
		}
	}
}