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
	[Issue(IssueTracker.Github, 14505, "[Bug] Shell BackgroundColor and TabBarBackgroundColor do not update on iOS 15", PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue14505 : TestShell
	{
		const string Test1 = "Test 1";
		const string Test2 = "Test 2";

		protected override void Init()
		{
			AddBottomTab(CreatePage1(Test1), Test1);
			AddBottomTab(CreatePage2(Test2), Test2);

			static ContentPage CreatePage1(string title)
			{
				var layout = new StackLayout();

				var instructions = new Label
				{
					Padding = 12,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					Text = "If the NavigationBar is Red, everything is ok. Pass to the next Tab."
				};

				layout.Children.Add(instructions);

				var contentPage1 = new ContentPage
				{
					Title = title,
					Content = layout
				};

				Shell.SetBackgroundColor(contentPage1, Color.Red);
				Shell.SetForegroundColor(contentPage1, Color.Yellow);
				Shell.SetTabBarBackgroundColor(contentPage1, Color.Red);
				Shell.SetTabBarForegroundColor(contentPage1, Color.Yellow);

				return contentPage1;
			}

			static ContentPage CreatePage2(string title)
			{
				var layout = new StackLayout();

				var instructions = new Label
				{
					Padding = 12,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					Text = "If the NavigationBar is Green, the test has passed."
				};

				layout.Children.Add(instructions);

				var contentPage2 = new ContentPage
				{
					Title = title,
					Content = layout
				};

				Shell.SetBackgroundColor(contentPage2, Color.Green);
				Shell.SetForegroundColor(contentPage2, Color.Red);
				Shell.SetTabBarBackgroundColor(contentPage2, Color.Green);
				Shell.SetTabBarForegroundColor(contentPage2, Color.Red);

				return contentPage2;
			}
		}
	}
}