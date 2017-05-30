using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 939802, "Gap between ListView cells even if SeparatorVisablity is set to none ", PlatformAffected.iOS)]
	public class Bugzilla39802 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{

			var list = new ObservableCollection<MyItem>();
			for (int i = 0; i < 2; i++)
			{
				list.Add(new MyItem { Title = "List item: " + (i + 1) });
			}
			ListItems = list;

			BindingContext = this;
			var lst = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				ItemTemplate =  new DataTemplate(typeof(ItemTemplate))
			};
			lst.SeparatorVisibility = SeparatorVisibility.None;
			lst.SetBinding(ListView.ItemsSourceProperty, nameof(ListItems));
			Content = lst;
		}

		public class ItemTemplate : ViewCell
		{
			public ItemTemplate()
			{
				var stk = new StackLayout
				{
					BackgroundColor = Color.Blue,
					Padding = new Thickness(15,0,0,0)
				};
				var lbl = new Label
				{
					TextColor = Color.White,
					VerticalOptions = LayoutOptions.CenterAndExpand
				};
				lbl.SetBinding(Label.TextProperty, nameof(MyItem.Title));
				stk.Children.Add(lbl);
				View = stk;
			}
		}

		public ObservableCollection<MyItem> ListItems { get; set; }
		public class MyItem
		{
			public string Title { get; set; }
		}
	}
}