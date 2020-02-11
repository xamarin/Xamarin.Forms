using NUnit.Framework;
using Xamarin.Forms.Markup.LeftToRight;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class LabelExtensionsLeftToRightTests : MarkupBaseTestFixture<Label>
	{
		[Test]
		public void TextLeft()
			=> TestPropertiesSet(l => l.TextLeft(), (Label.HorizontalTextAlignmentProperty, TextAlignment.End, TextAlignment.Start));

		[Test]
		public void TextRight()
			=> TestPropertiesSet(l => l.TextRight(), (Label.HorizontalTextAlignmentProperty, TextAlignment.Start, TextAlignment.End));

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