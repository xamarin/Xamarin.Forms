using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
#endif


namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7167,
		"[Bug] improved observablecollection with system.reactive. a lot of collectionchanges. a reset is sent and listview scrolls to the top", PlatformAffected.UWP)]
	public partial class Issue7167 : TestContentPage
	{
		public Issue7167()
		{
#if APP
			InitializeComponent();
#endif
			BindingContext = new Issue7167ViewModel();

		}
			
		protected override void Init()
		{
			
		}

		void MyListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem;
			var index = e.SelectedItemIndex;

		}


#if UITEST

		const string ListViewId = "ListViewId";
		const string AddCommandID = "AddCommandID";
		const string ClearListCommandId = "ClearListCommandId";
		const string AddRangeCommandId = "AddRangeCommandId";
		const string AddRangeWithCleanCommandId = "AddRangeWithCleanCommandId";

		[Test]
		public  void Issue7167Test()
		{
			RunningApp.Print.Tree();
			// arrange
			RunningApp.Screenshot("Empty ListView");
			RunningApp.Tap(q => q.Button(AddRangeCommandId));
			RunningApp.Tap(q => q.Button(AddRangeCommandId));
			RunningApp.ScrollDown("25", ScrollStrategy.Programmatically);
			RunningApp.WaitForElement(x => x.Marked("25"));
			RunningApp.Tap(q => q.Marked(AddRangeCommandId));
			RunningApp.Query(x => x.Marked(ListViewId).Child().Marked("25"));
			// act

			// assert

		}
#endif

		
	}

	internal class Issue7167ViewModel
	{
		//private static readonly IEnumerable<string> _items = Enumerable.Range(0, 50).Select(num => num.ToString());

		private IEnumerable<string> CreateItems()
		{
			var itemCount = Items.Count();
			return Enumerable.Range(itemCount, 50).Select(num => num.ToString());
		}

		public ImprovedObservableCollection<string> Items { get; set; } = new ImprovedObservableCollection<string>();

		public ICommand AddCommand => new Command(_ => Items.Add(CreateItems().First()));
		public ICommand ClearListCommand => new Command(_ => Items.Clear());
		public ICommand AddRangeCommand => new Command(_ => Items.AddRange(CreateItems()));
		public ICommand AddRangeWithCleanCommand => new Command(_ =>
		{
			Items.Clear();
			Items.AddRange(CreateItems());
		});

	}

	internal class ImprovedObservableCollection<T> : ObservableCollection<T>
	{
		bool _isActivated = true;

		public ImprovedObservableCollection()
		{
			
		}
		public void AddRange(IEnumerable<T> source)
		{
			_isActivated = false;

			foreach (var item in source)
			{
				base.Items.Add(item);
			}

			_isActivated = true;

			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_isActivated)
				base.OnCollectionChanged(e);
		}

	}

	

}


