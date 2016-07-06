using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	public static class FakeVendorExtensions
	{
		public static readonly BindableProperty FooProperty = BindableProperty.Create("FooWindows", typeof(bool),
			typeof(MasterDetailPage), true);

		public static void SetFoo(this MasterDetailPage mdp, bool value)
		{
			mdp.SetValue(FooProperty, value);
		}

		public static bool GetFoo(this MasterDetailPage mdp)
		{
			return (bool)mdp.GetValue(FooProperty);
		}

		public static void SetFoo(this IMasterDetailPageWindowsConfiguration config, bool value)
		{
			config.Element.SetValue(FooProperty, value);
		}

		public static bool GetFoo(this IMasterDetailPageWindowsConfiguration config)
		{
			return config.Element.GetFoo();
		}
	}

	[TestFixture]
	public class PlatformSpecificsTests
	{
		[Test]
		public void AttachedProperty()
		{
			var x = new MasterDetailPage();

			Assert.IsTrue(x.GetFoo());

			x.SetFoo(false);

			Assert.IsFalse(x.GetFoo());
		}

		[Test]
		public void ConsumeVendorSetting()
		{
			var x = new MasterDetailPage();
			x.OnWindows().SetFoo(false);

			Assert.IsFalse(x.GetFoo());

			Assert.IsFalse(x.OnWindows().GetFoo());
		}

		[Test]
		public void Properties()
		{
			var x = new MasterDetailPage();
			x.OnAndroid().SomeAndroidThing = 42;

			Assert.IsTrue(x.OnAndroid().SomeAndroidThing == 42);
		}

		[Test]
		public void ConvenienceConfiguration()
		{
			var x = new MasterDetailPage();

			x.OnAndroid().UseTabletDefaults();
			
			Assert.IsTrue(x.OnAndroid().SomeAndroidThing == 10);
			Assert.IsTrue(x.OnAndroid().SomeOtherAndroidThing == 45);

			x.OnAndroid().UsePhabletDefaults();
			
			Assert.IsTrue(x.OnAndroid().SomeAndroidThing == 8);
			Assert.IsTrue(x.OnAndroid().SomeOtherAndroidThing == 40);
		}
	}
}