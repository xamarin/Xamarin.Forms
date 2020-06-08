using System.Threading.Tasks;
using NUnit.Framework;
using UIKit;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class HtmlLabelTests : PlatformTestFixture
	{
		[Test, Category("Text"), Category("Label"), Category("Color")]
		[Description("Label text color should apply in HTML mode")]
		public async Task LabelTextColorAppliesToHtml()
		{
			var label = new Label { TextColor = Color.Red, Text = "<p>Hello</p>", TextType = TextType.Html };
			var expected = Color.Red.ToUIColor();
			var actual = await GetControlProperty(label, uiLabel => uiLabel.TextColor);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, Category("Text"), Category("Label"), Category("Color")]
		[Description("Label background color should apply in HTML mode")]
		public async Task LabelBackgroundColorAppliesToHtml()
		{
			var label = new Label { BackgroundColor = Color.Red, Text = "<p>Hello</p>", TextType = TextType.Html };
			var expected = Color.Red.ToUIColor();
			var actual = await GetRendererProperty(label, r => r.NativeView.BackgroundColor);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, Category("Text"), Category("Label"), Category("Font")]
		[Description("Label Font should apply in HTML mode")]
		public async Task LabelFontAppliesToHtml()
		{
			var label = new Label { FontFamily = "MarkerFelt-Thin", FontSize = 24, FontAttributes = FontAttributes.Italic, 
				Text = "<p>Hello</p>", TextType = TextType.Html };
			var expectedFontFamily = label.FontFamily;
			var expectedFontSize = (System.nfloat)label.FontSize;
			
			var actualFont = await GetControlProperty(label, uiLabel => uiLabel.Font);

			Assert.That(actualFont.FontDescriptor.SymbolicTraits & UIFontDescriptorSymbolicTraits.Italic, Is.Not.Zero);
			Assert.That(actualFont.Name, Is.EqualTo(expectedFontFamily));
			Assert.That(actualFont.PointSize, Is.EqualTo(expectedFontSize));
		}
	}
}