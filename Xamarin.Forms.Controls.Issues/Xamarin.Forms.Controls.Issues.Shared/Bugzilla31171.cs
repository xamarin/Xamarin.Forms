using System;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 31171, "Popped page gets held in memory when using NavigationPage")]
	public class Bugzilla31171 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new Bugzilla31171Page1());
		}

		public class Bugzilla31171Page1 : ContentPage
		{
			Label pageLabel;
			WeakReference page2Tracker;

			public Bugzilla31171Page1()
			{
				var stack = new StackLayout() { VerticalOptions = LayoutOptions.Center };

				pageLabel = new Label
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					Text = "Page 1",
				};
				stack.Children.Add(pageLabel);

				Content = stack;
			}

			protected override async void OnAppearing()
			{
				base.OnAppearing();

				if (page2Tracker == null)
				{
					var page2 = new Bugzilla31171Page2();

					page2Tracker = new WeakReference(page2);

					await Task.Delay(1000);
					await Navigation.PushAsync(page2);

					StartTrackPage2();
				}
			}


			async void StartTrackPage2()
			{
				var flag = true;
				while (flag)
				{
					pageLabel.Text = string.Format("Page 1. Page 2 IsAlive = {0}", page2Tracker.IsAlive);
					if (!page2Tracker.IsAlive)
					{
						flag = false;
						break;
					}
					await Task.Delay(1000);
					GC.Collect();
				}
			}
		}

		class Bugzilla31171Page2 : ContentPage
		{
			public Bugzilla31171Page2()
			{
				Content = new Label()
				{
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center,
					Text = "Page 2"
				};
			}

			protected override async void OnAppearing()
			{
				base.OnAppearing();

				await Task.Delay(1000);
				await Navigation.PopAsync();
			}
		}

#if UITEST
		[Test]
		public void Bugzilla31171Test()
		{
			RunningApp.WaitForElement(q => q.Text("Page 1. Page 2 IsAlive = False"));
		}
#endif
	}
}