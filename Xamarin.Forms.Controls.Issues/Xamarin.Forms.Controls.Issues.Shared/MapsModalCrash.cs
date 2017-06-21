using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Maps;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 1701, "Modal Page over Map crashes application", PlatformAffected.Android)]
	public class MapsModalCrash : TestContentPage 
	{
		protected override void Init()
		{
			var button = new Button { Text = "Start Test" };
			button.Clicked += (sender, args) =>
			{
				Application.Current.MainPage = MapPage();
			};

			Content = new StackLayout
			{
				Padding = new Thickness(0, 20, 0, 0),
				Children =
				{
					button
				}
			};
		}

		static ContentPage MapPage()
		{
			var map = new Map();

			var button = new Button { Text = "Click Me" };
			button.Clicked += (sender, args) => button.Navigation.PushModalAsync(new NavigationPage(SuccessPage()));

			return new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						map,
						button
					}
				}
			};
		}

		static ContentPage SuccessPage()
		{
			return new ContentPage
			{
				Content = new Label { Text = "If you're seeing this, then the test was a success.", AutomationId = "SuccessLabel" }
			};
		}

#if UITEST
		//[Test]
		//public void _$BZ$Test()
		//{
		//	//RunningApp.WaitForElement(q => q.Marked(""));
		//}
#endif
	}
}