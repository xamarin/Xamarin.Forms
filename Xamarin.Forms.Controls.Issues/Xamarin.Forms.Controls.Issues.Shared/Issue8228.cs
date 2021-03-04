using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest.iOS;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8228, "[Bug]Grouped CollectionView shows the item source object name in each group", PlatformAffected.iOS)]
	public class Issue8228 : TestContentPage
	{
		protected override void Init()
		{
			Title = "Grouped CollectionView";
			var collectionItems = new ObservableCollection<CategoryGroup>
			{
				new CategoryGroup("GroupHeader1", "GroupFooter1", new ObservableCollection<Category>
				{
					new Category { Name = "Item1" },
					new Category { Name = "Item2" },
					new Category { Name = "Item3" }
				}),
				new CategoryGroup("GroupHeader2", "GroupFooter2", new ObservableCollection<Category>
				{
					new Category { Name = "Item1" },
					new Category { Name = "Item2" },
					new Category { Name = "Item3" }
				})
			};
			Content = new StackLayout
			{
				Children =
				{
					new Button
					{
						Text = "Add Empty Header and Footer group",
						Command = new Command((parameter) => {
							collectionItems.Add(new CategoryGroup("", "", new ObservableCollection<Category>
							{
								new Category { Name = "Item1" },
								new Category { Name = "Item2" },
								new Category { Name = "Item3" }
							}));
						})
					},
					new CollectionView
					{
						ItemsLayout = LinearItemsLayout.Vertical,
						IsGrouped = true,
						GroupHeaderTemplate = new DataTemplate(() =>
						{
							var nameLabel = new Label
							{
								TextColor = Color.Green
							};
							nameLabel.SetBinding(Label.TextProperty, "GroupHeaderName");
							return nameLabel;
						}),
						GroupFooterTemplate = new DataTemplate(() =>
						{
							var nameLabel = new Label
							{
								TextColor = Color.Red
							};
							nameLabel.SetBinding(Label.TextProperty, "GroupFooterName");
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
						ItemsSource = collectionItems
					}
				}
			};
		}

#if UITEST
		[Test]
		public void EmptyGroupHeaderTest()
		{
			RunningApp.WaitForElement(q => q.Button("Add Empty Header and Footer group"));
			RunningApp.Tap(q => q.Button("Add Empty Header and Footer group"));
			RunningApp.WaitForNoElement(q => q.Marked(nameof(CategoryGroup)));
		}
#endif
	}

	public class Category
	{
		public string Name { get; set; }
	}

	public class CategoryGroup : List<Category>
	{
		public string GroupHeaderName { get; set; }
		public string GroupFooterName { get; set; }

		public CategoryGroup(string groupHeaderName, string groupFooterName, ObservableCollection<Category> categories) : base(categories)
		{
			GroupHeaderName = groupHeaderName;
			GroupFooterName = groupFooterName;
		}
	}
}
