using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8228, "[Bug]Grouped CollectionView shows the item source object name in each group", PlatformAffected.iOS)]
	public class Issue8228 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Grouped CollectionView";
			Content = new CollectionView
			{
				ItemsLayout = LinearItemsLayout.Vertical,
				IsGrouped = true,
				GroupHeaderTemplate = new DataTemplate(() =>
				{
					var nameLabel = new Label
					{
						TextColor = Color.Green
					};
					nameLabel.SetBinding(Label.TextProperty, "GroupName");
					return nameLabel;
				}),
				ItemTemplate = new DataTemplate(() =>
				{
					var nameLabel = new Label
					{
						Padding = new Thickness(20)
					};
					nameLabel.SetBinding(Label.TextProperty, "Name");
					return nameLabel;
				}),
				ItemsSource = new List<CategoryGroup>
				{
					new CategoryGroup("Group1", new ObservableCollection<Category>
					{
						new Category { Name = "Item1" },
						new Category { Name = "Item2" },
						new Category { Name = "Item3" }
					}),
					new CategoryGroup("Group2", new ObservableCollection<Category>
					{
						new Category { Name = "Item1" },
						new Category { Name = "Item2" },
						new Category { Name = "Item3" }
					})
				}
			};
		}
	}

	public class Category
	{
		public string Name { get; set; }
	}

	public class CategoryGroup : List<Category>
	{
		public string GroupName { get; set; }

		public CategoryGroup(string groupName, ObservableCollection<Category> categories) : base(categories)
		{
			GroupName = groupName;
		}
	}
}
