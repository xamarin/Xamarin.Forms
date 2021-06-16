using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.RefreshView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8282, "[Bug] [iOS] RefreshView draws behind CollectionView Header", PlatformAffected.iOS)]
	public partial class Issue8282 : TestContentPage
	{
		public Issue8282()
		{
#if APP
			Title = "Issue 8282";
			InitializeComponent();
			BindingContext = new Issue8282ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class Issue8282Model : INotifyPropertyChanged
	{
		private int _position;

		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;

				OnPropertyChanged("Position");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue8282ViewModel : INotifyPropertyChanged
	{
		bool _isRefreshing;

		public Issue8282ViewModel()
		{
			PopulateItems();

			RefreshCommand = new Command(async () =>
			{
				IsRefreshing = true;

				await Task.Delay(2000);
				PopulateItems();

				IsRefreshing = false;
			});
		}

		public ObservableCollection<Issue8282Model> Items { get; set; } = new ObservableCollection<Issue8282Model>();

		public bool IsRefreshing
		{
			get
			{
				return _isRefreshing;
			}
			set
			{
				_isRefreshing = value;

				OnPropertyChanged("IsRefreshing");
			}
		}


		public Command RefreshCommand { get; set; }

		void PopulateItems()
		{
			var count = Items.Count;

			for (var i = count; i < count + 10; i++)
				Items.Add(new Issue8282Model() { Position = i });
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}