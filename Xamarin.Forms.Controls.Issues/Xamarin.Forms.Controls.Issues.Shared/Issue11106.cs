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
	[Category(UITestCategories.ScrollView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11106,
		"[Bug] ScrollView UWP bug in 4.7.0.968!",
		PlatformAffected.UWP)]
	public class Issue11106 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Issue 11106";

			var scroll = new ScrollView();

			var layout = new StackLayout();

			for (int i = 0; i < 30; i++)
			{
				layout.Children.Add(new Entry());
			}

			scroll.Content = layout;

			Content = scroll;
		}
	}
}