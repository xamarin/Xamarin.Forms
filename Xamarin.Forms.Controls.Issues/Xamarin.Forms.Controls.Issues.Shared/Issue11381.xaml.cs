using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ListView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11381, "[Bug] [iOS] NRE on grouped ListView when removing cells with gesture recognizers",
		PlatformAffected.iOS)]
	public partial class Issue11381 : TestContentPage
	{
		public Issue11381()
		{
#if APP
			InitializeComponent();

			grouped = new ObservableCollection<GroupedIssue11381Model> ();
			var veggieGroup = new GroupedIssue11381Model () { LongName = "vegetables", ShortName="v" };
			var fruitGroup = new GroupedIssue11381Model () { LongName = "fruit", ShortName = "f" };
			veggieGroup.Add (new Issue11381Model () { Name = "celery", IsReallyAVeggie = true, Comment = "try ants on a log" });
			veggieGroup.Add (new Issue11381Model () { Name = "tomato", IsReallyAVeggie = false, Comment = "pairs well with basil" });
			veggieGroup.Add (new Issue11381Model () { Name = "zucchini", IsReallyAVeggie = true, Comment = "zucchini bread > bannana bread" });
			veggieGroup.Add (new Issue11381Model () { Name = "peas", IsReallyAVeggie = true, Comment = "like peas in a pod" });
			fruitGroup.Add (new Issue11381Model () {Name = "banana", IsReallyAVeggie = false,Comment = "available in chip form factor"});
			fruitGroup.Add (new Issue11381Model () {Name = "strawberry", IsReallyAVeggie = false,Comment = "spring plant"});
			fruitGroup.Add (new Issue11381Model () {Name = "cherry", IsReallyAVeggie = false,Comment = "topper for icecream"});
			grouped.Add (veggieGroup); grouped.Add (fruitGroup);
			Issue11381ListView.ItemsSource = grouped;
#endif
		}

		public ObservableCollection<GroupedIssue11381Model> grouped { get; set; }

		protected override void Init()
		{

		}

#if APP
		void OnTapGestureRecognizerTapped(object sender, EventArgs e)
		{
			if (sender is View view && view.BindingContext is Issue11381Model model)
			{
				var group = grouped.FirstOrDefault(g => g.Contains(model));

				if (group != null)
				{
					group.Remove(model);

					if (!group.Any())
					{
						grouped.Remove(group);
					}
				}
			}
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class Issue11381Model
	{
		public string Name { get; set; }
		public string Comment { get; set; }
		public bool IsReallyAVeggie { get; set; }
		public string Image { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class GroupedIssue11381Model : ObservableCollection<Issue11381Model>
	{
		public string LongName { get; set; }
		public string ShortName { get; set; }
	}
}