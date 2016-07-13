using NUnit.Framework;


namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class PlatformSpecificsTests
	{
		[Test]
		public void VendorPlatformProperty()
		{
			var x = new MasterDetailPage();

			Assert.IsTrue(x.OnWindows().GetVendorFoo());

			x.OnWindows().SetVendorFoo(false);

			Assert.IsFalse(x.OnWindows().GetVendorFoo());
		}

		[Test]
		public void ConsumeVendorSetting()
		{
			var x = new MasterDetailPage();
			x.OnWindows().SetVendorFoo(false);

			Assert.IsFalse(x.OnWindows().GetVendorFoo());
		}

		[Test]
		public void Properties()
		{
			var x = new MasterDetailPage();
			x.OnAndroid().SetSomeAndroidThing(42);

			Assert.IsTrue(x.OnAndroid().GetSomeAndroidThing() == 42);
		}

		[Test]
		public void ConvenienceConfiguration()
		{
			var x = new MasterDetailPage();

			x.OnAndroid().UseTabletDefaults();
			
			Assert.IsTrue(x.OnAndroid().GetSomeAndroidThing() == 10);
			Assert.IsTrue(x.OnAndroid().GetSomeOtherAndroidThing() == 45);

			x.OnAndroid().UsePhabletDefaults();
			
			Assert.IsTrue(x.OnAndroid().GetSomeAndroidThing() == 8);
			Assert.IsTrue(x.OnAndroid().GetSomeOtherAndroidThing() == 40);
		}

		[Test]
		public void NavigationPageiOSConfiguration()
		{
			var x = new NavigationPage();

			x.OniOS().SetNavigationBarIsTranslucent(true);

			Assert.IsTrue(x.OniOS().GetNavigationBarIsTranslucent()); 
		}
	}
}