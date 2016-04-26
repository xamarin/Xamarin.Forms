using System;

using Xamarin.Forms.CustomAttributes;
using System.Linq;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 40502, "CarouselView NRE on MainPage change WinRT")]
	public class Bugzilla40502 : TestContentPage
	{
		protected override void Init()
		{
			Button newButton = new Button { Text = "click"};
			Content = new StackLayout {
				Children = {
					new CarouselView() { ItemsSource = Enumerable.Range(0, 10), ItemTemplate = new DataTemplate(typeof(Label)) },
					newButton
				}
			};
			newButton.Clicked += (sender, e) => {
				ContentPage newPage = new ContentPage() { Content = new Label { Text = "success" } };
				Application.Current.MainPage = newPage;
			};
		}


#if UITEST
		[Test]
		public void Bugzilla40502Test()
		{
			RunningApp.WaitForElement(q => q.Marked("click"));
			RunningApp.Tap(q => q.Marked("click"));
			RunningApp.WaitForElement(q => q.Marked("success"));
		}
#endif
	}
}
