using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


namespace Xamarin.Forms.Controls.Issues
{
#if APP
	using Xamarin.Forms.Xaml;
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9992, "[CollectionView] Add new element to ItemSource bypasses VerticalItemSpacing and HorizontalItemSpacing",
		PlatformAffected.Android)]
	public partial class Issue9992 : TestContentPage
	{
		public Issue9992 ViewModel
		{
			get => BindingContext as Issue9992;
			set => BindingContext = value;
		}

		public ObservableCollection<Item> Items { get; private set; }
		public ICommand AddCommand { get; private set; }
		public ICommand ReloadCommand { get; private set; }


		protected override void Init()
		{
#if APP
			InitializeComponent();
#endif

			AddCommand = new Command(() =>
			{
				Items.Add(new Item { Name = "Item " + (Items.Count + 1) });
			});
			ReloadCommand = new Command(() =>
			{
				ObservableCollection<Item> tempCollection = new ObservableCollection<Item>(Items);
				Items.Clear();
				foreach (Item item in tempCollection)
					Items.Add(item);
			});

			Items = new ObservableCollection<Item>()
			{
				new Item { Name = "Item 1" }, new Item { Name = "Item 2" }, new Item { Name = "Item 3" }
			};

			ViewModel = this;
		}


		[Preserve(AllMembers = true)]
		public class Item : INotifyPropertyChanged
		{
			string _name;

			public string Name
			{
				get => _name;
				set
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			void OnPropertyChanged([CallerMemberName] string propertyName = "")
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}
}