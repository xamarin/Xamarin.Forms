using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(typeof(Button), true)]
	[TestFixture(typeof(Button), false)]
	[TestFixture(typeof(Frame), true)]
	[TestFixture(typeof(Frame), false)]
	[TestFixture(typeof(ImageButton), true)]
	[TestFixture(typeof(ImageButton), false)]
	[TestFixture(typeof(Label), true)]
	[TestFixture(typeof(Label), false)]
	[TestFixture(typeof(Page), true)]
	[TestFixture(typeof(Page), false)]
	public class PaddingElementExtensionsTests<TPaddingElement> : MarkupBaseTestFixture<TPaddingElement> where TPaddingElement : Element, IPaddingElement, new()
	{
		public PaddingElementExtensionsTests(bool withExperimentalFlag) : base(withExperimentalFlag) { }

		[Test]
		public void PaddingThickness()
			=> TestPropertiesSet(l => l.Padding(new Thickness(1)), (PaddingElement.PaddingProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void PaddingUniform()
			=> TestPropertiesSet(l => l.Padding(1), (PaddingElement.PaddingProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void PaddingHorizontalVertical()
			=> TestPropertiesSet(l => l.Padding(1, 2), (PaddingElement.PaddingProperty, new Thickness(0), new Thickness(1, 2)));

		[Test]
		public void Paddings()
			=> TestPropertiesSet(l => l.Paddings(left: 1, top: 2, right: 3, bottom: 4), (PaddingElement.PaddingProperty, new Thickness(0), new Thickness(1, 2, 3, 4)));

		[Test]
		public void SupportDerivedFrom() => AssertExperimental(() =>
		{
			DerivedFrom _ =
				new DerivedFrom()
				.Padding(1)
				.Padding(1, 2)
				.Paddings(left: 1, top: 2, right: 3, bottom: 4);
		});

		class DerivedFrom : ContentView { }
	}
}