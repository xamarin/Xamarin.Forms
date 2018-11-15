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
	[Issue(IssueTracker.Github, 2580, "Adding accessibility tags to a label seems to cause the renderer to need more space", PlatformAffected.Android)]
	public class Issue2580 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var instructions = new Label { Text = "Manual check that both Label 1 have the same size, if the 1st is bigger than this test failed." };
			var label = new Label { Text = "Label 1", BackgroundColor = Color.Red };
			AutomationProperties.SetHelpText(label, "This is a The longer this is, the worse the problem");
			var label2 = new Label { Text = "Label 1", BackgroundColor = Color.Red };
			var horizontalLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { label
					//,new Label { Text = "label 2" }, new Label { Text = "label 3" }, new Label { Text = "label 4" } }
				}
			};
			Content = new StackLayout
			{
				Children = {
					//instructions,
					horizontalLayout
					//, new StackLayout { Orientation = StackOrientation.Horizontal, Children = { label2, new Label { Text = "label 2" }, new Label { Text = "label 3" }, new Label { Text = "label 4" } } } } };
			}
			};
		}
	}
}