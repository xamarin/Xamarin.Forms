using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 32206, "ContextActions cause memory leak: Page is never destroyed", PlatformAffected.iOS)]
	public class Bugzilla32206 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new LandingPage32206());
		}

#if UITEST
		[Test]
		public void Bugzilla32206Test()
		{
			for (var n = 0; n < 10; n++)
			{
				RunningApp.WaitForElement(q => q.Marked("Push"));
				RunningApp.Tap(q => q.Marked("Push"));

				RunningApp.WaitForElement(q => q.Marked("ListView"));
				RunningApp.Back();
			}

			// At this point, the counter can be any value, but it's most likely not zero.
			// Invoking GC once is enough to clean up all garbage data and set counter to zero
			RunningApp.WaitForElement(q => q.Marked("GC"));
			RunningApp.Tap(q => q.Marked("GC"));

			RunningApp.WaitForElement(q => q.Marked("Counter: 0"));
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class LandingPage32206 : ContentPage
	{
		public static int s_counter;
		public Label _label;

		public LandingPage32206()
		{
			_label = new Label
			{
				Text = "Counter: " + s_counter,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Spacing = 15,
				Children =
				{
					new Label
					{
						Text = "Click Push to show a ListView. When you hit the Back button, Counter will show the number of pages that have not been finalized yet."
						+ " If you click GC, the counter should be 0."
					},
					_label,
					new Button
					{
						Text = "GC",
						AutomationId = "GC",
						Command = new Command(o =>
						{
							GC.Collect();
							GC.WaitForPendingFinalizers();
							GC.Collect();
							_label.Text = "Counter: " + s_counter;
						})
					},
					new Button
					{
						Text = "Push",
						AutomationId = "Push",
						Command = new Command(async o =>
						{
							await Navigation.PushAsync(new ContentPage32206());
						})
					}
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (_label != null)
				_label.Text = "Counter: " + s_counter;
		}
	}

	[Preserve(AllMembers = true)]
	public class ContentPage32206 : ContentPage
	{
		public ContentPage32206()
		{
			Interlocked.Increment(ref LandingPage32206.s_counter);
			System.Diagnostics.Debug.WriteLine("Page: " + LandingPage32206.s_counter);

			Content = new ListView
			{
				ItemsSource = new List<string> { "Apple", "Banana", "Cherry" },
				ItemTemplate = new DataTemplate(typeof(ViewCell32206)),
				AutomationId = "ListView"
			};
		}

		~ContentPage32206()
		{
			Interlocked.Decrement(ref LandingPage32206.s_counter);
			System.Diagnostics.Debug.WriteLine("Page: " + LandingPage32206.s_counter);
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewCell32206 : ViewCell
	{
		public ViewCell32206()
		{
			View = new Label();
			View.SetBinding(Label.TextProperty, ".");
			ContextActions.Add(new MenuItem { Text = "Delete" });
		}
	}
}