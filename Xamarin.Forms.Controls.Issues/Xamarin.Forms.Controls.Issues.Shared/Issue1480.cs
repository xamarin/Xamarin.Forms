using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Threading.Tasks;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1480, "[iOS] Change CALayer.Transform in MainThread", PlatformAffected.iOS)]
	public class Issue1480 : TestContentPage
	{
		protected override void Init()
		{
			Content = new ContentView();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			var checkWebView = new WebView();
			var checkPage = new ContentPage {
				Content = checkWebView
			};
			checkPage.Appearing += (s, e) => checkWebView.TranslateTo(0, 100);
			Navigation.PushModalAsync(checkPage);
		}

#if UITEST

#endif
	}
}