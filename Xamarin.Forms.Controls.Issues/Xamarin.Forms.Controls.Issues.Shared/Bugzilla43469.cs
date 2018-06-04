﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 43469, "Calling DisplayAlert twice in WinRT causes a crash", PlatformAffected.WinRT)]
	public class Bugzilla43469 : TestContentPage
	{
		const string kButtonText = "Click to call DisplayAlert six times";
		protected override void Init()
		{
			var button = new Button { Text = "Click to call DisplayAlert six times" };

			button.Clicked += async (sender, args) =>
			{
				await DisplayAlert("First", "Text", "OK", "Cancel");
				await DisplayAlert("Second", "Text", "OK", "Cancel");
				await DisplayAlert("Three", "Text", "OK", "Cancel");
				Device.BeginInvokeOnMainThread(new Action(async () =>
				{
					await DisplayAlert("Fourth", "Text", "OK", "Cancel");
				}));

				Device.BeginInvokeOnMainThread(new Action(async () =>
				{
					await DisplayAlert("Fifth", "Text", "OK", "Cancel");
				}));

				Device.BeginInvokeOnMainThread(new Action(async () =>
				{
					await DisplayAlert("Sixth", "Text", "OK", "Cancel");
				}));
			};

			Content = button;
		}


#if UITEST

		[Test]
		public async void Bugzilla43469Test()
		{
			RunningApp.WaitForElement(q => q.Marked(kButtonText));
			RunningApp.Tap(kButtonText);
			RunningApp.WaitForElement(q => q.Marked("First"));
			RunningApp.Tap(q => q.Marked("OK"));
			RunningApp.WaitForElement(q => q.Marked("Second"));
			RunningApp.Tap(q => q.Marked("OK"));
			RunningApp.WaitForElement(q => q.Marked("Three"));
			RunningApp.Tap(q => q.Marked("OK"));


			await Task.Delay(100);
			RunningApp.Tap(q => q.Marked("OK"));
			await Task.Delay(100);
			RunningApp.Tap(q => q.Marked("OK"));
			await Task.Delay(100);
			RunningApp.Tap(q => q.Marked("OK"));
			await Task.Delay(100);
			RunningApp.WaitForElement(q => q.Marked(kButtonText));
		}
#endif

	}
}