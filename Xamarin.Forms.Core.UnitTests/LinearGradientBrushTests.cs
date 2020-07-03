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
	}
}