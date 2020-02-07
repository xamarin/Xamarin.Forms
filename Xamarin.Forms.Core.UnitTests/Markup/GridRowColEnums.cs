using System;
using NUnit.Framework;
using static Xamarin.Forms.Markup.GridRowCol;

namespace Xamarin.Forms.Markup.UnitTests
{
	[TestFixture]
	public class GridRowCol : MarkupBaseTestFixture
	{
		enum Row { First, Second, Third }
		enum Col { First, Second, Third, Fourth }

		[Test]
		public void DefineRowsWithoutEnums()
		{
			var grid = new Forms.Grid
			{
				RowDefinitions = Rows.Define(Auto, Star, 20)
			};

			Assert.That(grid.RowDefinitions.Count, Is.EqualTo(3));
			Assert.That(grid.RowDefinitions[0]?.Height, Is.EqualTo(GridLength.Auto));
			Assert.That(grid.RowDefinitions[1]?.Height, Is.EqualTo(GridLength.Star));
			Assert.That(grid.RowDefinitions[2]?.Height, Is.EqualTo(new GridLength(20)));
		}

		[Test]
		public void DefineRowsWithEnums()
		{
			var grid = new Forms.Grid
			{
				RowDefinitions = Rows.Define(
					(Row.First, Auto),
					(Row.Second, Star),
					(Row.Third, 20)
				)
			};

			Assert.That(grid.RowDefinitions.Count, Is.EqualTo(3));
			Assert.That(grid.RowDefinitions[0]?.Height, Is.EqualTo(GridLength.Auto));
			Assert.That(grid.RowDefinitions[1]?.Height, Is.EqualTo(GridLength.Star));
			Assert.That(grid.RowDefinitions[2]?.Height, Is.EqualTo(new GridLength(20)));
		}

		[Test]
		public void InvalidRowEnumOrder()
		{
			Assert.Throws<ArgumentException>(
				() => Rows.Define((Row.First, 8), (Row.Third, 8)),
				$"Value of row name Third is not 1. " +
				"Rows must be defined with enum names whose values form the sequence 0,1,2,..."
			);
		}

		[Test]
		public void DefineColumnsWithoutEnums()
		{
			var grid = new Forms.Grid
			{
				ColumnDefinitions = Columns.Define(Auto, Star, 20, 40)
			};

			Assert.That(grid.ColumnDefinitions.Count, Is.EqualTo(4));
			Assert.That(grid.ColumnDefinitions[0]?.Width, Is.EqualTo(GridLength.Auto));
			Assert.That(grid.ColumnDefinitions[1]?.Width, Is.EqualTo(GridLength.Star));
			Assert.That(grid.ColumnDefinitions[2]?.Width, Is.EqualTo(new GridLength(20)));
			Assert.That(grid.ColumnDefinitions[3]?.Width, Is.EqualTo(new GridLength(40)));
		}

		[Test]
		public void DefineColumnsWithEnums()
		{
			var grid = new Forms.Grid
			{
				ColumnDefinitions = Columns.Define(
					(Col.First, Auto),
					(Col.Second, Star),
					(Col.Third, 20),
					(Col.Fourth, 40)
				)
			};

			Assert.That(grid.ColumnDefinitions.Count, Is.EqualTo(4));
			Assert.That(grid.ColumnDefinitions[0]?.Width, Is.EqualTo(GridLength.Auto));
			Assert.That(grid.ColumnDefinitions[1]?.Width, Is.EqualTo(GridLength.Star));
			Assert.That(grid.ColumnDefinitions[2]?.Width, Is.EqualTo(new GridLength(20)));
			Assert.That(grid.ColumnDefinitions[3]?.Width, Is.EqualTo(new GridLength(40)));
		}

		[Test]
		public void InvalidColumnEnumOrder()
		{
			Assert.Throws<ArgumentException>(
				() => Columns.Define((Col.Second, 8), (Col.First, 8)),
				$"Value of column name Second is not 0. " +
				"Columns must be defined with enum names whose values form the sequence 0,1,2,..."
			);
		}

		[Test]
		public void AllColumns()
			=> Assert.That(All<Col>(), Is.EqualTo(4));

		[Test]
		public void LastRow()
			=> Assert.That(Last<Row>(), Is.EqualTo(2));
	}
}