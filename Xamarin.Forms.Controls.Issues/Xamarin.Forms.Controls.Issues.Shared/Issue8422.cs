using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shell)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8422, "[Bug] [iOS] Secondary ToolbarItems are added to regular Toolbar instead a secondary Toolbar", PlatformAffected.iOS)]
	public class Issue8422 : TestShell
	{
		protected override void Init()
		{
			var newContentPage = CreateContentPage("Secondary toolbar test");
			newContentPage.Content = new Label { Text = "Testing the iOS secondary toolbar in Shell apps.", HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };

			var newPrimaryToolbarItem = new ToolbarItem { Text = "Primary", Order = ToolbarItemOrder.Primary };
			newContentPage.ToolbarItems.Add(newPrimaryToolbarItem);

			var newSecondaryToolbarItem = new ToolbarItem { Text = "Secondary", Order = ToolbarItemOrder.Secondary };
			newSecondaryToolbarItem.Command = new Command(async () => await DisplayAlert("Hello", "This works", "OK"));
			newContentPage.ToolbarItems.Add(newSecondaryToolbarItem);

			var newSecondaryToolbarItemTwo = new ToolbarItem { Text = "Secondary two", Order = ToolbarItemOrder.Secondary };
			newContentPage.ToolbarItems.Add(newSecondaryToolbarItemTwo);
		}

#if UITEST
		[Test]
		public void Issue1Test() 
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot("I am at Issue8422");
			RunningApp.WaitForElement(q => q.Marked("Secondary"));
			RunningApp.Screenshot("I see the Secondary ToolbarItem");
			RunningApp.WaitForElement(q => q.Marked("Secondary two"));
			RunningApp.Screenshot("I see a second Secondary ToolbarItem");
		}
#endif
	}

}