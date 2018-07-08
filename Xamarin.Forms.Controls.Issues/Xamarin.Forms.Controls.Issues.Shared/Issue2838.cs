using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2838, "UWP does not render Frame CornerRadius", PlatformAffected.UWP)]
	public class Issue2838 : TestContentPage
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children =
				{
					new Label()
					{
						Text ="The frame below should have its corners rounded and the background should not protrude through them.",
						TextColor = Color.Black,
						WidthRequest = 300,
						LineBreakMode = LineBreakMode.WordWrap,
						HorizontalOptions = LayoutOptions.Center,
						Margin = new Thickness(20)
					},
					new Frame
					{
						WidthRequest = 300,
						HeightRequest = 160,
						HorizontalOptions = LayoutOptions.Center,
						CornerRadius = 10,
						BackgroundColor = Color.Red,
						BorderColor = Color.Blue
					}
				}
			};
		}

#if UITEST
		[Test]
		public void Issue1Test ()
		{
			RunningApp.Screenshot ("I am at Issue 1");
			RunningApp.WaitForElement (q => q.Marked ("IssuePageLabel"));
			RunningApp.Screenshot ("I see the Label");
		}
#endif
	}
}