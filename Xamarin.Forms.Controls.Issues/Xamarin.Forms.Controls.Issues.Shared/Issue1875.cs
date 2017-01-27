using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1875, "NSRangeException adding items through ItemAppearing", PlatformAffected.iOS)]
	public class Issue1875
		: ContentPage
	{
		const int NumberOfRecords = 15;
		readonly MainViewModel _viewModel;
		int _start = 0;

		public Issue1875()
		{
			var loadData = new Button { Text = "Load", HorizontalOptions = LayoutOptions.FillAndExpand };
			var mainList = new ListView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			mainList.SetBinding(ListView.ItemsSourceProperty, "Items");

			_viewModel = new MainViewModel();
			BindingContext = _viewModel;
			loadData.Clicked += async (sender, e) => { await LoadData(); };

			mainList.ItemAppearing += OnItemAppearing;

			Content = new StackLayout
			{
				Children =
				{
					loadData,
					mainList
				}
			};
		}

		async Task LoadData()
		{
			await _viewModel.LoadData(_start, NumberOfRecords);
			_start = _start + NumberOfRecords;
		}

		async void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
		{
			var item = (int)e.Item;
			if (!_viewModel.IsLoading && item == _viewModel.Items.Last())
				await LoadData();
		}

		public class MainViewModel : INotifyPropertyChanged
		{
			bool _isLoading;
			ObservableCollection<int> _items;

			public MainViewModel()
			{
			}

			public bool IsLoading
			{
				get { return _isLoading; }
				set
				{
					if (_isLoading != value)
					{
						_isLoading = value;
						PropertyChanged(this, new PropertyChangedEventArgs("IsLoading"));
					}
				}
			}

			public ObservableCollection<int> Items
			{
				get
				{
					if (_items == null)
						_items = new ObservableCollection<int>();

					return _items;
				}
				set
				{
					_items = value;
					PropertyChanged(this, new PropertyChangedEventArgs("Items"));
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
#pragma warning disable 1998 // considered for removal
			public async Task LoadData(int start, int numberOfRecords)
#pragma warning restore 1998
			{
				IsLoading = true;
				for (var counter = 0; counter < numberOfRecords; counter++)
					Items.Add(start + counter);

				IsLoading = false;
			}
		}
	}
}