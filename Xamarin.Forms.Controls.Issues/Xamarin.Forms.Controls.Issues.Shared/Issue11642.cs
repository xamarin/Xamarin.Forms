using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11642, "Android - Grouped CollectionView - Removing all items from a large list and adding them back to the group renders oddly", PlatformAffected.Android)]
	public class Issue11642 : TestContentPage
	{
		public ObservableCollection<Issue11642Group> TestItemSource { get; set; }
		public Issue11642Group BackingGroup { get; set; }
		public DataTemplate TemplateOne { get; set; } = new DataTemplate(() => new StackLayout() { HeightRequest = 40, BackgroundColor = Color.Blue });
		public DataTemplate TemplateTwo { get; set; } = new DataTemplate(() => new StackLayout() { HeightRequest = 20, BackgroundColor = Color.Orange });
		public DataTemplate TemplateThree { get; set; } = new DataTemplate(() => new StackLayout() { HeightRequest = 60, BackgroundColor = Color.Green });

		protected override void Init()
		{
			TestItemSource = GenerateCollection();

			var collectionViewHeader = new StackLayout();
			var toggleButton = new Button() { Text = "Toggle First Group Items", Command = new Command(ToggleItemsInFirstGroup) };
			collectionViewHeader.Children.Add(toggleButton);

			var templateSelector = new Issue11642TemplateSelector();
			templateSelector.TemplateOne = TemplateOne;
			templateSelector.TemplateTwo = TemplateTwo;
			templateSelector.TemplateThree = TemplateThree;

			var collectionView = new CollectionView();
			collectionView.Header = collectionViewHeader;
			collectionView.ItemsSource = TestItemSource;
			collectionView.IsGrouped = true;
			collectionView.GroupHeaderTemplate = new DataTemplate(() => new Label() { Text = "Group Name" });
			collectionView.ItemTemplate = templateSelector;

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
			return new Issue11642Group("List 1", new List<IIssue11642Test>()
				{
					new Issue11642TestItemOne(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemOne(),
				});
		}

		public ObservableCollection<Issue11642Group> GenerateCollection()
		{
			return new ObservableCollection<Issue11642Group>()
			{
				new Issue11642Group("List 1", new List<IIssue11642Test>()
				{
					new Issue11642TestItemOne(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemOne(),
				}),
				new Issue11642Group("List 2", new List<IIssue11642Test>()
				{
					new Issue11642TestItemThree(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemOne(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemOne(),
				}),
				new Issue11642Group("List 3", new List<IIssue11642Test>()
				{
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
				}),
				new Issue11642Group("List 4", new List<IIssue11642Test>()
				{
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
				}),
				new Issue11642Group("List 5", new List<IIssue11642Test>()
				{
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
				}),
				new Issue11642Group("List 6", new List<IIssue11642Test>()
				{
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemTwo(),
					new Issue11642TestItemThree(),
				})
			};
		}
	}

	public class Issue11642Group : ObservableCollection<IIssue11642Test>
	{
		public string GroupName { get; set; }

		public Issue11642Group(string groupName, List<IIssue11642Test> items) : base(items)
		{
			GroupName = groupName;
		}
	}

	public class Issue11642TestItemOne : IIssue11642Test
	{
	}

	public class Issue11642TestItemTwo : IIssue11642Test
	{
	}

	public class Issue11642TestItemThree : IIssue11642Test
	{
	}

	public interface IIssue11642Test { }


	public class Issue11642TemplateSelector : DataTemplateSelector
	{
		public DataTemplate TemplateOne { get; set; }
		public DataTemplate TemplateTwo { get; set; }
		public DataTemplate TemplateThree { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is Issue11642TestItemOne)
				return TemplateOne;
			else if (item is Issue11642TestItemTwo)
				return TemplateTwo;
			else
				return TemplateThree;
		}
	}

}