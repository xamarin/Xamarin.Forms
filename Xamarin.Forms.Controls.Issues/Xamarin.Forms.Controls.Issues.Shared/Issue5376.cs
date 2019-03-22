using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading.Tasks;
using System.Threading;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.LifeCycle)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5376, "Call unfocus entry crashes app", PlatformAffected.Android)]
	public class Issue5376 : TestMasterDetailPage
	{
		protected async override void Init()
		{
			MasterBehavior = MasterBehavior.Popover;
			IsPresented = false;
			Master = new ContentPage { Title = "test 5376" };
			var testPage = new NavigationPage(new EntryPage() { Title = $"Test page" });
			Detail = testPage;
			await Task.Delay(100);
			Detail = new ContentPage();
			await Task.Delay(100);
			Detail = testPage;
			Detail = new ContentPage { Content = new Label { Text = "Success" } };
		}

		[Preserve(AllMembers = true)]
		class EntryPage : ContentPage
		{
			Entry entry;

			public EntryPage()
			{
				entry = new Entry { Text = Title };
				Content = new StackLayout { Children = { entry } };
			}

			protected override void OnAppearing()
			{
				base.OnAppearing();

				new Thread(() =>
				{
					var success = false;
					while (!success)
					{
						Thread.Sleep(1000);
						Device.BeginInvokeOnMainThread(() =>
						{
							if (entry.IsFocused)
							{
								entry.Unfocus();
								success = true;
							}
							else
							{
								entry.Focus();
							}
						});
					}
				}).Start();
			}
		}

#if UITEST
		[Test]
		public void Issue5376Test() 
		{
			RunningApp.WaitForElement ("Success");
		}
#endif
	}
}