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
	[Issue(IssueTracker.Bugzilla, 44476, "[Android] Unwanted margin at top of details page when nested in a NavigationPage", 
		PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.MasterDetailPage)]
#endif
	public class Bugzilla44476 : TestNavigationPage
	{
		const string _success = "This should be visible.";
		protected override void Init()
		{
			BackgroundColor = Color.Maroon;
			
			PushAsync(new MasterDetailPage
			{
				Title = "Bugzilla Issue 44476",
				Master = new ContentPage
				{
					Title = "Master",
					Content = new StackLayout
					{
						Children =
						{
							new Label { Text = "Master" }
						}
					}
				},
				Detail = new ContentPage
				{
					Title = "Detail",
					Content = new StackLayout
					{
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children =
						{
							new Label { Text = "Detail Page" },
							new StackLayout
							{
								VerticalOptions = LayoutOptions.EndAndExpand,
								Children =
								{
									new Label { Text = _success  }
								}
							}
						}
					}
				},
			});
		}


#if UITEST
		[Test]
		public void UnwantedMarginAtTopOfDetailsPageWhenNestedInANavigationPage()
		{
			RunningApp.WaitForElement(_success);
		}
#endif
	}
}