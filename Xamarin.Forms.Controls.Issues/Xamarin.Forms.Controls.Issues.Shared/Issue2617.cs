using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Collections.Generic;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2617, "Error on binding ListView with duplicated items", PlatformAffected.UWP)]
	public class Issue2617 : TestContentPage
	{
		class MyHeaderViewCell : ViewCell
		{
			public MyHeaderViewCell()
			{
				Height = 25;
				var label = new Label { VerticalOptions = LayoutOptions.Center };
				label.SetBinding(Label.TextProperty, nameof(GroupedItem.Name));
				View = label;
			}
		}

		class GroupedItem: List<string>
		{
			public GroupedItem()
			{
				AddRange(Enumerable.Range(0, 3).Select(i => "Group item"));
			}
			public string Name { get; set; }
		}

		protected override void Init()
		{
			var listView = new ListView
			{
				ItemsSource = Enumerable.Range(0, 3).Select(x => "Item 1"),
				ItemTemplate = new DataTemplate(() =>
				{
					Label nameLabel = new Label();
					nameLabel.SetBinding(Label.TextProperty, new Binding("."));
					var cell = new ViewCell
					{
						View = new Frame()
						{
							Content = nameLabel
						},
					};
					return cell;
				})
			};
			var listViewIsGrooped = new ListView
			{
				ItemsSource = Enumerable.Range(0, 3).Select(x => new GroupedItem() { Name = $"Group {x}" }),
				IsGroupingEnabled = true,
				GroupHeaderTemplate = new DataTemplate(typeof(MyHeaderViewCell)),
				ItemTemplate = new DataTemplate(() =>
				{
					Label nameLabel = new Label();
					nameLabel.SetBinding(Label.TextProperty, new Binding("."));
					var cell = new ViewCell
					{
						View = new Frame()
						{
							Content = nameLabel
						},
					};
					return cell;
				})
			};
			Content = new StackLayout
			{
				Children =
				{
					new Button()
					{
						Text = "Send one million same items to ItemsSource",
						Command = new Command(() => listView.ItemsSource = Enumerable.Range(0, 1000000).Select(x => "Item 1"))
					},
					new Button()
					{
						Text = "Clear ItemsSource",
						Command = new Command(() => listView.ItemsSource = null)
					},
					listView,
					listViewIsGrooped
				}
			};
		}
	}
}