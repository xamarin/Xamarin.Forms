using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1469, "Setting SelectedItem to null inside ItemSelected event handler does not work", PlatformAffected.UWP)]
	public class Issue1469 : TestContentPage
	{
		protected override void Init()
		{
			var statusLabel = new Label();
			var _items = Enumerable.Range(1, 4).Select(i => $"Item {i}").ToArray();
			var list = new ListView()
			{
				ItemsSource = _items
			};
			list.ItemSelected += (_, e) =>
			{
				if (e.SelectedItem == null)
				{
					statusLabel.Text = "Success"; // last selected item must be null
					return;
				}
				statusLabel.Text = "Fail";
				list.SelectedItem = null;
			};
			Content = new StackLayout
			{
				Children = {
					statusLabel,
					new Button { Text = "Select 3rd item", Command = new Command(() => list.SelectedItem = _items[2]) },
					new Button { Text = "Clear selection", Command = new Command(() => list.SelectedItem = list.SelectedItem = null) },
					list
				}
			};
		}
	}
}