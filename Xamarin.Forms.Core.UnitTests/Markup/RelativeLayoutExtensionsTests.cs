using NUnit.Framework;
using Xamarin.Forms.Internals;
using UnitExpressionSearch = Xamarin.Forms.Core.UnitTests.RelativeLayoutTests.UnitExpressionSearch;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class RelativeLayoutExtensionsTests : MarkupBaseTestFixture<RelativeLayout>
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			ExpressionSearch.Default = new UnitExpressionSearch();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			ExpressionSearch.Default = new UnitExpressionSearch();
		}

		public RelativeLayoutExtensionsTests(bool withExperimentalFlag) : base(withExperimentalFlag) { }

		RelativeLayout Layout => Bindable;

		[Test]
		public void NoChildren() => AssertExperimental(() =>
		{
			Layout.Children();
			Assert.That(Layout.Children, Is.Empty);
		});

		[Test]
		public void AddUnconstrainedAndNullChildren() => AssertExperimental(() =>
		{
			Layout.Children(
				new Label { }.Assign(out Label child0).Unconstrained(),
				null,
				new Label { }.Assign(out Label child1).Unconstrained()
			);
			Assert.That(Layout.Children.Count, Is.EqualTo(2));
			Assert.That(object.ReferenceEquals(Layout.Children[0], child0), Is.True);
			Assert.That(object.ReferenceEquals(Layout.Children[1], child1), Is.True);
		});


		[Test]
		public void AddBoundsConstrainedChild() => AssertExperimental(() =>
		{
			Layout.IsPlatformEnabled = true;
			Layout.Children(
				new Label { IsPlatformEnabled = true } 
				.Assign(out Label child) 
				.Constrain(() => new Rectangle(30, 20, Layout.Height / 2, Layout.Height / 4))
			);
			Layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.That(child.Bounds, Is.EqualTo(new Rectangle(30, 20, 50, 25)));
			Assert.That(Layout.Children.Count, Is.EqualTo(1));
			Assert.That(object.ReferenceEquals(Layout.Children[0], child), Is.True);
		});

		[Test]
		public void AddExpressionConstrainedChild() => AssertExperimental(() =>
		{
			Layout.IsPlatformEnabled = true;
			Layout.Children(
				new Label { IsPlatformEnabled = true }
				.Assign(out Label child)
				.Constrain() .X      (() => 30)
							 .Y      (() => 20)
							 .Width  (() => Layout.Height / 2)
							 .Height (() => Layout.Height / 4)
							 
			);
			Layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.That(child.Bounds, Is.EqualTo(new Rectangle(30, 20, 50, 25)));
			Assert.That(Layout.Children.Count, Is.EqualTo(1));
			Assert.That(object.ReferenceEquals(Layout.Children[0], child), Is.True);
		});

		[Test]
		public void AddConstraintsConstrainedChildren() => AssertExperimental(() =>
		{
			Layout.IsPlatformEnabled = true;
			Layout.Children(
				new Label { IsPlatformEnabled = true }
				.Assign (out Label child0)
				.Constraints().X      (30)
							  .Y      (20)
							  .Width  (parent => parent.Height / 5)
							  .Height (parent => parent.Height / 10),

				new Label { IsPlatformEnabled = true }
				.Assign(out Label child1)
				.Constraints().X      (child0, (layout, view) => view.Bounds.Right + 10)
							  .Y      (child0, (layout, view) => view.Y)
							  .Width  (child0, (layout, view) => view.Width)
							  .Height (child0, (layout, view) => view.Height),

				new Label { IsPlatformEnabled = true }
				.Assign(out Label child2)
				.Constraints().X      (parent => parent.Height / 5)
							  .Y      (parent => parent.Height / 10)
							  .Width  (30)
							  .Height (20)
			);
			Layout.Layout(new Rectangle(0, 0, 100, 100));

			Assert.That(child0.Bounds, Is.EqualTo(new Rectangle(30, 20, 20, 10)));
			Assert.That(child1.Bounds, Is.EqualTo(new Rectangle(60, 20, 20, 10)));
			Assert.That(child2.Bounds, Is.EqualTo(new Rectangle(20, 10, 30, 20)));

			Assert.That(Layout.Children.Count, Is.EqualTo(3));
			Assert.That(object.ReferenceEquals(Layout.Children[0], child0), Is.True);
			Assert.That(object.ReferenceEquals(Layout.Children[1], child1), Is.True);
			Assert.That(object.ReferenceEquals(Layout.Children[2], child2), Is.True);
		});

		[Test]
		public void SupportDerived() => AssertExperimental(() =>
		{
			DerivedFromRelativeLayout _ =
				new DerivedFromRelativeLayout()
				.Children();
		});

		class DerivedFromRelativeLayout : RelativeLayout { }
	}
}