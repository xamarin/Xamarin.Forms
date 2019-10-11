using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7700, "[Bug][iOS] If CollectionView in other Tab gets changed before it's displayed, it stays invisible",
		PlatformAffected.iOS)]
	public class Issue7700 : TestTabbedPage
	{
		ObservableCollection<string> _source = new ObservableCollection<string>(){ "one", "two", "three" };

		const string Add = "Add";
		const string Success = "Success";
		const string Tab2 = "Tab2";

		protected override void Init()
		{
#if APP
			FlagTestHelpers.SetCollectionViewTestFlag();

			Children.Add(FirstPage());
			Children.Add(CollectionViewPage());
#endif
		}

		ContentPage FirstPage()
		{
			var page = new ContentPage() { Title = "7700 First Page", Padding = 40 };

			var button = new Button() { Text = Add, AutomationId = Add };

			button.Clicked += AddButtonClicked;

			page.Content = button;

			return page;
		}

		private void AddButtonClicked(object sender, EventArgs e)
		{
			_source.Insert(0, Success);
		}

		ContentPage CollectionViewPage()
		{
			var cv = new CollectionView();

			cv.ItemTemplate = new DataTemplate(() => {
				var label = new Label();
				label.SetBinding(Label.TextProperty, new Binding("."));
				return label;
			});

			cv.ItemsSource = _source;

			var page = new ContentPage() { Title = Tab2, Padding = 40 };

			page.Content = cv;

			return page;
		}

#if UITEST
		[Test]
		public void AddingItemToUnviewedCollectionViewShouldNotCrash()
		{
			RunningApp.WaitForElement(Go);
			RunningApp.Tap(Go);	
			RunningApp.Tap(Tab2);		

			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
