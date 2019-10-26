using System;
using System.Collections.Generic;

using Xamarin.Forms.CustomAttributes;

using Xamarin.Forms;
using Xamarin.Forms.Internals;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues.TestPages
{


	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6048, "OnPlatform padding", PlatformAffected.All, NavigationBehavior.PushAsync)]
	public partial class Issue6048 : ContentPage
	{
		const string DialogTitle = "Oooops... 😥";
		const string ConfirmButton = "OK";
#if APP

        public Issue6048()
        {
			try
			{
				InitializeComponent();
			}
			catch (FormatException ex)
			{
				DisplayAlert(DialogTitle, "An exception was throw", ConfirmButton);
			}
        }
#endif

#if UITEST
		[SetUp]
		public void Setup()
		{
			AppSetup.BeginIsolate();
			AppSetup.NavigateToIssue(GetType(), AppSetup.RunningApp);
		}

		[Test]
		public void OnPlatformPaddingTests()
		{
			var app = AppSetup.RunningApp;
			var dialog = app.WaitForElement(DialogTitle)[0];

			Assert.AreEqual("alertTitle", dialog.Id);

			app.Tap(ConfirmButton);
		}
#endif
	}
}