using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
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
	}
}
