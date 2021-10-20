using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ListView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 14095,
		"[Bug] ListView grouping group names not shown on UWP",
		PlatformAffected.UWP)]
	public partial class Issue14095 : TestContentPage
	{
		ObservableCollection<GroupedIssue14095Model> Grouped { get; set; }

		public Issue14095()
		{
#if APP
			InitializeComponent();

			Grouped = new ObservableCollection<GroupedIssue14095Model>();
			var veggieGroup = new GroupedIssue14095Model() { LongName = "vegetables", ShortName = "v" };
			var fruitGroup = new GroupedIssue14095Model() { LongName = "fruit", ShortName = "f" };
			veggieGroup.Add(new Issue14095Model() { Name = "celery", IsReallyAVeggie = true, Comment = "try ants on a log" });
			veggieGroup.Add(new Issue14095Model() { Name = "tomato", IsReallyAVeggie = false, Comment = "pairs well with basil" });
			veggieGroup.Add(new Issue14095Model() { Name = "zucchini", IsReallyAVeggie = true, Comment = "zucchini bread > bannana bread" });
			veggieGroup.Add(new Issue14095Model() { Name = "peas", IsReallyAVeggie = true, Comment = "like peas in a pod" });
			fruitGroup.Add(new Issue14095Model() { Name = "banana", IsReallyAVeggie = false, Comment = "available in chip form factor" });
			fruitGroup.Add(new Issue14095Model() { Name = "strawberry", IsReallyAVeggie = false, Comment = "spring plant" });
			fruitGroup.Add(new Issue14095Model() { Name = "cherry", IsReallyAVeggie = false, Comment = "topper for icecream" });

			Grouped.Add(veggieGroup);
			Grouped.Add(fruitGroup);

			lstView.ItemsSource = Grouped;
#endif
		}

		protected override void Init()
		{
		}
	}

	public class Issue14095Model
	{
		public string Name { get; set; }
		public string Comment { get; set; }
		public bool IsReallyAVeggie { get; set; }
		public string Image { get; set; }
	}

	public class GroupedIssue14095Model : ObservableCollection<Issue14095Model>
	{
		public string LongName { get; set; }
		public string ShortName { get; set; }
	}
}