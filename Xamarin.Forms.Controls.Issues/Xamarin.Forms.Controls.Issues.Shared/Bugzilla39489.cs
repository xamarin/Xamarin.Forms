using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 39489, "Memory leak when using NavigationPage with Maps", PlatformAffected.Android)]
	public class Bugzilla39489 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new Bz39489Content());
		}

#if UITEST
		[Test]
		public async Task Bugzilla39489Test()
		{
			// Original bug report (https://bugzilla.xamarin.com/show_bug.cgi?id=39489) had a crash (OOM) after 25-30
			// page loads. Obviously it's going to depend heavily on the device and amount of available memory, but
			// if this starts failing before 50 we'll know we've sprung another serious leak
			int iterations = 50;

			for (int n = 0; n < iterations; n++)
			{
				RunningApp.WaitForElement(q => q.Marked("New Page"));
				RunningApp.Tap(q => q.Marked("New Page"));
				RunningApp.WaitForElement(q => q.Marked("New Page"));
				await Task.Delay(1000);
				RunningApp.Back();
			}
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class Bz39489Content : ContentPage
	{
		public Bz39489Content()
		{
			var button = new Button { Text = "New Page" };

			var gcbutton = new Button { Text = "GC" };

			var map = new Map();

			button.Clicked += Button_Clicked;
			gcbutton.Clicked += GCbutton_Clicked;

			Content = new StackLayout { Children = { button, gcbutton, map } };
		}

		void GCbutton_Clicked(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(">>>>>>>> Running Garbage Collection");
			GC.Collect();
			GC.WaitForPendingFinalizers();
			System.Diagnostics.Debug.WriteLine($">>>>>>>> GC.GetTotalMemory = {GC.GetTotalMemory(true):n0}");
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new Bz39489Content());
		}
	}
}