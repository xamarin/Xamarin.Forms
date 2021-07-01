using System;
using System.Collections.Generic;
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
    [Category(UITestCategories.Shape)]
#endif
    [Preserve(AllMembers = true)]
    [Issue(IssueTracker.Github, 13441,
        "[Bug] Android CarouselView crashes inside of ListView if Loop set to False",
		PlatformAffected.Android)]
	public partial class Issue13441 : TestContentPage
	{
        public Issue13441()
        {
#if APP
            InitializeComponent();

            List<Issue13441Items> items = new List<Issue13441Items>();

            for (int i = 0; i < 20; i++)
            {
                items.Add(new Issue13441Items()
                {
                    Items = new List<Issue13441Item>()
                    {
                        new Issue13441Item(){ Text = "https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/controls/cells-images/textcell-large.png" },
                        new Issue13441Item(){ Text = "https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/tableview-images/entry-cell.png" },
                        new Issue13441Item(){ Text = "https://docs.microsoft.com/xamarin/ios/user-interface/controls/tables/creating-tables-in-a-storyboard-images/image28a.png" }
                    }
                });
            }

            listView.ItemsSource = items;
#endif
        }

        protected override void Init()
		{
        }
#if UITEST && __ANDROID__
        [Test]
        public void CarouselViewInsideListViewScrollTest()
        {
            string listView = "ListViewId";
            RunningApp.WaitForElement(listView);
            RunningApp.ScrollDown(b => b.Marked(listView));
            RunningApp.ScrollDown(b => b.Marked(listView));
        }
#endif
    }

    [Preserve(AllMembers = true)]
    public class Issue13441Items
    {
        public List<Issue13441Item> Items { get; set; } = new List<Issue13441Item>();
    }

    [Preserve(AllMembers = true)]
    public class Issue13441Item
    {
        public string Text { get; set; }
    }
}