using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class RadialGradientBrushTests : BaseTestFixture
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
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush();

			int gradientStops = radialGradientBrush.GradientStops.Count;

			Assert.AreEqual(0, gradientStops);
		}

		[Test]
		public void TestRadialGradientBrushRadius()
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush();
			radialGradientBrush.Radius = 20;

			Assert.AreEqual(20, radialGradientBrush.Radius);
		}

		[Test]
		public void TestRadialGradientBrushOnlyOneGradientStop()
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush
			{
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.Red, }
				},
				Radius = 20
			};

			Assert.IsNotNull(radialGradientBrush);
		}

		[Test]
		public void TestRadialGradientBrushGradientStops()
		{
			RadialGradientBrush radialGradientBrush = new RadialGradientBrush
			{
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.Red, Offset = 0.1f },
					new GradientStop { Color = Color.Blue, Offset = 1.0f }
				},
				Radius = 20
			};

			Assert.AreEqual(2, radialGradientBrush.GradientStops.Count);
		}
	}
}