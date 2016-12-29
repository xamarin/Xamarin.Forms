using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40251, "Cannot style Buttons natively using UIButton.Appearance", PlatformAffected.iOS)]
	public class Bugzilla40251 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			Content = new ContentView
			{
				Content = new Button
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Text = "Click",
					BackgroundColor = Color.Black,
					WidthRequest = 250,
					HeightRequest = 50
				}
			};
		}
	}
}