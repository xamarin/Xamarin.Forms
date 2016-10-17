using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43955, "White space created during navigation from page without the NavigationBar to one with it", PlatformAffected.iOS)]
	public class Bugzilla43995 : TestNavigationPage
	{
		protected override void Init()
		{

			Title = "ToolbarItem Page";
			var toolbarItemPage = new ContentPage
			{
				Title = "ToolbarItem Page",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.End,
					Children =
					{
						new Label
						{
							Text = "This should also be visible"
						}
					}
				}
			};
			toolbarItemPage.ToolbarItems.Add(new ToolbarItem("Action 1", null, () => { }, ToolbarItemOrder.Primary, 1));
			toolbarItemPage.ToolbarItems.Add(new ToolbarItem("Action 2", null, () => { }, ToolbarItemOrder.Primary, 2));

			toolbarItemPage.ToolbarItems.Add(new ToolbarItem("Action 3", null, () => { }, ToolbarItemOrder.Secondary, 3));
			toolbarItemPage.ToolbarItems.Add(new ToolbarItem("Action 4", null, () => { }, ToolbarItemOrder.Secondary, 4));
			toolbarItemPage.ToolbarItems.Add(new ToolbarItem("Action 5", null, () => { }, ToolbarItemOrder.Secondary, 5));

			var page4 = new ContentPage
			{
				BackgroundColor = Color.Yellow,
				Title = "Page 4"
			};
			page4.Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.End,
				Padding = new Thickness(0, 15, 0, 0),
				Children =
					{
						new Button
						{
							Text = "Click to toggle SetHasNavigationBar",
							Command = new Command(() => SetHasNavigationBar(page4, !GetHasNavigationBar(page4)))
						},
						new Button
						{
							Text = "Click for ToolbarItem Page",
							Command = new Command(async () =>
							{
								await PushAsync(toolbarItemPage);
							})
						}
					}
			};
			SetHasNavigationBar(page4, false);

			var page3 = new ContentPage
			{
				BackgroundColor = Color.Silver,
				Title = "Page 3",
			};
			page3.Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.End,
				Children =
				{
					new Button
					{
						Text = "Go back",
						Command = new Command(async () =>
						{
							await Navigation.PopAsync();
						})
					},
					new Button
					{
						Text = "Click to Navigate",
						Command = new Command(async () =>
						{
							await PushAsync(page4);
						})
					}
				}
			};

			var page2 = new ContentPage
			{
				BackgroundColor = Color.Red,
				Title = "Page 2",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.End,
					Children =
					{
						new Label
						{
							Text = "This should be visible"
						}
					}
				}
			};
			SetHasNavigationBar(page2, false);

			var page1 = new ContentPage
			{
				Title = "Page 1",
				BackgroundColor = Color.Green,
				Content = new StackLayout
				{
					Children =
					{
						new Button
						{
							Text = "Click to Navigate",
							Command = new Command(() =>
							{
								SetHasNavigationBar(page3, false);
								PushAsync(page3);
							})
						}
					}
				}
			};

			var tabbedPage = new TabbedPage();
			tabbedPage.Children.Add(page1);
			tabbedPage.Children.Add(page2);

			SetHasNavigationBar(tabbedPage, false);

			PushAsync(tabbedPage);
		}
	}
}