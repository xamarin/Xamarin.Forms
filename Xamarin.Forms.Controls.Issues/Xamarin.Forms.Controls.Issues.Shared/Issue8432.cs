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
			Label header = new Label
			{
				Text = "A Webview should open below and display www.WhatIsMyBrowser.com and the UserAgent that is set here.",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.Center
			};

			WebView webView = new WebView
			{
				Source = "https://www.whatismybrowser.com/detect/what-http-headers-is-my-browser-sending",
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				setUserAgentString = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705 CLIFF"
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