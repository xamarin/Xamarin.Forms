using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public class SolidColorBrushTests : BaseTestFixture
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
			SolidColorBrush solidColorBrush = new SolidColorBrush();
			Assert.AreEqual("[Color: A=-1, R=-1, G=-1, B=-1, Hue=-1, Saturation=-1, Luminosity=-1]", solidColorBrush.Color.ToString(), "Color");
		}
	}
}