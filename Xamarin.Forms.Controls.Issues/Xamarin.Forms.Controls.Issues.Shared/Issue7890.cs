using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7890, "TemplatedItemsList incorrect grouped collection range removal", PlatformAffected.All)]
	public class Issue7890 : TestContentPage
	{
		protected override void Init()
		{
			var items = Enumerable.Range(0, 10).Select(x => new DataGroup(x));
			var source = new ObservableCollectionFast<DataGroup>(items);

			var listView = new ListView()
			{
				IsGroupingEnabled = true,
				ItemsSource = source,
				GroupDisplayBinding = new Binding("Text"),
			};

			Content = new ScrollView()
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label() { Text = "Button click should remove items from 1 to 5"},
						new Button()
						{
							Text = "remove",
							Command = new Command(() =>
							{
								source.RemoveRange(1,5);
							})
						},
						listView
					}
				}
			};
		}

		public class DataGroup : List<Data>
		{
			public DataGroup(int num)
			{
				Text = $"Group {num}";
				Add(new Data() { Text = num });
			}
			public string Text { get; set; }
		}
		public class Data
		{
			public int Text { get; set; }

			public override string ToString()
			{
				return Text.ToString();
			}
		}
	}

	public class ObservableCollectionFast<T> : ObservableCollection<T>
	{
		public ObservableCollectionFast(IEnumerable<T> collection) : base(collection) { }

		public void RemoveRange(int index, int count)
		{
			var removed = new List<T>(count);
			for (var i = index + count - 1; i >= index; i--)
			{
				removed.Add(Items[i]);
				Items.Remove(Items[i]);
			}
			this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
		}
	}
}
