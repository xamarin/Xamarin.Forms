using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class LinearGradientBrushTests : BaseTestFixture
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();

			Device.SetFlags(new[] { ExperimentalFlags.BrushExperimental });
		}

		[Test]
		public void TestConstructor()
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush();

			Assert.AreEqual(1.0d, linearGradientBrush.EndPoint.X, "EndPoint.X");
			Assert.AreEqual(0.0d, linearGradientBrush.EndPoint.Y, "EndPoint.Y");
		}

		[Test]
		public void TestLinearGradientBrushPoints()
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0)
			};

			Assert.AreEqual(0, linearGradientBrush.StartPoint.X);
			Assert.AreEqual(0, linearGradientBrush.StartPoint.Y);

			Assert.AreEqual(1, linearGradientBrush.EndPoint.X);
			Assert.AreEqual(0, linearGradientBrush.EndPoint.Y);
		}

		[Test]
		public void TestLinearGradientBrushOnlyOneGradientStop()
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush
			{
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.Red, }
				},
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0)
			};

			Assert.IsNotNull(linearGradientBrush);
		}

		[Test]
		public void TestLinearGradientBrushGradientStops()
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush
			{
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.Red, Offset = 0.1f },
					new GradientStop { Color = Color.Blue, Offset = 1.0f }
				},
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0)
			};

			Assert.AreEqual(2, linearGradientBrush.GradientStops.Count);
		}
	}
}