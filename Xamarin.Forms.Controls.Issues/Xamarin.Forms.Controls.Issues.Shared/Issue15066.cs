using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 15066, "[Bug] StackLayout's layout inside ScrollView is not updated properly when adding children", PlatformAffected.Android)]
	public class Issue15066 : TestContentPage
	{
		protected override void Init()
		{
			Button addButton = new Button()
			{
				Text = "Add",
			};
			Label testInstructionsLabel = new Label()
			{
				Text = "Click the 'Add' button until the height of the dark green StackLayout is larger than the height of the page. " +
					"Then scroll to the end of the page. " +
					"If there is a dark blue gap at the end of the page, the test has failed.",
				HorizontalTextAlignment = TextAlignment.Center,
			};
			StackLayout containerStackLayout = new StackLayout()
			{
				BackgroundColor = Color.DarkGreen,
				Children = { addButton, testInstructionsLabel },
			};
			addButton.Clicked += (_, __) =>
				containerStackLayout.Children.Add(new StackLayout() { BackgroundColor = Color.Gray, HeightRequest = 40.0 });
			Content = new ScrollView()
			{
				BackgroundColor = Color.DarkBlue,
				Content = containerStackLayout,
			};
		}
	}
}
