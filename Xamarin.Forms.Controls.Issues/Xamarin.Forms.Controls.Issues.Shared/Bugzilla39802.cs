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
			BackgroundColor = Color.Yellow;

			var list = new ObservableCollection<MyItem>();
			for (int i = 0; i < 100; i++)
			{
				var item = new MyItem { Title = "List item: " + (i + 1) , Color = Color.Red};
				list.Add(item);
				if (i % 2 == 0)
				{
					item.Color = Color.Blue;
				}

			}
			ListItems = list;

			BindingContext = this;
			var lst = new ListView(ListViewCachingStrategy.RecycleElement)
			{
				BackgroundColor = Color.Transparent,
				ItemTemplate = new DataTemplate(typeof(ItemTemplate))
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
					Padding = new Thickness(15, 0, 0, 0)
				};
				stk.SetBinding(VisualElement.BackgroundColorProperty, nameof(MyItem.Color));
				var lbl = new Label
				{
					TextColor = Color.Yellow,
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

			public Color Color { get; set; }
		}
	}
}