using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43734, "[Android AppCompat] TabbedPage tab title cannot be updated", PlatformAffected.Android)]
	public class Bugzilla43734 : TestTabbedPage
	{
		protected override void Init()
		{
			Children.Add(new ContentPage
			{
				Title = "1",
				Content = new Grid
				{
					Children = { new Button
					{
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						WidthRequest = 250,
						HeightRequest = 50,
						Text = "Click first",
						Command = new Command(() =>
						{
							Children[0].Title = "x";
						})
					}
					}
				}
			});

			Children.Add(new ContentPage
			{
				Title = "2",
				Content = new Grid
				{
					Children = { new Button
					{
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						WidthRequest = 250,
						HeightRequest = 50,
						Text = "Click second",
						Command = new Command(() =>
						{
							Children[1].Title = "y";
						})
					}
					}
				}
			});
		}
	}
}