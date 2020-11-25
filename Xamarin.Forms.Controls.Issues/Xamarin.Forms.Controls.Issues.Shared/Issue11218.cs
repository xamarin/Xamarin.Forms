using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11218, "[Bug] The Shell RemovePage function causes crash on iOS",
		PlatformAffected.iOS)]
#if UITEST
	//[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue11218 : TestShell
	{
		const string toMiddlePageText = "to middle page";
		const string initialPageText = "Initial page";
		const string toLastPageText = "to last page";

		protected override void Init()
		{
			var button = new Button
			{
				Text = toMiddlePageText,
				BackgroundColor = Color.Blue,
				TextColor = Color.White,
				VerticalOptions = LayoutOptions.EndAndExpand
			};

			button.Clicked += (_, __) =>
			{
				Shell.Current.GoToAsync("middlePage");
			};

			var label = new Label
			{
				Text = initialPageText,
				TextColor = Color.Black,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand
			};

			var layout = new StackLayout();
			layout.Children.Add(label);
			layout.Children.Add(button);
			var page = new ContentPage { Content = layout };
			AddContentPage(page, "page1");
			Routing.RegisterRoute("middlePage", typeof(MiddlePage));
			Routing.RegisterRoute("lastPage", typeof(LastPage));
		}

		[Preserve(AllMembers = true)]
		public partial class LastPage : ContentPage
		{
			public LastPage()
			{
				var existingPages = Shell.Current.Navigation.NavigationStack.ToList();
				foreach (var page in existingPages)
				{
					if (page != null)
					{
						Shell.Current.Navigation.RemovePage(page);
					}
				}
			}
		}

		[Preserve(AllMembers = true)]
		public class MiddlePage : ContentPage
		{
			public MiddlePage()
			{
				var button = new Button
				{
					Text = toLastPageText,
					BackgroundColor = Color.Blue,
					TextColor = Color.White,
					VerticalOptions = LayoutOptions.EndAndExpand
				};

				button.Clicked += (_, __) =>
				{
					Shell.Current.GoToAsync("lastPage");
				};

				var label = new Label
				{
					Text = "Middle page",
					TextColor = Color.Black,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				};

				var layout = new StackLayout();
				layout.Children.Add(label);
				layout.Children.Add(button);

				Content = layout;
			}
		}


#if UITEST
		[Test]
		public void Issue11218RemovePageTest()
		{
			RunningApp.WaitForElement(initialPageText);
			RunningApp.Tap(toMiddlePageText);
			RunningApp.WaitForElement(toLastPageText);
			RunningApp.Tap(toLastPageText);
			base.TapBackArrow();
			RunningApp.WaitForElement(initialPageText);
		}
#endif
	}
}