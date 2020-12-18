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
		public async Task LabelFormattedTextRTLWorks()
		{
			var formattedString = new FormattedString();
			formattedString.Spans.Add(new Span { Text = "RTL formatted text" });
			var label = new Label { FormattedText = formattedString };
			label.FlowDirection = FlowDirection.RightToLeft;
			var expected = UITextAlignment.Right;
			var actual = await GetControlProperty(label, uiLabel => uiLabel.TextAlignment);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}