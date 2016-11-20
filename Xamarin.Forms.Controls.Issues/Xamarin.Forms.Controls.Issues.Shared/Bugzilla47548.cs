using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 47548, "There is no way to skip adding status bar underlay on AppCompat", PlatformAffected.Android)]
	public class Bugzilla47548 : TestMasterDetailPage
	{
		protected override void Init()
		{
			Master = new ContentPage { Title = "Master", BackgroundColor = Color.Chocolate };
			Detail = new NavigationPage(new MyMainPage { Title = "MyMainPage" }) { Title = "Detail" };
		}
	}

	public class MyMainPage : ContentPage
	{
		public MyMainPage()
		{
			var list = new List<int>();
			for (var i = 0; i < 100; i++)
				list.Add(i);

			NavigationPage.SetHasNavigationBar(this, false);

			var listView = new ListView
			{
				ItemsSource = list
			};

			var grid = new Grid
			{
				BackgroundColor = Color.FromRgb(Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToByteArray()[0], Guid.NewGuid().ToByteArray()[0])
			};
			grid.RowDefinitions.Add(new RowDefinition {Height = 75});
			grid.RowDefinitions.Add(new RowDefinition());

			var sl = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.Red,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var g = new Grid
			{
				WidthRequest = 50,
				HeightRequest = 50,
				VerticalOptions = LayoutOptions.End,
				BackgroundColor = Color.DeepPink
			};
			var t = new TapGestureRecognizer();
			t.Tapped += TapGestureRecognizerT2;
			g.GestureRecognizers.Add(t);
			sl.Children.Add(g);

			var g2 = new Grid
			{
				WidthRequest = 50,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.DeepPink
			};
			var t2 = new TapGestureRecognizer();
			t2.Tapped += TapGestureRecognizerT;
			g2.GestureRecognizers.Add(t2);
			sl.Children.Add(g2);

			Grid.SetRow(sl, 0);
			grid.Children.Add(sl);

			Grid.SetRow(listView, 1);
			grid.Children.Add(listView);

			Content = grid;
		}

		void TapGestureRecognizerT(object sender, EventArgs e)
		{
			(Parent as NavigationPage).PushAsync(new MyMainPage());
		}

		void TapGestureRecognizerT2(object sender, EventArgs e)
		{
			((Parent as NavigationPage).Parent as MasterDetailPage).IsPresented = !((Parent as NavigationPage).Parent as MasterDetailPage).IsPresented;
		}
	}
}