using System;
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
	[Issue(IssueTracker.Bugzilla, 22986, "(iOS) - \"More\" tab on TabbedPage is disabled/does not register the taps", PlatformAffected.iOS)]
	public class Bugzilla22986 : TestTabbedPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Children.Add(new NavigationPage(new ContentPage() { BackgroundColor = Color.Red }) { Title = "Page1" });
			Children.Add(new NavigationPage(new ContentPage() { BackgroundColor = Color.Blue }) { Title = "Page2" });
			Children.Add(new NavigationPage(new ContentPage() { BackgroundColor = Color.Yellow }) { Title = "Page3" });
			Children.Add(new NavigationPage(new ContentPage() { BackgroundColor = Color.Green }) { Title = "Page4" });
			Children.Add(new XTest { Title = "Page5" });
			Children.Add(new XTest { Title = "Page6" });
			Children.Add(new XTest { Title = "Page7" });
			Children.Add(new XTest { Title = "Page8" });
			Children.Add(new XTest { Title = "Page9" });
			Children.Add(new XTest { Title = "Page10" });
		}
	}

	public class XTest : ContentPage
	{
		public XTest()
		{
			Content = new StackLayout
			{
				Spacing = 10,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					new Button
					{
						HorizontalOptions = LayoutOptions.Center,
						Text = "Push modal",
						Command = new Command(async () => { await Navigation.PushModalAsync(new ContentPage { Title = "Modal", Content = new ContentView
						{
							Content = new Button
							{
								HorizontalOptions = LayoutOptions.Center,
								VerticalOptions = LayoutOptions.Center,
								Text = "Go back",
								Command = new Command(async () => { await Navigation.PopModalAsync(true); })
							}
						} }, true); })
					},
					new Button
					{
						HorizontalOptions = LayoutOptions.Center,
						Text = "Push non-modal (will error out)",
						Command = new Command(async () =>
						{
							try
							{
								await Navigation.PushAsync(new ContentPage { Title = "Non-modal" }, true);
							}
							catch(Exception e)
							{
								await DisplayAlert("Error", e.Message, "OK");
							}
						})
					}
				}
			};
		}
	}
}