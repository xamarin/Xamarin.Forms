using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class LayoutExtensionsTests : MarkupBaseTestFixture<ContentView>
	{
		[Test]
		public void PaddingThickness()
			=> TestPropertySet(l => l.Padding(new Thickness(1)), (Layout.PaddingProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void PaddingUniform()
			=> TestPropertySet(l => l.Padding(1), (Layout.PaddingProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void PaddingHorizontalVertical()
			=> TestPropertySet(l => l.Padding(1, 2), (Layout.PaddingProperty, new Thickness(0), new Thickness(1, 2)));

		[Test]
		public void Paddings()
			=> TestPropertySet(l => l.Paddings(left: 1, top: 2, right: 3, bottom: 4), (Layout.PaddingProperty, new Thickness(0), new Thickness(1, 2, 3, 4)));

		[Test]
		public void SupportDerivedFromLayout()
		{
			DerivedFromLayout _ =
				new DerivedFromLayout()
				.Padding(1)
				.Padding(1, 2)
				.Paddings(left: 1, top: 2, right: 3, bottom: 4);
		}

		class DerivedFromLayout : ContentView { }
	}
}