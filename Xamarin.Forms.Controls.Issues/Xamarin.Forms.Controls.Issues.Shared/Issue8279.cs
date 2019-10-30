using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 8279, "[Feature requested] ListView do not ScrollTo a group when there is no child of this group", PlatformAffected.Android)]
    public class Issue8279 : ContentPage
    {
        public static ListView List { get; set; }
        public static List<MyGroup> Data { get; set;  }
        public Issue8279()
        {
            Data = new List<MyGroup>();
            Data.Add(new MyGroup(){Headertitle = "Header 1"});
            Data.First().Add(new MyData(){Title = "title 1"});
            Data.First().Add(new MyData() { Title = "title 2" });
			Data.First().Add(new MyData() { Title = "title 3" });
			Data.First().Add(new MyData() { Title = "title 4" });
			Data.First().Add(new MyData() { Title = "title 5" });
			Data.First().Add(new MyData() { Title = "title 6" });
			Data.First().Add(new MyData() { Title = "title 7" });
			Data.First().Add(new MyData() { Title = "title 8" });
			Data.Add(new MyGroup() { Headertitle = "Header 2" });
			Data.Add(new MyGroup() { Headertitle = "Header 3" });
			Data.Last().Add(new MyData() { Title = "title 3a" });
			Data.Last().Add(new MyData() { Title = "title 3b" });
			Data.Last().Add(new MyData() { Title = "title 3b" });
			Data.Last().Add(new MyData() { Title = "title 3b" });

			List = new ListView();
            List.HorizontalOptions = LayoutOptions.FillAndExpand;
            List.VerticalOptions = LayoutOptions.FillAndExpand;
            List.BackgroundColor = Color.Yellow;
            List.ItemTemplate = new DataTemplate(typeof (VCTest));
            List.GroupHeaderTemplate = new DataTemplate(typeof(VCHeader));
            List.IsGroupingEnabled = true;
            List.ItemsSource = Data;

			var lastGroup = Data.Last();
			var lastItem = lastGroup.First();
			var firstGroup = Data.First();
			var firstItem = firstGroup.First();
			var emptyGroup = Data[1];

			var button1 = new Button()
			{
				Text = "Scroll with no item but group",
				Command = new Command(()=> List.ScrollTo(null, lastGroup, ScrollToPosition.MakeVisible, true))
			};

			var button2 = new Button()
			{
				Text = "Scroll with item but no group",
				Command = new Command(() => List.ScrollTo(firstItem, ScrollToPosition.MakeVisible, true))
			};

			var button3 = new Button()
			{
				Text = "Scroll with item with group",
				Command = new Command(() => List.ScrollTo(firstItem, lastGroup, ScrollToPosition.MakeVisible, true))
			};

			var button4 = new Button()
			{
				Text = "Scroll with no item no group",
				Command = new Command(() => List.ScrollTo(null, null, ScrollToPosition.MakeVisible, true))
			};

			var button5 = new Button()
			{
				Text = "Scroll with no item but empty group",
				Command = new Command(() => List.ScrollTo(null, emptyGroup, ScrollToPosition.MakeVisible, true))
			};

			Content = new StackLayout () {
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { button1, button2, button3, button4, button5, List },
			};
        }

		[Preserve(AllMembers = true)]
		public class MyData : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			string _title;

			public const string PropTitle = "Title";

			public string Title
			{
				get { return _title; }
				set
				{
					if (value.Equals(_title, StringComparison.Ordinal))
						return;
					_title = value;
					OnPropertyChanged(new PropertyChangedEventArgs(PropTitle));
				}
			}

			public void OnPropertyChanged(PropertyChangedEventArgs e)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, e);
			}
		}
		[Preserve(AllMembers = true)]
		public class MyGroup : ObservableCollection<MyData>, INotifyPropertyChanged
		{
			string _headertitle;

			public const string PropHeadertitle = "Headertitle";

			public string Headertitle
			{
				get { return _headertitle; }
				set
				{
					if (value.Equals(_headertitle, StringComparison.Ordinal))
						return;
					_headertitle = value;
					OnPropertyChanged(new PropertyChangedEventArgs(PropHeadertitle));
				}
			}
		}
		[Preserve(AllMembers = true)]
		internal class VCTest : ViewCell
		{
			public VCTest()
			{
				var label = new Label();
				label.SetBinding(Label.TextProperty, "Title");
				View = label;
			}
		}
		[Preserve(AllMembers = true)]
		internal class VCHeader : ViewCell
		{
			public VCHeader()
			{
				var label = new Label();
				label.SetBinding(Label.TextProperty, "Headertitle");
				View = label;
			}
		}
	}
	
}
