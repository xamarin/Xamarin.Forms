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
	[Category(UITestCategories.Navigation)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14380, "[Bug] Setting the Background property of a NavigationPage does nothing on iOS",
		PlatformAffected.iOS)]
	public class Issue14380 : TestNavigationPage
	{
		public Issue14380()
		{
			Background = new LinearGradientBrush
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1, 0),
				GradientStops = new GradientStopCollection
				{
					new GradientStop { Color = Color.GreenYellow, Offset = 0.0f },
					new GradientStop { Color = Color.DarkGreen, Offset = 0.9f },
				}
			};
		}

		protected override void Init()
		{
#if APP
			PushAsync(new Issue14380SecondPage());
#endif
		}
	}

	public class Issue14380SecondPage : ContentPage
	{
		public Issue14380SecondPage()
		{
			Title = "Issue 14380";
			BackgroundColor = Color.Transparent;
		}
	}
}
