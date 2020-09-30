﻿using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3139, "DisplayActionSheet is hiding behind Dialogs", PlatformAffected.UWP)]
	public class Issue3139 : TestContentPage
	{
		protected override async void Init()
		{
			var statusLabel = new Label()
			{
				FontSize = 40,
				TextColor = Color.White
			};
			Content = new StackLayout()
			{
				Children = {
					statusLabel,
					new Label {
						Text = "Pop-ups should appear on top of the dialog. And it's got any button pressed.",
						TextColor = Color.Yellow
					}
				}
			};

			var alertTask = DisplayAlert("AlertDialog", "Close me", "Close");
			await Task.Delay(200);
			var result1 = await DisplayActionSheet("ActionSheet", "Also Yes", "Click Yes", "Yes", "Yes Yes") ?? string.Empty;
			var result2 = await Application.Current.MainPage.DisplayActionSheet("Main page ActionSheet", "Again Yes", "Click Yes", "Yes", "Yes Yes") ?? string.Empty;
			var testPassed = result1.Contains("Yes") && result2.Contains("Yes") && !alertTask.IsCompleted;
			statusLabel.Text = "Test " + (testPassed ? "passed" : "failed");
			BackgroundColor = !testPassed ? Color.DarkRed : Color.DarkGreen;
			await alertTask;
		}

#if UITEST && __WINDOWS__
		[Test]
		public void Issue3139Test ()
		{
			RunningApp.WaitForElement (q => q.Marked ("Click Yes"));
			RunningApp.Tap (c => c.Marked ("Yes"));
			RunningApp.WaitForElement (q => q.Marked ("Again Yes"));
			RunningApp.Tap (c => c.Marked ("Yes"));
			RunningApp.WaitForElement(q => q.Marked("Test passed"));
		}
#endif
	}
}