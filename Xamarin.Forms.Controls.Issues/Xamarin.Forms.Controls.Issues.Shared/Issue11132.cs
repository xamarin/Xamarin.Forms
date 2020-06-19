using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Frame)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11132, "[Bug] [iOS] UpdateClip throws NullReferenceException when the Name of the Mask of the Layer is null", PlatformAffected.iOS)]
	public class Issue11132 : TestContentPage
	{
		public Issue11132()
		{

		}

		protected override void Init()
		{
			Title = "Issue 11132";

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "If the test works without exceptions (an orange rectangle is rendered), the test has passed."
			};

			var issue11132Control = new Issue11132Control
			{
				BackgroundColor = Color.Red,
				HeightRequest = 100
			};

			layout.Children.Add(instructions);
			layout.Children.Add(issue11132Control);

			Content = layout;
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue11132Control : View
	{

	}
}