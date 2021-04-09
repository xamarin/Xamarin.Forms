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
			MyWebView.Navigating += OnNavigating;
			//MyWebView.Navigating += OnNavigatingAsync;
#endif
		}

		protected override void Init()
		{

		}

		private void OnNavigating(object sender, WebNavigatingEventArgs e)
		{
			bool shouldCancel = ShouldCancel();

			Debug.WriteLine($"Issue12720 - OnNavigating - Cancelling? {shouldCancel}");

			e.OldCancel = shouldCancel;
		}

		private async void OnNavigatingAsync(object sender, WebNavigatingEventArgs e)
		{
			Debug.WriteLine($"Issue12720 - OnNavigatingAsync - Grabbing deferral token and waiting a while");

			var token = e.GetDeferral();
			bool shouldCancel = await ShouldCancelAsync();

			Debug.WriteLine($"Issue12720 - OnNavigatingAsync - Cancelling? {shouldCancel}");

			token.Complete();
		}

		private bool ShouldCancel()
		{
			return false;
		}

		private async Task<bool> ShouldCancelAsync()
		{
			await Task.Delay(2000);

			return false;
		}
	}
}
