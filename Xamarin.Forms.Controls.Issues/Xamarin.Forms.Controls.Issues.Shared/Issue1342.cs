using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif
namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1342, "[iOS] ListView throws Exception on ObservableCollection.Add/Remove", PlatformAffected.iOS)]
	public class Issue1342 : TestNavigationPage
	{
		protected override void Init()
		{
			Navigation.PushAsync(new MainPage());
		}

		class MainViewModel
		{
			public ListViewModel ViewModel { get; set; }
		}

		class ListViewModel
		{
			public ObservableCollection<string> Items { get; set; }
		}

		class MainPage : TabbedPage
		{
			public MainPage()
			{
				var mainViewModel = new MainViewModel
				{
					ViewModel = new ListViewModel
					{
						Items = new ObservableCollection<string>(new[] { "item2.1", "item2.2", "item2.3" })
					}
				};
				BindingContext = mainViewModel;

				var page1 = new ContentPage
				{
					Title = "Page 1",
					Content = new Label { Text = "Ok, now please click the toolbar item!" }
				};
				Children.Add(page1);

				var page2 = new ListPage { Title = "Page 2", AutomationId = "Page2" };
				page2.SetBinding(BindingContextProperty, new Binding(nameof(MainViewModel.ViewModel)));
				Children.Add(page2);

				ToolbarItems.Add(
					new ToolbarItem("Add item", null, () => mainViewModel.ViewModel.Items.Add("new item")) { AutomationId = "Add" });
			}
		}

		class ListPage : ContentPage
		{
			public ListPage()
			{
				var listView = new ListView(ListViewCachingStrategy.RecycleElement) { AutomationId = "ListView", HasUnevenRows = true };
				listView.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(ListViewModel.Items)));
				Content = listView;
			}
		}
#if UITEST
		[Test]
		public void Issue1342Test()
		{
			RunningApp.WaitForElement("Add");
			RunningApp.Tap("Add");
			RunningApp.Tap("Page2");
			RunningApp.WaitForElement("ListView");
		}
#endif
	}
}