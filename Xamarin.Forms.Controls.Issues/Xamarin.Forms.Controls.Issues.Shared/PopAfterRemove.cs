using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	//[Category(UITestCategories.)]
#endif

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 999999, "PopAsync crashing after RemovePage when support packages are updated to 25.1.1", PlatformAffected.Android)]
	public class Bugzilla999999 : TestNavigationPage
	{
		ContentPage _intermediate1;
		ContentPage _intermediate2;

		protected override async void Init()
		{
			_intermediate1 = Intermediate();
			_intermediate2 = Intermediate();

			await PushAsync(Root());
			await PushAsync(_intermediate1);
			await PushAsync(_intermediate2);
			await PushAsync(Last());
		}

		const string StartTest = "Start Test";

		ContentPage Last()
		{
			var test = new Button { Text = StartTest };

			var layout = new StackLayout();

			layout.Children.Add(new Label{Text = "Last"});
			layout.Children.Add(test);

			test.Clicked += (sender, args) =>
			{
				// TODO hartez 2017/07/11 20:51:46 Need to check both orders for intermediate pages, and also animated/not	
				Navigation.RemovePage(_intermediate1);
				Navigation.RemovePage(_intermediate2);
				Navigation.PopAsync();
			};

			return new ContentPage { Content = layout };
		}

		static ContentPage Root()
		{
			return new ContentPage { Content = new Label { Text = "Root" } };
		}

		static ContentPage Intermediate()
		{
			return new ContentPage { Content = new Label { Text = "Page" } };
		}

#if UITEST
		//[Test]
		//public void _999999Test()
		//{
		//	//RunningApp.WaitForElement(q => q.Marked(""));
		//}
#endif
	}
}