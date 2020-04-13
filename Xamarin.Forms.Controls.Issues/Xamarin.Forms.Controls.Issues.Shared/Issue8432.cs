using System;
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
	[Issue(IssueTracker.Github, 8432, "WebView - possibility to set custom user agent", PlatformAffected.All)]
	public class Issue8432 : TestContentPage
	{
		protected override void Init()
		{
			string UAString = "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko XAMARIN Rocks";

			Label header = new Label
			{
				Text = $"A Webview of www.WhatIsMyBrowser.com and the UserAgent - {UAString} ",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center
			};

			WebView webView = new WebView
			{
				Source = "https://www.whatismybrowser.com/detect/what-http-headers-is-my-browser-sending",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				UserAgentString = UAString
			};

			Content = new StackLayout
			{
				Padding = new Thickness(20),
				Children =
				{
					header,
					webView
				}
			};
		}
	}
}