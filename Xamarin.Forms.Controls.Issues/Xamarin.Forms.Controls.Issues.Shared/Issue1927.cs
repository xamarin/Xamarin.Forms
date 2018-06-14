using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1927, "iOS ListViewRenderer has a bug in row insertion/deletion logic.", PlatformAffected.iOS)]
	public class Issue1927 : TestContentPage
	{
		public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>(Enumerable.Range(0, 10).Select(i => $"Initial {i}"));

		public void AddItemToList(string item)
		{
			Items.Add(item);
		}

		protected override void Init()
		{
			var listView = new ListView(ListViewCachingStrategy.RecycleElement) { AutomationId = "ListView", ItemsSource = Items };
			Content = new StackLayout { Children = { new Button { Text = "Add", AutomationId = "btnAdd", Command = new Command(() => AddItemToList("test")) }, listView } };
		}


#if UITEST
		[Test]
		public void Issue1927 Test ()
		{
			RunningApp.WaitForElement (q => q.Marked ("btnAdd"));
			RunningApp.Tap (q => q.Marked ("btnAdd"));
			RunningApp.WaitForElement (q => q.Marked ("ListView"));
		}
#endif
	}
}