using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 42534, "Setting SetSelectAllOnFocus(true) on Custom Entry Does Not Always Work", PlatformAffected.Android)]
	public class Issue42534 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		public List<int> ListItems = new List<int>();

		protected override void Init()
		{
			for (var i = 1000; i < 1020; i++)
			{
				ListItems.Add(i);
			}

			var listView = new ListView
			{
				ItemsSource = ListItems,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HasUnevenRows = true,
				ItemTemplate = new DataTemplate(() =>
				{
					var stackLayout = new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Padding = 0,
						VerticalOptions = LayoutOptions.FillAndExpand
					};

					var label = new Label
					{
						Text = "My Label"
					};
					stackLayout.Children.Add(label);

					var entry = new Entry
					{
						HorizontalOptions = LayoutOptions.End,
						WidthRequest = 100
					};
					entry.SetBinding(Entry.TextProperty, new Binding("."));
					stackLayout.Children.Add(entry);

					return new ViewCell {Height = 75, View = stackLayout};
				})
			};

			Content = listView;
		}
	}
}