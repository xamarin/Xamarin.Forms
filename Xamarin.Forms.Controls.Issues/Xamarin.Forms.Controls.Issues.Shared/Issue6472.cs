using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ManualReview)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6472, "[Bug][iOS] listview / observable collection throwing native error on load", PlatformAffected.iOS)]
	public class Issue6472 : TestContentPage
	{
		class testData
		{
			public int recordId { get; set; }
			public string recordText { get; set; }
		}

		static class staticData
		{
			public static ObservableCollection<testData> TestCollection = new ObservableCollection<testData>();
			public async static void testPopulate()
			{
				await Task.Run(() =>
				{
					TestCollection.Clear();
					for (int i = 0; i < 11; i++)
					{
						testData tdn = new testData
						{
							recordId = i,
							recordText = i.ToString()
						};
						TestCollection.Add(tdn);
					}
				});
			}
		}

		protected override void Init()
		{
			ListView listView = new ListView
			{
				ItemTemplate = new DataTemplate(() =>
				{
					var labelAccount = new Label
					{
						Margin = new Thickness(10, 0),
						VerticalTextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Start,
						LineBreakMode = LineBreakMode.NoWrap,
					};
					labelAccount.FontSize = 18;
					labelAccount.SetBinding(Label.TextProperty, "recordText");

					var stackAccountLayout = new StackLayout
					{
						Orientation = StackOrientation.Vertical,
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.StartAndExpand,
						Children = { labelAccount }
					};
					return new ViewCell { View = stackAccountLayout };
				}),
			};
			Content = listView;
			listView.ItemsSource = staticData.TestCollection;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			staticData.testPopulate();
		}
	}
}