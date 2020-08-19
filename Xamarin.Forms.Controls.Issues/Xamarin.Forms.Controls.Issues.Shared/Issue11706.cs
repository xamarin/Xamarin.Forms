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
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11706, "UWP Shell - Toolbar buttons do not work correctly", PlatformAffected.UWP)]
	public class Issue11706 : TestShell
	{
		protected override void Init()
		{
			ContentPage page1 = AddTopTab("Tab 1");
			page1.Title = "Top Bar Page 1";
			page1.ToolbarItems.Add(new ToolbarItem() { Text = "Primary item", IconImageSource = "books.png" });
			page1.ToolbarItems.Add(new ToolbarItem() { Text = "Secondary item", IconImageSource = "bell.png", Order = ToolbarItemOrder.Secondary });
			page1.Content =
				new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "Toolbar items should display an image"
						}
					}
				};

		}

#if UITEST && __SHELL__
		[Test]
		public void Issue11706Test()
		{
		}
#endif
	}
}