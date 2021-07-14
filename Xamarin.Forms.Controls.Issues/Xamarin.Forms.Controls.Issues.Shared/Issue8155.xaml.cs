using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if UITEST && __ANDROID__
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST && __ANDROID__
	[Category(UITestCategories.CollectionView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8155, "[Bug] CollectionView ItemSelected only binds to List<object> and not to custom objects", PlatformAffected.All)]
	public partial class Issue8155 : TestContentPage
	{
		public Issue8155()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override async void Init()
		{
			Title = "Issue 8155";
			BindingContext = new Issue8155ViewModel();
			await ((Issue8155ViewModel)BindingContext).InitializeAsync();
		}
	}
 
	[Preserve(AllMembers = true)]
	public class Issue8155Model : BindableObject
	{
		private string _title;
		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		private bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public Issue8155Model(string title, bool isSelected = false)
		{
			Title = title;
			IsSelected = isSelected;
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue8155ViewModel : BindableObject
	{
		private ObservableCollection<Issue8155Model> _options = new ObservableCollection<Issue8155Model>();

		public ObservableCollection<Issue8155Model> Options
		{
			get => _options;
			set
			{
				_options = value;
				OnPropertyChanged();
			}
		}
  	
		public List<Issue8155Model> SelectedOptions
		{
			get => new List<Issue8155Model>(Options?.Where(o => o.IsSelected));
		}

		public Task InitializeAsync()
		{
			Options = new ObservableCollection<Issue8155Model>
			{
				new Issue8155Model("Option1"),
				new Issue8155Model("Option2", true),
				new Issue8155Model("Option3"),
				new Issue8155Model("Option4")
			};

			return Task.CompletedTask;
		}
	}
}