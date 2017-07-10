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
	[Issue(IssueTracker.Bugzilla, 945722, "", PlatformAffected.All)]
	public class Bugzilla45722 : TestContentPage 
	{
		protected override void Init()
		{
			var button = new Button() { Text = "Update List" };

			var lv = new ListView();

			var items = new List<string>() {
			};

			var dt = new DataTemplate(() => new ViewCell { View = new Label { Text = "Hey, it's a label." } });

			

			Content = new StackLayout
			{
				Padding = new Thickness(0, 20, 0, 0),
				Children =
				{
					button, lv
				}
			};
		}

#if UITEST
		[Test]
		public void _45722Test()
		{
			//RunningApp.WaitForElement(q => q.Marked(""));
		}
#endif
	}
}