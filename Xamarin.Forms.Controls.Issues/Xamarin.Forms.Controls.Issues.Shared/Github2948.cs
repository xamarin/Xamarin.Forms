using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2948, "[Android] Create a platform specific for UseAnimations in TabbedPageRenderer", PlatformAffected.Android)]
	public class Bugzilla2948 : TestTabbedPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			for(var i =0; i<5; i++)
			{
				var flag1 = true;
				var flag2 = true;

				var contentPage = new ContentPage();
				contentPage.BackgroundColor = Color.FromRgb(Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToByteArray()[0]);
				contentPage.Title = "Page " + i;

				var stackLayout = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					Spacing = 10,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.CenterAndExpand
				};

				var button1 = new Button();
				button1.Text = "Disable smooth scrolling";
				button1.BackgroundColor = Color.Black;
				button1.TextColor = Color.White;
				button1.WidthRequest = 250;
				button1.HeightRequest = 50;
				button1.Command = new Command(() =>
				{
					if (!flag1)
						button1.Text = "Disable smooth scrolling";
					else
						button1.Text = "Enable smooth scrolling";

					flag1 = !flag1;
					On<Android>().SetIsSmoothScrollEnabled(flag1);
				});
				stackLayout.Children.Add(button1);

				var button2 = new Button();
				button2.Text = "Disable swipe paging";
				button2.BackgroundColor = Color.Black;
				button2.TextColor = Color.White;
				button2.WidthRequest = 250;
				button2.HeightRequest = 50;
				button2.Command = new Command(() =>
				{
					if (!flag2)
						button2.Text = "Disable swipe paging";
					else
						button2.Text = "Enable swipe paging";

					flag2 = !flag2;
					On<Android>().SetIsSwipePagingEnabled(flag2);
				});
				stackLayout.Children.Add(button2);

				contentPage.Content = stackLayout;

				Children.Add(contentPage);
			}
		}
	}
}