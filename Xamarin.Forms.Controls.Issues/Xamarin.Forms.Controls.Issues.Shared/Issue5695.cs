using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5555, "Memory leak when SwitchCell or EntryCell", PlatformAffected.iOS)]
	public class Issue5695 : TestMasterDetailPage
    {
		[Preserve(AllMembers = true)]
		class LeakPage : ContentPage
		{
			public LeakPage()
			{
                ToolbarItems.Add(new ToolbarItem { Text = "Dummy" });
			}
		}

		protected override void Init ()
		{
			var wref = new WeakReference(null);

			var result = new Label { 
				FontSize = 16,			
				Text = "Click 'Push page'"
			};

			var checkResult = new Button
			{
				Text = "Check Result",
				IsEnabled = false,
				Command = new Command(() =>
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();

					result.Text = wref.IsAlive ? "Failed" : "Success";
				})
			};

			Detail = new NavigationPage(new LeakPage());

            Master = new ContentPage
            {
                Title = "menu",
			    Content = new StackLayout
                {
                    Children = {
                        result,
                        new Button
                        {
                            Text = "Push page",
                            Command = new Command(async() => {

                                await Detail.Navigation.PushAsync(new LeakPage());

								var pageToRemove = Detail.Navigation.NavigationStack[0];
								
								Detail.Navigation.RemovePage(pageToRemove);

								wref.Target = pageToRemove;

								checkResult.IsEnabled = true;
								result.Text = "You can check result";
							})
                        },
						checkResult
                    }
                }
			};
		}

#if UITEST
		[Test]
		public void Issue5695Test()
		{
			RunningApp.Tap(q => q.Marked("Push page"));
			RunningApp.WaitForElement(q => q.Marked("Push page"));

			RunningApp.WaitForElement(q => q.Marked("You can check result"));
			RunningApp.Tap(q => q.Marked("Check Result"));

			RunningApp.WaitForElement(q => q.Marked("Success"));
		}
#endif
	}
}