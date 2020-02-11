using System.Collections.Generic;
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
	[Issue(IssueTracker.Github, 9525, "MediaElement Disposing exception when MainPage is changed on iOS", PlatformAffected.iOS)]
	public class Issue9525 : TestNavigationPage
	{
		protected override void Init()
		{
#if APP
			Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { "MediaElement_Experimental" });

			PushAsync(CreateRoot());
#endif
		}
		private ContentPage CreateRoot()
		{ 
			var button = new Button
			{
				AutomationId = "Issue9525Button",
				Text = "Go to new page",
			};
			button.Clicked += Button_Clicked;
			return new ContentPage
			{
				Content = new StackLayout
				{
					Children =
					{
						new MediaElement
						{
							AutomationId = "Issue9525MediaElement",
							Source="https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
							HeightRequest=200,
						},
						button
					}
				}
			};
			
		}
		private void Button_Clicked(object sender, System.EventArgs e)
		{
			Navigation.InsertPageBefore(CreateRoot(), CurrentPage);
			PopAsync().Wait();
		}


#if UITEST
		[Test]
		public void Issue9525Test()
		{
			//Will be exeption if fail.
			RunningApp.Screenshot("I am at Issue9525");
			for (var i = 0; i < 10; i++)
			{
				RunningApp.WaitForElement(q => q.Marked("Issue9525Button"));
				RunningApp.Screenshot("I see the Button");
				RunningApp.Tap("Issue9525Button");
			}
		}
#endif
	}
}
