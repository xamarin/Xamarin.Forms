using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
#if UITEST
using NUnit.Framework;
using Xamarin.UITest;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11642, "Android - Grouped CollectionView - Removing all items from a large list and adding them back to the group renders oddly", PlatformAffected.Android)]
	public class Issue11642 : TestContentPage
	{
		public ObservableCollection<Issue11642Group> TestItemSource { get; set; }
		public Issue11642Group BackingGroup { get; set; }

		protected override void Init()
		{
			TestItemSource = GenerateCollection();

			var collectionViewHeader = new StackLayout();
			var toggleButton = new Button() { Text = "Toggle First Group Items", Command = new Command(ToggleItemsInFirstGroup) };
			collectionViewHeader.Children.Add(toggleButton);

			var collectionItemTemplate = new StackLayout();
			collectionItemTemplate.Children.Add(new Label() { Text = "NOOOO" });

			var collectionView = new CollectionView();
			collectionView.Header = collectionViewHeader;
			collectionView.ItemsSource = TestItemSource;
			collectionView.IsGrouped = true;
			collectionView.GroupHeaderTemplate = new DataTemplate(() => new Label() { Text = "Group Name" });
			collectionView.ItemTemplate = new DataTemplate(() => new StackLayout() { HeightRequest = 40, Padding = 10, BackgroundColor = Color.Red });

			Content = collectionView;
		}

		public void ToggleItemsInFirstGroup()
		{
			if (TestItemSource[0].Count != 0)
				TestItemSource[0].Clear();
			else
				TestItemSource[0] = GenerateGroup();
		}

		public Issue11642Group GenerateGroup()
		{
			return new Issue11642Group("List 1", new List<Issue11642TestItem>()
				{
					new Issue11642TestItem(),
					new Issue11642TestItem(),
					new Issue11642TestItem(),
					new Issue11642TestItem(),
					new Issue11642TestItem(),
				});
		}

		public ObservableCollection<Issue11642Group> GenerateCollection()
		{
			return new ObservableCollection<Issue11642Group>()
			{
				new Issue11642Group("List 1", new List<Issue11642TestItem>()
				{
					new Issue11642TestItem(),
					new Issue11642TestItem(),
					new Issue11642TestItem(),
					new Issue11642TestItem(),
					new Issue11642TestItem(),
				}),
				new Issue11642Group("List 2", new List<Issue11642TestItem>()
				{
					new Issue11642TestItem(),
					new Issue11642TestItem(),
				})
			};
		}
	}

	public class Issue11642Group : ObservableCollection<Issue11642TestItem>
	{
		public string GroupName { get; set; }

		public Issue11642Group(string groupName, List<Issue11642TestItem> items) : base(items)
		{
			GroupName = groupName;
		}
	}

	public class Issue11642TestItem
	{
		public string ItemName { get; set; }
	}

}