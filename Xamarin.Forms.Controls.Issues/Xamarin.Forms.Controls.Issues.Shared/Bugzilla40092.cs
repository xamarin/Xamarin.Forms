using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Threading.Tasks;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
    [Preserve(AllMembers = true)]
    [Issue(IssueTracker.Bugzilla, 40092, "Ensure android devices with fractional scale factors (3.5) don't have a white line around the border"
		, PlatformAffected.Android)]
#if UITEST
	[Category(UITestCategories.BoxView)]
#endif
	public class Bugzilla40092 : TestContentPage
    {
		const string Black = "black";
		const string White = "white";
        protected override void Init()
        {
            AbsoluteLayout mainLayout = new AbsoluteLayout()
            {
                BackgroundColor = Color.White,
				AutomationId = White
            };


            // The root page of your application
            var thePage = new ContentView
            {
                BackgroundColor = Color.Red,
                Content = mainLayout
            };

            BoxView view = new BoxView()
            {
                Color = Color.Black,
				AutomationId = Black
            };

            mainLayout.Children.Add(view, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            Content = thePage;

        }


#if UITEST
		[Test]
		public void AllScreenIsBlack()
		{
			
			var box = RunningApp.WaitForElement(Black)[0];
			var layout = RunningApp.WaitForElement(White)[0];

			if (box.Rect.Height == layout.Rect.Height &&
				box.Rect.Width == layout.Rect.Width)
				RunningApp.WaitForElement(Black);
			else
				RunningApp.WaitForNoElement(Black);
		}
#endif
	}
}
