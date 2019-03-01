using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(Core.UITests.UITestCategories.Entry)]
#endif
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 5202, "Entry.Focus() throws ObjectDisposedException", PlatformAffected.Android)]
	public class Issue5202 : TestContentPage
	{
		protected override void Init()
		{
			var layout = new StackLayout();
			var entry = new Entry();
			var button = new Button { Text = "Click me" };
			button.Clicked += (_, __) =>
			{
				layout.Children.Clear();
				layout.Children.Add(entry);
				layout.Children.Add(button);
				entry.Focus();
			};

			layout.Children.Add(entry);
			layout.Children.Add(button);
			Content = layout;
		}
#if UITEST
		[Test]
		public void Issue5202Test()
		{
			RunningApp.WaitForElement("Click me");
			RunningApp.Tap("Click me");
		}
#endif
	}
}
