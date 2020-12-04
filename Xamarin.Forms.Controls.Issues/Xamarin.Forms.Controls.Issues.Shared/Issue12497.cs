using System;
using System.Threading.Tasks;
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
	[Category(UITestCategories.WebView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12497, "Calling Focus() on a WebView on UWP does not move the focus to the WebView", PlatformAffected.UWP)]
	public class Issue12497 : TestContentPage
	{
		bool _firstTimeOnAppearing = true;
		Button _button1;
		Button _button2;
		Button _button3;
		WebView _webView;
		Label _label;

		const string FAIL = "Fail";
		const string SUCCESS = "Success";
		const string btnAutomationId = "btn3";

		protected override void Init()
		{
			PopulatePage();
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (_firstTimeOnAppearing)
			{
				_firstTimeOnAppearing = false;

				//PopulatePage();

				// The following is just here as a convenience.
				// It's a nasty, hacky way of setting the initial focus, 
				// that introduces a race condition. Good enough for
				// this repro sample though.
				Task.Run(async () =>
				{
					await Task.Delay(1000);
					if (_button1 != null)
						_button1.Focus();

				});
			}
		}

		void PopulatePage()
		{
			int defaultTabIndex = Int32.MaxValue; // UWP says this should be the max int. Xamarin.Forms docs say it should be 0;

			_label = new Label
			{
				Text = FAIL
			};

			_button1 = new Button
			{
				Text = "Button 1 (should be first in tab order)",
				TextColor = Color.Black,
				BackgroundColor = Color.White,
				TabIndex = defaultTabIndex,
				VerticalOptions = LayoutOptions.Start
			};

			_webView = new WebView
			{
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.Fill,
				TabIndex = defaultTabIndex,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Source = new HtmlWebViewSource
				{
					Html = "<html><body>Hello (should be second in tab order)<br>1<br>2<br>3<br>4<br>5<br>6<br>7<br>8<br>9<br>10<br>11<br>12<br>13<br>14<br>15<br>16<br>17<br>18<br>19<br>20<br>21<br>22<br>23<br>24<br>25<br>26<br>27<br>28<br>29</body></html>"
				}
			};

			_webView.Focused += (s, e) =>
			{
				_label.Text = SUCCESS;
			};

			_button2 = new Button
			{
				Text = "Button 2 (should be third in tab order)",
				TextColor = Color.Black,
				BackgroundColor = Color.White,
				TabIndex = defaultTabIndex,
				VerticalOptions = LayoutOptions.End
			};

			_button3 = new Button
			{
				AutomationId = btnAutomationId,
				Text = "Button 3 (press to set focus to WebView)",
				TextColor = Color.Black,
				BackgroundColor = Color.White,
				TabIndex = defaultTabIndex,
				VerticalOptions = LayoutOptions.End,
				Command = new Command(() =>
				{
					_webView.Focus(); // _button1.Focus();
				})
			};

			Content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Pink,
				Children =
				{
					_button1,
					_webView,
					_button2,
					_button3,
					_label
				}
			};
		}

#if UITEST
		[Test]
		public void WebviewFocusIsFiring()
		{
			RunningApp.Tap(btnAutomationId);
			RunningApp.WaitForElement(SUCCESS);
		}
#endif
	}
}