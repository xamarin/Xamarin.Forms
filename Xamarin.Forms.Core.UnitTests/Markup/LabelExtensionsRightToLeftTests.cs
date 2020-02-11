using NUnit.Framework;
using Xamarin.Forms.Markup.RightToLeft;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class LabelExtensionsRightToLeftTests : MarkupBaseTestFixture<Label>
	{
		[Test]
		public void TextLeft()
			=> TestPropertiesSet(l => l.TextLeft(), (Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.End));

		[Test]
		public void TextRight()
			=> TestPropertiesSet(l => l.TextRight(), (Label.HorizontalTextAlignmentProperty, TextAlignment.End, TextAlignment.Start));

		[Test]
		public void SupportDerivedFromLabel()
		{
			DerivedFromLabel _ =
				new DerivedFromLabel()
				.TextLeft()
				.TextRight();
		}

		class DerivedFromLabel : Label { }
	}
}