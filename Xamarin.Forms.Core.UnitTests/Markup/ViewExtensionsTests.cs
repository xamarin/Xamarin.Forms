using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class ViewExtensionsTests : MarkupBaseTestFixture<BoxView>
	{
		[Test]
		public void Start()
			=> TestPropertiesSet(v => v.Start(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Start));

		[Test]
		public void CenterH()
			=> TestPropertiesSet(v => v.CenterH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Center));

		[Test]
		public void FillH()
			=> TestPropertiesSet(v => v.FillH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill));

		[Test]
		public void End()
			=> TestPropertiesSet(v => v.End(), (View.HorizontalOptionsProperty, LayoutOptions.Start, LayoutOptions.End));

		[Test]
		public void StartExpand()
			=> TestPropertiesSet(v => v.StartExpand(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.StartAndExpand));

		[Test]
		public void CenterExpandH()
			=> TestPropertiesSet(v => v.CenterExpandH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand));

		[Test]
		public void FillExpandH()
			=> TestPropertiesSet(v => v.FillExpandH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand));

		[Test]
		public void EndExpand()
			=> TestPropertiesSet(v => v.EndExpand(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.EndAndExpand));

		[Test]
		public void Top()
			=> TestPropertiesSet(v => v.Top(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Start));

		[Test]
		public void Bottom()
			=> TestPropertiesSet(v => v.Bottom(), (View.VerticalOptionsProperty, LayoutOptions.Start, LayoutOptions.End));

		[Test]
		public void CenterV()
			=> TestPropertiesSet(v => v.CenterV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Center));

		[Test]
		public void FillV()
			=> TestPropertiesSet(v => v.FillV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill));

		[Test]
		public void TopExpand()
			=> TestPropertiesSet(v => v.TopExpand(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.StartAndExpand));

		[Test]
		public void BottomExpand()
			=> TestPropertiesSet(v => v.BottomExpand(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.EndAndExpand));

		[Test]
		public void CenterExpandV()
			=> TestPropertiesSet(v => v.CenterExpandV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand));

		[Test]
		public void FillExpandV()
			=> TestPropertiesSet(v => v.FillExpandV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand));

		[Test]
		public void Center()
			=> TestPropertiesSet(
					v => v.Center(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Center),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Center));

		[Test]
		public void Fill()
			=> TestPropertiesSet(
					v => v.Fill(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill)
					);

		[Test]
		public void CenterExpand()
			=> TestPropertiesSet(
					v => v.CenterExpand(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand));

		[Test]
		public void FillExpand()
			=> TestPropertiesSet(
					v => v.FillExpand(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand));

		[Test]
		public void MarginThickness()
			=> TestPropertiesSet(v => v.Margin(new Thickness(1)), (View.MarginProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void MarginUniform()
			=> TestPropertiesSet(v => v.Margin(1), (View.MarginProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void MarginHorizontalVertical()
			=> TestPropertiesSet(v => v.Margin(1, 2), (View.MarginProperty, new Thickness(0), new Thickness(1, 2)));

		[Test]
		public void Margins()
			=> TestPropertiesSet(v => v.Margins(left: 1, top: 2, right: 3, bottom: 4), (View.MarginProperty, new Thickness(0), new Thickness(1, 2, 3, 4)));

		[Test]
		public void SupportDerivedFromView()
		{
			DerivedFromView _ =
				new DerivedFromView()
				.Start()
				.CenterH()
				.FillH()
				.End()
				.StartExpand()
				.CenterExpandH()
				.FillExpandH()
				.EndExpand()
				.Top()
				.Bottom()
				.CenterV()
				.FillV()
				.TopExpand()
				.BottomExpand()
				.CenterExpandV()
				.FillExpandV()
				.Center()
				.Fill()
				.CenterExpand()
				.FillExpand()
				.Margin(new Thickness(1))
				.Margin(1, 2)
				.Margins(left: 1, top: 2, right: 3, bottom: 4);
		}

		class DerivedFromView : BoxView { }
	}
}