using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43995, "White space created during navigation from page without the NavigationBar to one with it", PlatformAffected.All)]
	public class Bugzilla43995 : TestNavigationPage
	{
		protected override void Init()
		{

			Title = "ToolbarItem Page";
            //BarTextColor = Color.FromHex("#FF5722");

			var toolbarItemPage = new ContentPage
			{
				BackgroundColor = Color.Black,
				Title = "ToolbarItem Page"
			};

			toolbarItemPage.Content = new ContentView
			{
				Margin = new Thickness(5),
				BackgroundColor = Color.Red,
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.End,
					Children =
					{
						new Button
						{
							Text = "Add Toolbar item",
							Command = new Command(() =>
							{
								toolbarItemPage.ToolbarItems.Add(new ToolbarItem($"Action {ToolbarItems.Count+1}", null, () => { }, ToolbarItemOrder.Secondary, 1));
							})
						},
						new Button
						{
							Text = "Remove last Toolbar item",
							Command = new Command(() =>
							{
								if(toolbarItemPage.ToolbarItems.Count > 0)
									toolbarItemPage.ToolbarItems.Remove(toolbarItemPage.ToolbarItems.Last());
							})
						},
						new Button
						{
							Text = "Clear Toolbar items",
							Command = new Command(() =>
							{
								toolbarItemPage.ToolbarItems.Clear();
							})
						},
						new Button
						{
							Text = "Change BarBackgroundColor",
							Command = new Command(() =>
							{
								BarBackgroundColor = BarBackgroundColor == Color.FromHex("#FF9800") ? Color.FromHex("#8BC34A") : Color.FromHex("#FF9800");
							})
						},
						new Button
						{
							Text = "Change BarTextColor",
							Command = new Command(() =>
							{
								BarTextColor = BarTextColor == Color.FromHex("#795548") ? Color.FromHex("#1A237E") : Color.FromHex("#795548");
							})
						},
						new Button
						{
							Text = "Show or hide toolbar",
							Command = new Command(() =>
							{
								NavigationPage.SetHasNavigationBar(toolbarItemPage, !NavigationPage.GetHasNavigationBar(toolbarItemPage));
							})
						},
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
				BackgroundColor = Color.Black,
				Title = "Page 4"
			};

			page4.ToolbarItems.Add(new ToolbarItem("Action", null, () => { }, ToolbarItemOrder.Secondary, 1));

			page4.Content = new ContentView
			{
				Margin = new Thickness(5),
				BackgroundColor = Color.Yellow,
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.End,
					Padding = new Thickness(0, 15, 0, 0),
					Children =
					{
						new Button
						{
							Text = "Show or hide toolbar",
							Command = new Command(() =>
							{
								NavigationPage.SetHasNavigationBar(page4, !NavigationPage.GetHasNavigationBar(page4));
							})
						},
						new Button
						{
							Text = "This should be visible; click for ToolbarItem Page",
							Command = new Command(async () =>
							{
								await PushAsync(toolbarItemPage);
							})
						}
					}
				}
			};

			var page3 = new ContentPage
			{
				BackgroundColor = Color.Silver,
				Title = "Page 3",
				Content = new StackLayout
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
			NavigationPage.SetHasNavigationBar(page2, false);

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

			NavigationPage.SetHasNavigationBar(tabbedPage, false);

			PushAsync(tabbedPage);
		}
	}
}