using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13399,
		"[Bug] The specified child already has a parent",
		PlatformAffected.Android)]
	public partial class Issue13399 : TestContentPage
	{
		public Issue13399()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue13399ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class Issue13399Model
	{
		public int Id { get; set; }
		public string Text { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue13399ViewModel : BindableObject
	{
		ObservableCollection<Issue13399Model> _items;

		public Issue13399ViewModel()
		{
			Items = new ObservableCollection<Issue13399Model>();
			LoadItems();
		}

		public ObservableCollection<Issue13399Model> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public ICommand DeleteLastItemCommand => new Command(ExecuteDeleteLastItem);
		public ICommand DeleteItemCommand => new Command<Issue13399Model>(ExecuteDeleteItem);

		void LoadItems()
		{
			for (int i = 0; i < 100; i++)
			{
				Items.Add(new Issue13399Model { Id = i + 1, Text = $"Item {i + 1}" });
			}
		}

		void ExecuteDeleteLastItem()
		{
			if (Items.Count == 0)
				return;

			var index = Items.Count - 1;
			Items.RemoveAt(index);
		}

		void ExecuteDeleteItem(Issue13399Model item)
		{
			if (Items.Count == 0)
				return;

			Items.Remove(item);
		}
	}
}