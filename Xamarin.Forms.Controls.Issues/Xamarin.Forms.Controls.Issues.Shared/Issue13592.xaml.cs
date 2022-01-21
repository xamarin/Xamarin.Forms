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
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13592,
		"[Bug] CollectionView initial selection is broken",
		PlatformAffected.iOS)]
	public partial class Issue13592 : TestContentPage
	{
		public Issue13592()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue13592ViewModel();
#endif
		}

		protected override void Init()
		{
		}
	}

	public class Issue13592ViewModel : BindableObject
	{
		const int NumItems = 20;
		const int NumSelected = 3;

		ObservableCollection<string> _stringItems;
		ObservableCollection<object> _selectedStringItems;
		ObservableCollection<int> _intItems;
		ObservableCollection<object> _selectedIntItems;
		ObservableCollection<string> _log;

		public Issue13592ViewModel()
		{
			Log = new ObservableCollection<string>();

			IntItems = new ObservableCollection<int>();

			for (var i = 0; i < NumItems; i++)
				IntItems.Add(i);

			StringItems = new ObservableCollection<string>();

			for (var i = 0; i < NumItems; i++)
				StringItems.Add(i.ToString());

			AddToLog($"Loaded {NumItems} items");

			SelectedIntItems = new ObservableCollection<object>();

			for (var i = 0; i < NumSelected; i++)
				SelectedIntItems.Add(i);

			SelectedStringItems = new ObservableCollection<object>();

			for (var i = 0; i < NumSelected; i++)
				SelectedStringItems.Add(i.ToString());

			AddToLog($"Loaded initial selection of: {string.Join(", ", this.SelectedIntItems)}");
		}

		public ObservableCollection<string> StringItems
		{
			get { return _stringItems; }
			set
			{
				_stringItems = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<object> SelectedStringItems
		{
			get { return _selectedStringItems; }
			set
			{
				_selectedStringItems = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<int> IntItems
		{
			get { return _intItems; }
			set
			{
				_intItems = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<object> SelectedIntItems
		{
			get { return _selectedIntItems; }
			set
			{
				_selectedIntItems = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<string> Log
		{
			get { return _log; }
			set
			{
				_log = value;
				OnPropertyChanged();
			}
		}

		public void AddToLog(string line)
		{
			Log.Add($"{line}.");
		}
	}
}