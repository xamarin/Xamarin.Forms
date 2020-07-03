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
	}
}