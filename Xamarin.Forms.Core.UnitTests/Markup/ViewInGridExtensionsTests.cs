using NUnit.Framework;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class ViewInGridExtensionsTests : MarkupBaseTestFixture<BoxView>
	{

		enum TestRow { First, Second }
		enum TestCol { First, Second }

		[Test]
		public void Row()
			=> TestPropertiesSet(b => b.Row(1), (Grid.RowProperty, 0, 1));

		[Test]
		public void RowWithSpan()
			=> TestPropertiesSet(
					b => b.Row(1, 2),
					(Grid.RowProperty, 0, 1),
					(Grid.RowSpanProperty, 1, 2));

		[Test]
		public void RowSpan()
			=> TestPropertiesSet(b => b.RowSpan(2), (Grid.RowSpanProperty, 1, 2));

		[Test]
		public void Col()
			=> TestPropertiesSet(b => b.Col(1), (Grid.ColumnProperty, 0, 1));

		[Test]
		public void ColWithSpan()
			=> TestPropertiesSet(
					b => b.Col(1, 2),
					(Grid.ColumnProperty, 0, 1),
					(Grid.ColumnSpanProperty, 1, 2));

		[Test]
		public void ColSpan()
			=> TestPropertiesSet(b => b.ColSpan(2), (Grid.ColumnSpanProperty, 1, 2));

		[Test]
		public void RowEnum()
			=> TestPropertiesSet(b => b.Row(TestRow.Second), (Grid.RowProperty, (int)TestRow.First, (int)TestRow.Second));

		[Test]
		public void RowWithLastRowEnum()
			=> TestPropertiesSet(
					b => b.Row(TestRow.First, TestRow.Second),
					(Grid.RowProperty, (int)TestRow.Second, (int)TestRow.First),
					(Grid.RowSpanProperty, 1, 2));

		[Test]
		public void ColEnum()
			=> TestPropertiesSet(b => b.Col(TestCol.Second), (Grid.ColumnProperty, (int)TestCol.First, (int)TestCol.Second));

		[Test]
		public void ColWithLastColEnum()
			=> TestPropertiesSet(
					b => b.Col(TestCol.First, TestCol.Second),
					(Grid.ColumnProperty, (int)TestCol.Second, (int)TestCol.First),
					(Grid.ColumnSpanProperty, 1, 2));
	}
}