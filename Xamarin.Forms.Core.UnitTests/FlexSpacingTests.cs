using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class FlexSpacingTests : BaseTestFixture
	{
		[Test]
		public void TestColumnSpacingNoWrap()
		{
			var label0 = new Label { IsPlatformEnabled = true };
			var label1 = new Label { IsPlatformEnabled = true };
			var label2 = new Label { IsPlatformEnabled = true };
			var label3 = new Label { IsPlatformEnabled = true };

			var layout = new FlexLayout {
				IsPlatformEnabled = true,
				Direction = FlexDirection.Row,
				Wrap = FlexWrap.NoWrap,
				ColumnSpacing = 6d,
				Children = {
					label0,
					label1,
					label2,
					label3
				}
			};

			layout.Layout(new Rectangle(0, 0, 118, 50));
			Assert.That(layout.Bounds, Is.EqualTo(new Rectangle(0, 0, 118, 50)));
			Assert.That(label0.Bounds, Is.EqualTo(new Rectangle(0, 0, 25, 50)));
			Assert.That(label1.Bounds, Is.EqualTo(new Rectangle(31, 0, 25, 50)));
			Assert.That(label2.Bounds, Is.EqualTo(new Rectangle(62, 0, 25, 50)));
			Assert.That(label3.Bounds, Is.EqualTo(new Rectangle(93, 0, 25, 50)));

		}

		[Test]
		public void TestColumnSpacingWrap()
		{
			var label0 = new Label { IsPlatformEnabled = true };
			var label1 = new Label { IsPlatformEnabled = true };
			var label2 = new Label { IsPlatformEnabled = true };
			var label3 = new Label { IsPlatformEnabled = true };
			FlexLayout.SetGrow(label0, 1);
			FlexLayout.SetGrow(label1, 1);
			FlexLayout.SetGrow(label2, 1);
			FlexLayout.SetGrow(label3, 1);

			var layout = new FlexLayout {
				IsPlatformEnabled = true,
				Direction = FlexDirection.Row,
				Wrap = FlexWrap.Wrap,
				ColumnSpacing = 6d,
				Children = {
					label0,
					label1,
					label2,
					label3
				}
			};

			layout.Layout(new Rectangle(0, 0, 210, 50));
			Assert.That(layout.Bounds, Is.EqualTo(new Rectangle(0, 0, 210, 50)));
			Assert.That(label0.Bounds, Is.EqualTo(new Rectangle(0, 0, 102, 25)));
			Assert.That(label1.Bounds, Is.EqualTo(new Rectangle(108, 0, 102, 25)));
			Assert.That(label2.Bounds, Is.EqualTo(new Rectangle(0, 25, 102, 25)));
			Assert.That(label3.Bounds, Is.EqualTo(new Rectangle(108, 25, 102, 25)));
		}

		[Test]
		public void TestRowSpacing()
		{
			var label0 = new Label { IsPlatformEnabled = true };
			var label1 = new Label { IsPlatformEnabled = true };
			var label2 = new Label { IsPlatformEnabled = true };
			var label3 = new Label { IsPlatformEnabled = true };
			var label4 = new Label { IsPlatformEnabled = true };
			FlexLayout.SetGrow(label0, 1);
			FlexLayout.SetGrow(label1, 1);
			FlexLayout.SetGrow(label2, 1);
			FlexLayout.SetGrow(label3, 1);

			var layout = new FlexLayout {
				IsPlatformEnabled = true,
				Direction = FlexDirection.Row,
				Wrap = FlexWrap.Wrap,
				ColumnSpacing = 6d,
				RowSpacing = 4d,
				Children = {
					label0,
					label1,
					label2,
					label3,
					label4,
				}
			};

			layout.Layout(new Rectangle(0, 0, 210, 50));
			Assert.That(layout.Bounds, Is.EqualTo(new Rectangle(0, 0, 210, 50)));
			Assert.That(label0.Bounds, Is.EqualTo(new Rectangle(0, 0, 102, 14)));
			Assert.That(label1.Bounds, Is.EqualTo(new Rectangle(108, 0, 102, 14)));
			Assert.That(label2.Bounds, Is.EqualTo(new Rectangle(0, 18, 102, 14)));
			Assert.That(label3.Bounds, Is.EqualTo(new Rectangle(108, 18, 102, 14)));
			Assert.That(label4.Bounds, Is.EqualTo(new Rectangle(0, 36, 100, 14)));
		}
	}
}
