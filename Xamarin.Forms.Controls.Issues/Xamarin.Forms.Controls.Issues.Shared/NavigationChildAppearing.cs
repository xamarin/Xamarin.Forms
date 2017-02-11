using System;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using NUnit.Framework;
#endif


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Are not last pages in NavigationStack were notified appearing event when NavigationPage was added to ui stack", NavigationBehavior.PushModalAsync)]
	public class NavigationChildAppearing : TestPage
	{
		NavigationPage _navPage;
		ContentPage _appearingPage;
		ContentPage _page2, _page3;
		Label _checkIsAppearingLabel;

		IPageController PageController => this;

		protected override void Init()
		{
			NavigationPage navPage;
			ContentPage appearingPage, page2, page3;

			var appearingLabel = new Label
			{
				AutomationId = "checkIsAppearingLabel",
				Text = false.ToString()
			};

			appearingPage = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new Button
						{
							BackgroundColor = Color.Red,
							Text = "NavigateToPage2",
							Command = new Command(NavigateToPage2)
						}
					}
				}
			};
			page2 = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new StackLayout()
						{
							Orientation = StackOrientation.Horizontal,
							Children =
							{
								new Label
								{
									Text = "First page is appeared: "
								},
								appearingLabel
							}
						},
						new Button
						{
							Text = "SetPage3",
							Command = new Command(SetPage3)
						}
					}
				}
			};
			page3 = new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new Button
						{
							Text = "RevertToNavigationPage",
							Command = new Command(SetNavigationPage)
						}
					}
				}
			};

			appearingPage.Appearing += OnAppearingFirstPage;
			appearingPage.Disappearing += OnDisappearingFirstPage;

			navPage = new NavigationPage(appearingPage);

			_navPage = navPage;
			_appearingPage = appearingPage;
			_checkIsAppearingLabel = appearingLabel;
			_page2 = page2;
			_page3 = page3;

			SetPage(navPage);
		}

		void OnAppearingFirstPage(object sender, EventArgs e)
		{
			Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
			{
				var currentPage = _navPage.CurrentPage;

				_checkIsAppearingLabel.Text = true.ToString();

				if (currentPage != sender)
				{
					//throw new Exception("Page cannot get appearing event if it is not last in NavigationStack");
				}

				return false;
			});
		}

		void OnDisappearingFirstPage(object sender, EventArgs e)
		{
			_checkIsAppearingLabel.Text = false.ToString();
		}

		void SetNavigationPage()
		{
			SetPage(_navPage);
		}

		void NavigateToPage2()
		{
			if (_navPage.CurrentPage != _page2)
				_navPage.PushAsync(_page2);
		}

		void SetPage3()
		{
			SetPage(_page3);
		}

		void SetPage(Page page)
		{
			if (PageController.InternalChildren.Count > 0)
				PageController.InternalChildren.Remove(PageController.InternalChildren.First());
			PageController.InternalChildren.Add(page);
		}

		protected override bool OnBackButtonPressed()
		{
			var currentPage = (Page)PageController.InternalChildren.FirstOrDefault();
			var currentPageResult = currentPage?.SendBackButtonPressed();

			if (currentPageResult != null && currentPageResult.Value)
			{
				return true;
			}

			return base.OnBackButtonPressed();
		}

#if UITEST

		[Test]
		public void NavigationAppearingTest()
		{
			var app = RunningApp;
			app.WaitForElement(x => x.Text("NavigateToPage2"));
			app.Tap(x => x.Text("NavigateToPage2"));
			app.WaitForElement(x => x.Text("SetPage3"));
			app.Tap(x => x.Text("SetPage3"));
			app.WaitForElement(x => x.Text("RevertToNavigationPage"));
			app.Tap(x => x.Text("RevertToNavigationPage"));
			app.WaitForElement(x => x.Marked("checkIsAppearingLabel"));

			var checkIsAppearingLabel = app.Query(x => x.Marked("checkIsAppearingLabel")).First();

			Assert.IsFalse(string.Equals(checkIsAppearingLabel.Text, true.ToString()), "Page cannot get appearing event if it is not last in NavigationStack");

			app.Back();
		}

#endif


	}
}
