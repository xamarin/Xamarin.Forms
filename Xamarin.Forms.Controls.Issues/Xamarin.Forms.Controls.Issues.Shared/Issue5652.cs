using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5652, "ScrollView Jumps when Non-Interactable Element is Clicked", PlatformAffected.UWP)]
	public class Issue5652: TestContentPage
	{
		protected override void Init()
		{
			var entry = new Entry { Placeholder = "Should not jump here" };

			var button = new Button
			{
				Text = "Click around"
			};

			var longStackLayout = new StackLayout();

			longStackLayout.Children.Add(entry);

			Enumerable.Range(1, 50).Select(i => new Label { Text = $"Test label {i}" })
				.ForEach(label => longStackLayout.Children.Add(label));

			longStackLayout.Children.Add(button);

			_scrollView = new ScrollView
			{
				Content = longStackLayout
			};

			Content = _scrollView;
		}

		ScrollView _scrollView;
	}
}