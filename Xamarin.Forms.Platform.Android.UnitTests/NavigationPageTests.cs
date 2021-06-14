using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Platform.Android.UnitTests;

#if __ANDROID_29__
using AndroidX.AppCompat.App;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;
#else
#endif

namespace Xamarin.Forms.Platform.Android.UnitTests
{

	public class NavigationPageTests : PlatformTestFixture
	{
		[Test, Category("NavigationPage")]
		[Description("Pushing initial page before NavigationPageRenderer is attached leaves renderer in confused state")]
		public async Task PushInitialPageBeforeAttachingToWindowBreaksApp()
		{
			await Device.InvokeOnMainThreadAsync(async () =>
			{
				var navPage = new NavigationPage();
				TestActivity testSurface = null;
				try
				{
					testSurface = await TestActivity.GetTestSurface(Context, navPage);
					await navPage.PushAsync(new ContentPage());
					await testSurface.WindowAttachedTask;
					await navPage.PushAsync(new ContentPage());
					var manager = testSurface.GetFragmentManager();
					Assert.AreEqual(2, manager.Fragments.Count);
				}
				finally
				{
					testSurface?.Finish();
				}
			});
		}

		[Test, Category("NavigationPage")]
		[Description("Pushing page before NavigationPageRenderer is attached leaves renderer in confused state")]
		public async Task PagePushedBeforeAttachingToWindowBreaksApp()
		{
			await Device.InvokeOnMainThreadAsync(async () =>
			{
				var navPage = new NavigationPage(new ContentPage());
				TestActivity testSurface = null;
				try
				{
					testSurface = await TestActivity.GetTestSurface(Context, navPage);
					await navPage.PushAsync(new ContentPage());
					await testSurface.WindowAttachedTask;
					await navPage.PushAsync(new ContentPage());
					var manager = testSurface.GetFragmentManager();
					Assert.AreEqual(3, manager.Fragments.Count);
				}
				finally
				{
					testSurface?.Finish();
				}
			});
		}

		[Test, Category("NavigationPage")]
		[Description("Popping page before NavigationPageRenderer is attached leaves renderer in confused state")]
		public async Task PagePoppedBeforeAttachingToWindowBreaksApp()
		{
			var navPage = new NavigationPage(new ContentPage());
			await navPage.PushAsync(new ContentPage());

			await Device.InvokeOnMainThreadAsync(async () =>
			{
				TestActivity testSurface = null;
				try
				{
					testSurface = await TestActivity.GetTestSurface(Context, navPage);
					await navPage.PopAsync();
					await testSurface.WindowAttachedTask;
					await navPage.PushAsync(new ContentPage());
					var manager = testSurface.GetFragmentManager();
					Assert.AreEqual(2, manager.Fragments.Count);
				}
				finally
				{
					testSurface?.Finish();
				}
			});
		}

		[Test, Category("NavigationPage")]
		[Description("Pop to Root before NavigationPageRenderer is attached leaves renderer in confused state")]
		public async Task PopToRootBeforeAttachingToWindowBreaksApp()
		{
			var navPage = new NavigationPage(new ContentPage());
			await navPage.PushAsync(new ContentPage());
			await navPage.PushAsync(new ContentPage());
			await navPage.PushAsync(new ContentPage());

			await Device.InvokeOnMainThreadAsync(async () =>
			{
				TestActivity testSurface = null;
				try
				{
					testSurface = await TestActivity.GetTestSurface(Context, navPage);
					await navPage.PopToRootAsync();
					await testSurface.WindowAttachedTask;
					await navPage.PushAsync(new ContentPage());
					var manager = testSurface.GetFragmentManager();
					Assert.AreEqual(2, manager.Fragments.Count);
				}
				finally
				{
					testSurface?.Finish();
				}
			});
		}
	}
}
