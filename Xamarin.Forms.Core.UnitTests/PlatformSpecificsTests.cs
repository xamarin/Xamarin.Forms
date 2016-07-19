using NUnit.Framework;
using Xamarin.Forms.PlatformConfiguration.Android;
using Xamarin.Forms.PlatformConfiguration.iOS;
using ImAVendor.Forms.PlatformConfiguration.iOS;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class PlatformSpecificsTests
	{
		[Test]
		public void VendorPlatformProperty()
		{
			var x = new MasterDetailPage();

			Assert.IsTrue(x.On<iOSPlatform>().GetVendorFoo());

			x.On<iOSPlatform>().SetVendorFoo(false);

			Assert.IsFalse(x.On<iOSPlatform>().GetVendorFoo());
		}

		[Test]
		public void ConsumeVendorSetting()
		{
			var x = new MasterDetailPage();
			x.On<iOSPlatform>().SetVendorFoo(false);

			Assert.IsFalse(x.On<iOSPlatform>().GetVendorFoo());
		}

		[Test]
		public void Properties()
		{
			var x = new MasterDetailPage();
			x.On<AndroidPlatform>().SetSomeAndroidThing(42);

			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeAndroidThing() == 42);
		}

		[Test]
		public void ConvenienceConfiguration()
		{
			var x = new MasterDetailPage();

			x.On<AndroidPlatform>().UseTabletDefaults();

			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeAndroidThing() == 10);
			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeOtherAndroidThing() == 45);

			x.On<AndroidPlatform>().UsePhabletDefaults();
			
			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeAndroidThing() == 8);
			Assert.IsTrue(x.On<AndroidPlatform>().GetSomeOtherAndroidThing() == 40);
		}

		[Test]
		public void NavigationPageiOSConfiguration()
		{
			var x = new NavigationPage();

			x.On<iOSPlatform>().EnableTranslucentNavigationBar(true);

			Assert.IsTrue(x.On<iOSPlatform>().IsNavigationBarTranslucent()); 
		}
	}
}