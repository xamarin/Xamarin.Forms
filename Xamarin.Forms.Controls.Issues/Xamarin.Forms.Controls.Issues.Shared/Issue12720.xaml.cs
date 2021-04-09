using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Diagnostics;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.WebView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12720, "WebView Navigating Does Not Support Async Cancellation", PlatformAffected.All)]
	public partial class Issue12720 : TestContentPage
	{
		public Issue12720()
		{
#if APP
			InitializeComponent();
			BindingContext = this;
			//MyWebView.Navigating += OnNavigatingWithoutDeferral;
			//MyWebView.Navigating += OnNavigatingWithDeferral;
			MyWebView.Navigating += OnNavigatingWithDeferralAsync;
#endif
		}

		protected override void Init()
		{

		}

		private void OnNavigatingWithoutDeferral(object sender, WebNavigatingEventArgs e)
		{
			Debug.WriteLine("Navigating - Not going to do anything with deferrals!");
		}

		private void OnNavigatingWithDeferral(object sender, WebNavigatingEventArgs e)
		{
			var token = e.GetDeferral();

			var shouldCancel = ShouldCancel(e.Url);

			if (shouldCancel)
			{
				Debug.WriteLine($"Navigation to url was cancelled: {e.Url}");
				e.Cancel();
			}

			token.Complete();
		}

		private async void OnNavigatingWithDeferralAsync(object sender, WebNavigatingEventArgs e)
		{
			Debug.WriteLine($"Issue12720 - OnNavigatingAsync - Grabbing deferral token and waiting a while");
			Debug.WriteLine(DateTime.Now.ToString("T"));

			var token = e.GetDeferral();

			bool shouldCancel = await ShouldCancelAsync(e.Url);

			Debug.WriteLine(DateTime.Now.ToString("T"));

			if (shouldCancel)
			{
				Debug.WriteLine($"Navigation to url was cancelled: {e.Url}");
				e.Cancel();
			}

			token.Complete();
		}

		private bool ShouldCancel(string url)
		{
			if (url.Contains("bbc.co.uk"))
				return true;

			return false;
		}

		int _delayMs = 0;

		private int GetDelay()
		{
			_delayMs = _delayMs + 1000;

			return _delayMs;
		}

		private async Task<bool> ShouldCancelAsync(string url)
		{
			if (url.Contains("bbc.co.uk"))
			{
				var delay = GetDelay();

				Debug.WriteLine($"Delaying for {delay}ms");
				await Task.Delay(delay);
			}

			return ShouldCancel(url);
		}
	}
}
