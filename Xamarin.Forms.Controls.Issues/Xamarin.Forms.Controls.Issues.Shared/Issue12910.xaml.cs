using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 12910,
		"[Bug] 'Cannot access a disposed object. Object name: 'DefaultRenderer' - on ios with CollectionView and EmptyView",
		PlatformAffected.iOS)]
	public partial class Issue12910 : TestContentPage
	{
		public Issue12910()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue12910ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	public class Issue12910ViewModel : BindableObject
	{
		ObservableCollection<string> _items;

		public Issue12910ViewModel()
		{
			LoadItems();
		}

		public ObservableCollection<string> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public ICommand RemoveItemsCommand => new Command(ExecuteRemoveItems);

		void LoadItems()
		{
			Items = new ObservableCollection<string>
			{
				"Item 1",
				"Item 2",
				"Item 3",
				"Item 4",
				"Item 5",
				"Item 6",
				"Item 7",
				"Item 8",
				"Item 9",
				"Item 10"
			};
		}

		void ExecuteRemoveItems()
		{
			Items.Clear();
		}
	}
}