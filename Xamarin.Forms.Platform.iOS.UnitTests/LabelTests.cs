using System.Threading.Tasks;
using NUnit.Framework;
using UIKit;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class LabelTests : PlatformTestFixture
	{
		[Test, Category("RTL"), Category("Label")]
		[Description("RTL should work on Label with FormattedText")]
		public async Task RTLLabelWithFormattedTextWorks()
		{
			var formattedString = new FormattedString();
			formattedString.Spans.Add(new Span { Text = "RTL formatted text" });
			var label = new Label { FormattedText = formattedString };
			label.FlowDirection = FlowDirection.RightToLeft;
			var expected = UITextAlignment.Right;
			var actual = await GetControlProperty(label, uiLabel => uiLabel.TextAlignment);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, Category("RTL"), Category("Label")]
		[Description("RTL should work on Label with LineHeight")]
		public async Task RTLLabelWithLineHeightWorks()
		{
			var label = new Label { Text = "RTL text with LineHeight" };
			label.FlowDirection = FlowDirection.RightToLeft;
			label.LineHeight = 3;
			var expected = UITextAlignment.Right;
			var actual = await GetControlProperty(label, uiLabel => uiLabel.TextAlignment);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}