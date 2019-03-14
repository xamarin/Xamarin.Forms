using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.XamStore
{
	[Preserve(AllMembers=true)]
	public partial class DemoShellPage : ContentPage
	{

		HomeViewModel _vm;

		public HomeViewModel ViewModel
		{
			get => _vm; set
			{
				_vm = value;
				BindingContext = _vm;
				_vm.Navigation = this.Navigation;
			}
		}

		public DemoShellPage()
		{
			InitializeComponent();
			ViewModel = new HomeViewModel();
			NavigationPage.SetBackButtonTitle(this, "");
			//AddSearchHandler("Search", SearchBoxVisiblity.Expanded);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel?.OnAppearing();
		}

		void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			ViewModel.EditEntry((VocabEntry)e.Item);

		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			(sender as ListView).SelectedItem = null;
		}

		protected void AddSearchHandler(string placeholder, SearchBoxVisiblity visibility)
		{
			var searchHandler = new BasePage.CustomSearchHandler();

			searchHandler.ShowsResults = true;

			searchHandler.ClearIconName = "Clear";
			searchHandler.ClearIconHelpText = "Clears the search field text";

			searchHandler.ClearPlaceholderName = "Voice Search";
			searchHandler.ClearPlaceholderHelpText = "Start voice search";

			searchHandler.QueryIconName = "Search";
			searchHandler.QueryIconHelpText = "Press to search app";

			searchHandler.Placeholder = placeholder;
			searchHandler.SearchBoxVisibility = visibility;

			searchHandler.ClearPlaceholderEnabled = true;
			searchHandler.ClearPlaceholderIcon = "mic.png";

			Shell.SetSearchHandler(this, searchHandler);
		}

		protected void RemoveSearchHandler()
		{
			ClearValue(Shell.SearchHandlerProperty);
		}
	}

	[Preserve(AllMembers = true)]
	public class HomeViewModel : Issues.BaseViewModel
	{

		public Action Changed { get; set; }

		public IQueryable<VocabEntry> Entries { get; private set; }

		public ICommand NavToAddCommand { get; private set; }

		public ICommand NavToMenuCommand { get; private set; }

		public ICommand ToggleCommand { get; private set; }

		public ICommand DeleteEntryCommand { get; private set; }

		public ICommand SearchCommand { get; private set; }

		public INavigation Navigation { get; set; }

		private bool _isFullTabSelected = true;
		public bool IsFullTabSelected
		{
			get
			{
				return _isFullTabSelected;
			}
			set
			{
				_isFullTabSelected = value;
				OnPropertyChanged(nameof(IsBookmarkedTabSelected));
			}
		}

		public bool IsBookmarkedTabSelected
		{
			get
			{
				return !_isFullTabSelected;
			}
		}

		private string _id;
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		public HomeViewModel()
		{
			IsBusy = false;

			NavToAddCommand = new Command(AddEntry);
			DeleteEntryCommand = new Command<VocabEntry>(DeleteEntry);
			NavToMenuCommand = new Command<string>(GoToMenu);
			ToggleCommand = new Command<string>(Toggle);
			SearchCommand = new Command(OpenSearch);
		}

		void ConnectToRealm()
		{
			IsBusy = true;
			Entries = GenerateList();
			OnPropertyChanged(nameof(Entries));
		}

		IQueryable<VocabEntry> GenerateList()
		{
			return Enumerable.Range(1, 100).Select((arg) => new VocabEntry { IsBookmarked = arg % 2 == 0 }).AsQueryable();
		}

		private async void AddEntry()
		{
			try
			{
				await Shell.CurrentShell.GoToAsync("app:///base/details?id=new");
			}
			catch (Exception ex)
			{
				//App.ShowMessageBox("An error occred navigating to the Job List page", "Navigation Failed!");
				System.Diagnostics.Debug.WriteLine("Navigation failed " + ex.Message);
			};
		}

		private void OpenSearch()
		{

		}

		async private void GoToMenu(string id)
		{
			//var page = new VocabEntryDetailsPage(new VocabEntryDetailsViewModel(entry));

			//Navigation.PushAsync(page);
			Console.WriteLine("GoToMenu");
			await Shell.CurrentShell.GoToAsync($"app:///home/vocab?id={id}");
		}

		private void Toggle(string destination)
		{
			IsFullTabSelected = (destination == ListTabs.FULL);

			if (IsFullTabSelected)
			{
				Entries = GenerateList();
			}
			else
			{
				Entries = GenerateList().Where(c => c.IsBookmarked);
			}
			OnPropertyChanged(nameof(Entries));
		}

		internal async void EditEntry(VocabEntry entry)
		{
			//VocabEntryDetailsPage page = new VocabEntryDetailsPage
			//{
			//    EntryId = entry.Id
			//};

			//Navigation.PushAsync(page);

			await Shell.CurrentShell.GoToAsync($"app:///base/details?id={entry.Id}");
		}

		private void DeleteEntry(VocabEntry entry)
		{
			//_realm.Write(() => _realm.Remove(entry));
		}

		public void OnAppearing()
		{
			ConnectToRealm();
			IsBusy = false;
		}
	}

	[Preserve(AllMembers = true)]
	class InverseBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isTrue = (bool)value;
			return !isTrue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("Only one way bindings are supported with this converter");
		}
	}

	[Preserve(AllMembers = true)]
	class SelectedTabColorConverter : IValueConverter, IMarkupExtension
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var colors = ((string)parameter).Split((';'));
			var isSelected = (bool)value;
			return (isSelected) ? Color.FromHex(colors[0]) : Color.FromHex(colors[1]);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("Only one way bindings are supported with this converter");
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}

	[Preserve(AllMembers = true)]
	public class VocabEntry
	{
		public string Id { get; set; }

		public string Title =>  $"Entry {Id}";

		public bool IsBookmarked { get; set; }

		// If we remove that and use Metadata.Date in the binding, exception is thrown when deleting item. See #883.
		public DateTimeOffset Date => DateTime.Now;

		public VocabEntry()
		{
			Id = Guid.NewGuid().ToString();
		}

	}

	static class ListTabs
	{
		public const string FULL = "full";
		public const string BOOKMARKED = "bookmarked";
	}
}
