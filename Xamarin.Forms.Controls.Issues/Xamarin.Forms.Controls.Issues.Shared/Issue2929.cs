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
	[Category(UITestCategories.ListView)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2929, "[UWP] ListView with null ItemsSource crashes on 3.0.0.530893", 
		PlatformAffected.UWP)]
	public class Issue2929 : TestContentPage
	{
		const string Success = "Success";

		protected override void Init()
		{
			var lv = new ListView();

			Content = new StackLayout()
			{
				Children =
				{
					new Label{Text = Success},
					lv
				}
			};
		}

#if UITEST
		[Test]
		public void NullItemSourceDoesNotCrash ()
		{
			// If we can see the Success label, it means we didn't crash. 
			RunningApp.WaitForElement (Success);
		}
#endif
	}
}