using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13326, "[Bug] [Android] WebView.EvaluateJavascriptAsync thrown NullReferenceException when is offscreen in Shell", PlatformAffected.Android)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Shell)]
#endif
	public class Issue13326 : TestShell
	{
		const string Test1 = "Tab 1";
		const string Test2 = "Tab 2";

		protected override void Init()
		{
			AddBottomTab(CreatePage1(Test1), Test1);
			AddBottomTab(CreatePage2(Test2), Test2);

			static ContentPage CreatePage1(string title)
			{
				var layout = new StackLayout();

				var instructions = new Label
				{
					Padding = 12,
					BackgroundColor = Color.Black,
					TextColor = Color.White,
					Text = "Navigate to the second Tab"
				};

				layout.Children.Add(instructions);

				return new ContentPage
				{
					Title = title,
					Content = layout
				};
			}

			static ContentPage CreatePage2(string title)
			{
				return new Issue13326SecondPage(title);
			}
		}
	}

	public class Issue13326SecondPage : ContentPage
	{
		WebView _webView;

		public Issue13326SecondPage(string title)
		{
			Title = title;

			var layout = new StackLayout();

			var instructions = new Label
			{
				Padding = 12,
				BackgroundColor = Color.Black,
				TextColor = Color.White,
				Text = "Navigate back to the first tab and wait some seconds, without crashing the test has passed."
			};

			_webView = new WebView
			{
				HeightRequest = 300,
				WidthRequest = 300
			};

			layout.Children.Add(instructions);
			layout.Children.Add(_webView);

			Content = layout;

			Task.Run(async () =>
			{
				await Task.Delay(TimeSpan.FromSeconds(5));

				await _webView.EvaluateJavaScriptAsync("var a = 10;");
			});
		}
	}
}