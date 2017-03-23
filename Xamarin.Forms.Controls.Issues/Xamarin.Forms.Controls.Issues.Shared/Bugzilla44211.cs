using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.ComponentModel;

#if UITEST
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 44211, "SelectedPage's Appearing called twice upon opening a TabbedPage in AppCompat")]
	public class Bugzilla44211 : TestTabbedPage
	{
		class CountPage : ContentPage, INotifyPropertyChanged
		{
			public int AppearingCount { get; set; } = 0;
			public CountPage()
			{
				var countLabel = new Label
				{
					AutomationId = "AppearingCountLabel"
				};
				countLabel.BindingContext = this;
				countLabel.SetBinding(Label.TextProperty, "AppearingCount");

				Appearing += (s, e) =>
				{
					AppearingCount += 1;
					OnPropertyChanged("AppearingCount");
				};
				Content = new StackLayout
				{
					Children =
					{
						new Label { Text = "Times Appeared called (should only say 1 to start):" },
						countLabel
					}
				};
			}
		}

		protected override void Init()
		{
			var page1 = new CountPage { Title = "Page1" };
			var page2 = new CountPage { Title = "Page2" };
			var page3 = new CountPage { Title = "Page3" };

			Children.Add(page1);
			Children.Add(page2);
			Children.Add(page3);
			SelectedItem = page2;
		}

#if UITEST
		[Test]
		public void TabbedPageShouldOnlyFireAppearingOnce()
		{
			RunningApp.Screenshot("I am at the TabbedPage");
			var label = RunningApp.WaitForElement(q => q.Marked("AppearingCountLabel"));
			Assert.AreEqual("1", label[0].Text);
		}
#endif
	}
}