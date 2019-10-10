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
		public const string MenuItemsAutomationId = nameof(MenuItemsAutomationId);
		public const string DefaultDetailPageAutomationId = nameof(DefaultDetailPageAutomationId);
		public const string CustomDetailPageAutomationId = nameof(CustomDetailPageAutomationId);
		public const string NestedDetailPageAutomationId = nameof(NestedDetailPageAutomationId);
		public const string GenericMenuItemName = "Menu Item";

		public const string HomeDetailsDetailPageLabelText = "This is the home detail page";
		public const string CustomDetailPageLabelText = "This is a Detail ContentPage";
		public const string NestedDetailPageLabelText = "This is More Detail ContentPage";
		public const string GoToMoreDetailsPageButtonText = "Go to More Detail ContentPage";
		public const string GoBackButtonText = "Go back";

		protected override void Init()
		{
			Master = CreateMasterPage();
			Detail = CreateDefaultDetailPage();
		}

		NavigationPage CreateDefaultDetailPage()
		{
			return new NavigationPage(new ContentPage
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

		NavigationPage CreateCustomDetailPage()
		{
			return new NavigationPage(new ContentPage()
			{
				AutomationId = CustomDetailPageAutomationId,
				Title = "Detail",
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							Text = CustomDetailPageLabelText
						},
						new Button
						{
							Text = GoToMoreDetailsPageButtonText,
							Command = new Command(async () => await Detail.Navigation.PushAsync(CreateNestedDetailContentPage()))
						}
					}
				}
			});
		}

		ContentPage CreateMasterPage()
		{
			var menuItems = new ListView()
			{
				AutomationId = MenuItemsAutomationId,
				RowHeight = 100,
				HasUnevenRows = true,
				ItemsSource = new List<string>()
				{
					$"{GenericMenuItemName} One",
					$"{GenericMenuItemName} Two",
					$"{GenericMenuItemName} Three",
					$"{GenericMenuItemName} Four",
					$"{GenericMenuItemName} Five"
				}
			};

			menuItems.ItemSelected += (sender, args) =>
			{
				Detail = CreateCustomDetailPage();
			};

			return new ContentPage()
			{
				Title = "Master",
				Content = menuItems
			};
		}

		ContentPage CreateNestedDetailContentPage()
		{
			return new ContentPage()
			{
				AutomationId = NestedDetailPageAutomationId,
				Title = "More Details",
				Content = new StackLayout
				{
					Children =
					{
						new Label
						{
							Text = NestedDetailPageLabelText
						},
						new Button
						{
							Text = GoToMoreDetailsPageButtonText,
							Command = new Command(async () => await Detail.Navigation.PushAsync(CreateNestedDetailContentPage()))
						},
						new Button
						{
							Text = GoBackButtonText,
							Command = new Command(async () => await Detail.Navigation.PopAsync())
						}
					}
				}
			};
		}

#if UITEST
		[Test]
		public void Bugzilla33714Test()
		{
			var firsListViewElementIndex = 0;

			RunningApp.WaitForElement(q => q.Marked(DefaultDetailPageAutomationId));
			RunningApp.WaitForElement(q => q.Marked(MenuItemsAutomationId));
			RunningApp.Tap(q => q.Marked(MenuItemsAutomationId).Descendant(firsListViewElementIndex));

			RunningApp.WaitForElement(q => q.Marked(CustomDetailPageAutomationId));
			RunningApp.WaitForElement(q => q.Marked(GoToMoreDetailsPageButtonText));
			RunningApp.Tap(q => q.Marked(GoToMoreDetailsPageButtonText));

			RunningApp.WaitForElement(q => q.Marked(NestedDetailPageAutomationId));
			RunningApp.WaitForElement(q => q.Marked(GoToMoreDetailsPageButtonText));
			RunningApp.Tap(q => q.Marked(GoToMoreDetailsPageButtonText));

			RunningApp.Back();
			RunningApp.WaitForElement(q => q.Marked(NestedDetailPageAutomationId));

			RunningApp.Back();
			RunningApp.WaitForElement(q => q.Marked(CustomDetailPageAutomationId));
		}
#endif
	}
}
