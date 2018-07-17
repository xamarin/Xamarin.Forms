using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 3273, "Drag and drop reordering not firing CollectionChanged", PlatformAffected.UWP)]
	public class Issue3273 : TestContentPage
	{
		protected override void Init ()
		{
			var Items = new ObservableCollection<string>
			{
				"apple",
				"orange",
				"pear",
				"trash"
			};
			BindingContext = Items;

			Items.CollectionChanged += (_, e) => System.Diagnostics.Debug.WriteLine($"{e.Action} action fired.");
			Items.RemoveAt(3);
			Items.Move(0, 1);

			var listView = new ListView();
			listView.SetBinding(ListView.ItemsSourceProperty, ".");

			Content = new StackLayout
			{
				Children = {
					new Button {
						Text = "Move items",
						Command = new Command(() => Items.Move(0, 1))
					},
					listView
				}
			};
		}
	}
}
