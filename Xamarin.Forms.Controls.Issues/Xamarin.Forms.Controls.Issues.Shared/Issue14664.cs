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
	[Issue(IssueTracker.Github, 14664, "[Bug] ImageButton.Aspect Property is always Fill",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.ListView)]
	[NUnit.Framework.Category(UITestCategories.ManualReview)]
#endif
	public class Issue14664 : TestContentPage
	{
		protected override void Init()
		{
			var layout = new Grid
			{
				BackgroundColor = Color.Red
			};

			var listView = new ListView
			{
				BackgroundColor = Color.Green,
				ItemsSource = new string[]
				{
					"Item 1",
					"Item 2",
					"Item 3",
					"Item 4",
					"Item 5",
					"Item 6",
					"Item 7",
					"Item 8",
					"Item 9"
				}
			};

			layout.Children.Add(listView);

			Content = layout;
		}
	}
}