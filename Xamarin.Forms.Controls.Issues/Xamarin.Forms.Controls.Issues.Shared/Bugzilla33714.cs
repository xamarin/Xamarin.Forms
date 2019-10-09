using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using System;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Navigation)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 33714, "[WP] Navigating Back Within MasterDetailPage.Detail Causes Crash", NavigationBehavior.PushModalAsync)]
	public class Bugzilla33714 : TestMasterDetailPage
	{
		public const string ListViewAutomationId = nameof(ListViewAutomationId);
		public const string DefaultDetailPageAutomationId = "DefaultDetail";
		public const string DetailPageAutomationId = "Detail";
		public const string MoreDetailPageAutomationId = "MoreDetail";

		public const string HomeDetailsDetailPageLabelText = "This is the home detail page";
		public const string DetailPageLabelText = "This is a Detail ContentPage";
		public const string MoreDetailPageLabelText = "This is More Detail ContentPage";
		public const string GoToMoreDetailsPageButtonText = "Go to More Detail ContentPage";
		public const string GoBackButtonText = "Go back";

		protected override void Init()
		{
			Master = new MasterPage(this);
			Detail = new NavigationPage(new ContentPage
			{
				AutomationId = DefaultDetailPageAutomationId,
				Title = "Home",
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							AutomationId = "TestLabel",
							Text = HomeDetailsDetailPageLabelText
						}
					}
				}
			});
		}

		public class MoreDetail : ContentPage
		{
			public MoreDetail()
			{
				AutomationId = MoreDetailPageAutomationId;
				Title = "More Details";
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							Text = MoreDetailPageLabelText
						},
						new Button
						{
							Text = GoToMoreDetailsPageButtonText,
							Command = new Command(async () => await Navigation.PushAsync(new MoreDetail()))
						},
						new Button
						{
							Text = GoBackButtonText,
							Command = new Command(async () => await Navigation.PopAsync())
						}
					}
				};
			}
		}

		public class DetailPage : ContentPage
		{
			public DetailPage()
			{
				AutomationId = DetailPageAutomationId;
				Title = "Detail";
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							Text = DetailPageLabelText
						},
						new Button
						{
							Text = GoToMoreDetailsPageButtonText,
							Command = new Command(async () => await Navigation.PushAsync(new MoreDetail()))
						}
					}
				};
			}
		}

		public class MasterPage : ContentPage
		{
			readonly MasterDetailPage _masterPage;
			readonly List<string> _items;

			public MasterPage(MasterDetailPage masterPage)
			{
				AutomationId = "MasterPageTest";
				_masterPage = masterPage;
				Title = "Menu";

				for (var i = 0; i < 5; i++)
				{
					if (i == 0)
						_items = new List<string>();

					_items.Add("Menu Items");
				}

				var listView = new ListView
				{
					AutomationId = ListViewAutomationId,
					ItemsSource = _items,
					RowHeight = 100,
					HasUnevenRows = true
				};
				listView.ItemSelected += list_ItemSelected;

				Content = listView;
			}

			void list_ItemSelected(object sender, SelectedItemChangedEventArgs e)
			{
				_masterPage.Detail = new NavigationPage(new DetailPage());
			}
		}

#if UITEST
		[Test]
		public void Bugzilla33714Test()
		{
			// var firstListViewElementIndex = 0;

			RunningApp.WaitForElement(q => q.Marked("TestLabel"));
			//RunningApp.WaitForElement(q => q.Marked(HomeDetailsDetailPageLabelText));
			//RunningApp.WaitForElement(q => q.Marked("Menu Items"));
			//RunningApp.WaitForElement(q => q.Marked(ListViewAutomationId));
			//var a = RunningApp.Query(q => q.Marked(ListViewAutomationId).Child(firstListViewElementIndex)).FirstOrDefault();
			//Console.WriteLine($"Found something ? {a == null}");

			//RunningApp.WaitForElement(q => q.Marked(DefaultDetailPageAutomationId));
			//RunningApp.WaitForElement(q => q.Marked(ListViewAutomationId));
			//RunningApp.Tap(q => q.Marked(ListViewAutomationId).Child(firstListViewElementIndex));

			//RunningApp.WaitForElement(q => q.Marked(DetailPageAutomationId));
			//RunningApp.WaitForElement(q => q.Marked(GoToMoreDetailsPageButtonText));
			//RunningApp.Tap(q => q.Marked(GoToMoreDetailsPageButtonText));

			//RunningApp.WaitForElement(q => q.Marked(MoreDetailPageAutomationId));
			//RunningApp.WaitForElement(q => q.Marked(GoToMoreDetailsPageButtonText));
			//RunningApp.Tap(q => q.Marked(GoToMoreDetailsPageButtonText));

			//RunningApp.Back();
			//RunningApp.WaitForElement(q => q.Marked(DetailPageAutomationId));
		}
#endif
	}
}
