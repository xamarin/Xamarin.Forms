using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile), Issue(IssueTracker.Github, 9584, "CollectionView - Lost focus for Header and Footer when new ItemSource assigned", PlatformAffected.All)]
	public partial class Issue9584 : TestContentPage
	{
		public Issue9584()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new CollectionViewModel();
		}

		public class CollectionViewModel : INotifyPropertyChanged
		{
			public CollectionViewModel()
			{
				FillPersons();
			}

			ObservableCollection<Person> _persons = new ObservableCollection<Person>();
			string _searchString;
			CancellationTokenSource _cts;

			public ObservableCollection<Person> Persons
			{
				get => _persons;
				set
				{
					_persons = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Persons"));
				}
			}

			public string SearchString
			{
				get => _searchString;
				set
				{
					_searchString = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchString"));
					_cts?.Cancel();
					_cts = new CancellationTokenSource();
					Task.Delay(1000, _cts.Token).ContinueWith(c =>
					{
						if(c.IsCanceled)
						{
							return;
						}

						if(string.IsNullOrWhiteSpace(_searchString))
						{
							FillPersons();
						}
						else
						{
							//foreach (var item in Persons.Where(w => !w.Surname.Contains(_searchString) && !w.Name.Contains(_searchString)).ToList())
							//{
							//	Persons.Remove(item);
							//}
							Persons = new ObservableCollection<Person>(Persons.Where(w => w.Surname.Contains(_searchString) || w.Name.Contains(_searchString)));
						}
					});
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			void FillPersons()
			{
				List<Person> lst = new List<Person>();
				for (int i = 0; i < 100; i++)
				{
					lst.Add(new Person { Name = i.ToString(), Surname = (i + 100).ToString() });
				}

				Persons = new ObservableCollection<Person>(lst);
			}
		}

		public class Person
		{
			public string Name { get; set; }

			public string Surname { get; set; }
		}
	}
}