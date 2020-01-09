using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class ViewExtensionsTests : MarkupBaseTestFixture<BoxView>
	{
		[Test]
		public void Left()
			=> TestPropertySet(v => v.Left(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Start));

		[Test]
		public void CenterH()
			=> TestPropertySet(v => v.CenterH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Center));

		[Test]
		public void FillH()
			=> TestPropertySet(v => v.FillH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill));

		[Test]
		public void Right()
			=> TestPropertySet(v => v.Right(), (View.HorizontalOptionsProperty, LayoutOptions.Start, LayoutOptions.End));

		[Test]
		public void LeftExpand()
			=> TestPropertySet(v => v.LeftExpand(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.StartAndExpand));

		[Test]
		public void CenterExpandH()
			=> TestPropertySet(v => v.CenterExpandH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand));

		[Test]
		public void FillExpandH()
			=> TestPropertySet(v => v.FillExpandH(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand));

		[Test]
		public void RightExpand()
			=> TestPropertySet(v => v.RightExpand(), (View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.EndAndExpand));

		[Test]
		public void Top()
			=> TestPropertySet(v => v.Top(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Start));

		[Test]
		public void Bottom()
			=> TestPropertySet(v => v.Bottom(), (View.VerticalOptionsProperty, LayoutOptions.Start, LayoutOptions.End));

		[Test]
		public void CenterV()
			=> TestPropertySet(v => v.CenterV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Center));

		[Test]
		public void FillV()
			=> TestPropertySet(v => v.FillV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill));

		[Test]
		public void TopExpand()
			=> TestPropertySet(v => v.TopExpand(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.StartAndExpand));

		[Test]
		public void BottomExpand()
			=> TestPropertySet(v => v.BottomExpand(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.EndAndExpand));

		[Test]
		public void CenterExpandV()
			=> TestPropertySet(v => v.CenterExpandV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand));

		[Test]
		public void FillExpandV()
			=> TestPropertySet(v => v.FillExpandV(), (View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand));

		[Test]
		public void Center()
			=> TestPropertySet(
					v => v.Center(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Center),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Center));

		[Test]
		public void Fill()
			=> TestPropertySet(
					v => v.Fill(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.Fill)
					);

		[Test]
		public void CenterExpand()
			=> TestPropertySet(
					v => v.CenterExpand(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.CenterAndExpand));

		[Test]
		public void FillExpand()
			=> TestPropertySet(
					v => v.FillExpand(),
					(View.HorizontalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand),
					(View.VerticalOptionsProperty, LayoutOptions.End, LayoutOptions.FillAndExpand));

		[Test]
		public void MarginThickness()
			=> TestPropertySet(v => v.Margin(new Thickness(1)), (View.MarginProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void MarginUniform()
			=> TestPropertySet(v => v.Margin(1), (View.MarginProperty, new Thickness(0), new Thickness(1)));

		[Test]
		public void MarginHorizontalVertical()
			=> TestPropertySet(v => v.Margin(1, 2), (View.MarginProperty, new Thickness(0), new Thickness(1, 2)));

		[Test]
		public void Margins()
			=> TestPropertySet(v => v.Margins(left: 1, top: 2, right: 3, bottom: 4), (View.MarginProperty, new Thickness(0), new Thickness(1, 2, 3, 4)));

		[Test]
		public void SupportDerivedFromView()
		{
			DerivedFromView _ =
				new DerivedFromView()
				.Left()
				.CenterH()
				.FillH()
				.Right()
				.LeftExpand()
				.CenterExpandH()
				.FillExpandH()
				.RightExpand()
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