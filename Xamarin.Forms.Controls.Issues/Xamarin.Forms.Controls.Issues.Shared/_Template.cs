using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 1, "Issue Description", PlatformAffected.Default)]
	public class Bugzilla1 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "IssuePageLabel",
				Text = "See if I'm here"
			};
		}

#if UITEST
		[Test]
		public void Issue1Test ()
		{
			RunningApp.Screenshot ("I am at Issue 1");
			RunningApp.WaitForElement (q => q.Marked ("IssuePageLabel"));
			RunningApp.Screenshot ("I see the Label");
		}
#endif
	}

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1396, 
		"Label TextAlignment is not kept when resuming application", 
		PlatformAffected.Android)]
	public class Issue1396Vertical : TestContentPage
	{
		Label _label;

		protected override void Init()
		{
			var instructions = new Label
			{
				Text = "Tap the 'Change Text' button. Tap the Overview button. Resume the application. If the label" 
						+ " text is no longer centered, the test has failed."
			};

			var button = new Button { Text = "Change Text" };
			button.Clicked += (sender, args) =>
			{
				// TODO hartez 2018/03/29 09:34:21 We need to test this if it wraps 	

				_label.Text = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
			};

			_label = new Label
			{
				HeightRequest = 400,
				BackgroundColor = Color.Gold,
				Text = "I should be centered in the gold area",
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var layout = new StackLayout 
			{
				Children = { instructions, button, _label }
			};

			var content = new ContentPage 
			{
				Content = layout 
			};

			Content = new Label { Text = "Shouldn't see this" };

			Appearing += (sender, args) => Application.Current.MainPage = content;
		}
	}
}