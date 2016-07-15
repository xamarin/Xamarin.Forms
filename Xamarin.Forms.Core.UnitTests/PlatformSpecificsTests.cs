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

			Assert.IsTrue(x.On<IConfigIOS>().GetVendorFoo());

			x.On<IConfigIOS>().SetVendorFoo(false);

			Assert.IsFalse(x.On<IConfigIOS>().GetVendorFoo());
		}

		[Test]
		public void ConsumeVendorSetting()
		{
			var x = new MasterDetailPage();
			x.On<IConfigIOS>().SetVendorFoo(false);

			Assert.IsFalse(x.On<IConfigIOS>().GetVendorFoo());
		}

		[Test]
		public void Properties()
		{
			var x = new MasterDetailPage();
			x.On<IConfigAndroid>().SetSomeAndroidThing(42);

			Assert.IsTrue(x.On<IConfigAndroid>().GetSomeAndroidThing() == 42);
		}

		[Test]
		public void ConvenienceConfiguration()
		{
			var x = new MasterDetailPage();

			x.On<IConfigAndroid>().UseTabletDefaults();

			Assert.IsTrue(x.On<IConfigAndroid>().GetSomeAndroidThing() == 10);
			Assert.IsTrue(x.On<IConfigAndroid>().GetSomeOtherAndroidThing() == 45);

			x.On<IConfigAndroid>().UsePhabletDefaults();
			
			Assert.IsTrue(x.On<IConfigAndroid>().GetSomeAndroidThing() == 8);
			Assert.IsTrue(x.On<IConfigAndroid>().GetSomeOtherAndroidThing() == 40);
		}

		[Test]
		public void NavigationPageiOSConfiguration()
		{
			var x = new NavigationPage();

			x.On<IConfigIOS>().SetNavigationBarIsTranslucent(true);

			Assert.IsTrue(x.On<IConfigIOS>().GetNavigationBarIsTranslucent()); 
		}
	}
}