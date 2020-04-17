using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Text;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7090, "Navigating to CarouselPage in Flyout shows Black Page on iOS ", PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Github7090 : TestShell
	{
		protected override void Init()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i <= 40; i++)
				sb.AppendLine("ContentPage with ScrollView");

			var cpSecond = new ContentPage
			{
				Content =
					new ScrollView
					{
						VerticalOptions = LayoutOptions.FillAndExpand,
						Content = new StackLayout
						{
							VerticalOptions = LayoutOptions.FillAndExpand,
							BackgroundColor = Color.CadetBlue,
							Children =
							{
								new Label {
									Text = sb.ToString(),
									VerticalOptions = LayoutOptions.FillAndExpand,
								},
								new Button
								{
									AutomationId = "mockBtn", Text = "Just a button",
									BackgroundColor = Color.Beige,
									VerticalOptions = LayoutOptions.FillAndExpand
								}
							}
						}
					}


			};
			var cpFirst = new ContentPage
			{
				Content = new StackLayout
				{
					BackgroundColor = Color.CornflowerBlue,
					Children =
					{
						new Label {
							Text = "Simple ContentPage",
							VerticalOptions = LayoutOptions.FillAndExpand,
						},
						new Button
						{
							AutomationId = "mockBtn", Text = "Just a button",
							BackgroundColor = Color.DarkSeaGreen,
							VerticalOptions = LayoutOptions.FillAndExpand
						}
					}
				}
			};

			var contentPage = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new Label { Text = "Single ContentPage" },
						new Button {  AutomationId = "mockBtn", Text = "Just a button", }
					}
				}
			};
			ShellItem shellItem = new ShellItem
			{
				Title = "Carousel Page",
				Route = "test_carousel",
				Items =
				{
					new ShellSection()
					{
						Items =
						{
							new ShellContent()
							{
								ContentTemplate = new DataTemplate(() =>new CarouselPage()
								{
									Title = "G7090",
									Children = { cpFirst, cpSecond }

								})
							}
						}
					}
				}
			};
			ShellItem contentPageItem = new ShellItem
			{
				Title = "ContentPage",
				Route = "single_cp",
				Items =
				{
					new ShellSection()
					{
						Items =
						{
							new ShellContent()
							{
								ContentTemplate = new DataTemplate(() => contentPage)
							}
						}
					}
				}
			};
			Items.Add(shellItem);
			Items.Add(contentPageItem);
		}
	}
}