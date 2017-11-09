using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.FlexLayoutTests
{
	public class FlexLayoutBaseTestFixture : BaseTestFixture
	{

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			// global::Xamarin.Forms.FlexLayout.RegisterEngine(typeof(Xamarin.FlexLayoutEngine.Yoga.YogaEngine));
			global::Xamarin.Forms.FlexLayout.RegisterEngine(typeof(Xamarin.FlexLayoutEngine.Flex.FlexEngine));
			Device.PlatformServices = new MockPlatformServices();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			global::Xamarin.Forms.FlexLayout.RegisterEngine(null);
			Device.PlatformServices = null;
		}
	}
}
