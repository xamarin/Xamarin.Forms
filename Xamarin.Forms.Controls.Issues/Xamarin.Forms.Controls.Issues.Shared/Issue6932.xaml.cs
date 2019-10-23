using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Layout)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6932, "EmptyView for BindableLayout", PlatformAffected.All)]
	public partial class Issue6932 : TestContentPage
	{
		const string StackLayoutAutomationId = "StackLayoutThing";
		readonly PageViewModel _viewModel = new PageViewModel();

		public Issue6932()
		{
#if APP

			InitializeComponent();
			BindingContext = _viewModel;
#endif
		}

		protected override void Init()
		{

		}

		[Preserve(AllMembers = true)]
		class PageViewModel
		{
			public string LayoutAutomationId { get => StackLayoutAutomationId; }

			public ObservableCollection<object> ItemsSource { get; set; }
			public ICommand AddItemCommand { get; }
			public ICommand RemoveItemCommand { get; }
			public ICommand ClearCommand { get; }

			public PageViewModel()
			{
				ItemsSource = new ObservableCollection<object>(Enumerable.Range(0, 10).Cast<object>().ToList());

				int i = ItemsSource.Count;
				AddItemCommand = new Command(() => ItemsSource.Add(i++));
				RemoveItemCommand = new Command(() =>
				{
					if (ItemsSource.Count > 0)
						ItemsSource.RemoveAt(0);
				});

				ClearCommand = new Command(() =>
				{
					ItemsSource.Clear();
				});
			}
		}

#if UITEST
		[Test]
		public void EmptyViewBecomesVisibleWhenItemsSourceIsCleared()
		{
			RunningApp.Screenshot("Screen opens, items are shown");

			RunningApp.WaitForElement(StackLayoutAutomationId);
			RunningApp.Tap("Clear");
			RunningApp.WaitForElement("No Results");

			RunningApp.Screenshot("Empty view is visible");
		}

		[Test]
		public void EmptyViewBecomesVisibleWhenItemsSourceIsEmptiedOneByOne()
		{
			RunningApp.Screenshot("Screen opens, items are shown");

			RunningApp.WaitForElement(StackLayoutAutomationId);

			for (var i = 0; i < _viewModel.ItemsSource.Count; i++)
				RunningApp.Tap("Remove");

			RunningApp.WaitForElement("No Results");

			RunningApp.Screenshot("Empty view is visible");
		}

		[Test]
		public void EmptyViewHidesWhenItemsSourceIsFilled()
		{
			RunningApp.Screenshot("Screen opens, items are shown");

			RunningApp.WaitForElement(StackLayoutAutomationId);
			RunningApp.Tap("Clear");
			RunningApp.WaitForElement("No Results");

			RunningApp.Screenshot("Items are cleared, empty view visible");

			RunningApp.Tap("Add");
			RunningApp.WaitForNoElement("No Results");

			RunningApp.Screenshot("Item is added, empty view is not visible");
		}
#endif
	}
}