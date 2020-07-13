using System;
using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class RelativeLayoutTests : MarkupBaseTestFixture<RelativeLayout>
	{
		public RelativeLayoutTests(bool withExperimentalFlag) : base(withExperimentalFlag) { }

		RelativeLayout Layout => Bindable;

		[Test]
		public void NoChildren() => AssertExperimental(() =>
		{
			Layout.Children();
			Assert.That(Layout.Children, Is.Empty);
		});

		Label child0, child1;

		[Test]
		public void AddUnconstrainedAndNullChildren() => AssertExperimental(() =>
		{
			Layout.Children(
				new Label { }.Assign(out child0).Unconstrained(),
				null,
				new Label { }.Assign(out child1).Unconstrained()
			);
			Assert.That(Layout.Children.Count, Is.EqualTo(2));
			Assert.That(object.ReferenceEquals(Layout.Children[0], child0), Is.True);
			Assert.That(object.ReferenceEquals(Layout.Children[1], child1), Is.True);
		});

		// TODO: Code method to verify constraints
		// Expressions: let them set bool iscalled variables, enforce that they are called
		// Bounds: is also an expression, same
		// Constraints: check object instance with GetXConstraint

		[Test]
		public void SupportDerived() => AssertExperimental(() =>
		{
			DerivedFromRelativeLayout _ =
				new DerivedFromRelativeLayout()
				.Children()
				; // TODO: call all extensions
		});

		class DerivedFromRelativeLayout : RelativeLayout { }
	}
}