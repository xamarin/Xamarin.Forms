using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44777, "BarTextColor changes color for more than just the Navigation page")]
	public class Bugzilla44777 : TestMasterDetailPage
	{
		protected override void Init()
		{
			Master = new ContentPage() { Title = "I am a master page" };
			Detail = new NavigationPage(new ContentPage());
			((NavigationPage)Detail).BarBackgroundColor = Color.Blue;
			((NavigationPage)Detail).BarTextColor = Color.White;

			IsPresentedChanged += (sender, e) =>
			{
				var mp = sender as MasterDetailPage;
				if (mp.IsPresented)
					((NavigationPage)mp.Detail).On<iOS>().SetStatusBarTextColorMode(StatusBarTextColorMode.DoNotAdjust);
				else
					((NavigationPage)mp.Detail).On<iOS>().SetStatusBarTextColorMode(StatusBarTextColorMode.MatchNavigationBarTextLuminosity);
			};
		}
	}
}