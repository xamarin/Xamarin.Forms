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
	[Category(UITestCategories.Image)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4933, "Grid size incorrect when using with Image", PlatformAffected.All)]
	public class Issue4493 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			BackgroundColor = Color.Gray;
			var contentGrid = new Grid
			{
				AutomationId = "IssuePageGrid",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Maroon,
				RowSpacing = 0,
				RowDefinitions = new RowDefinitionCollection()
				{
					new RowDefinition(){Height = GridLength.Auto},
					new RowDefinition(){Height = 20}
				}
			};
			contentGrid.AddChild(new Image() { Source = "photo.jpg", AutomationId = "IssuePageImage" }, 0, 0);
			contentGrid.AddChild(new Label() { Text = "test message", BackgroundColor = Color.Blue }, 0, 1);
			Content = contentGrid;
		}
#if UITEST
		[Test]
		public void Issue4493Test() 
		{
			// Delete this and all other UITEST sections if there is no way to automate the test. Otherwise, be sure to rename the test and update the Category attribute on the class. Note that you can add multiple categories.
			RunningApp.Screenshot ("I am at Issue 4493");
			RunningApp.WaitForElement (q => q.Marked ("IssuePageLabel"));
			RunningApp.WaitForElement (q => q.Marked ("IssuePageImage"));
			RunningApp.Screenshot ("I see the grid has proper height - I can't see any Maroon Color");

		}
#endif
	}
}