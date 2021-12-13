using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
    [NUnit.Framework.Category(Core.UITests.UITestCategories.SwipeView)]
#endif
    [Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13790,
		"[Bug] ListView with grouping and SwipeView throws IllegalStateException: The specified child already has a parent",
		PlatformAffected.Android)]
	public partial class Issue13790 : TestContentPage
	{
		public Issue13790()
		{
#if APP
			InitializeComponent();
            BindingContext = new Issue13790ViewModel();
#endif
        }

        protected override void Init()
		{
		}
    }

    public class Issue13790Model
    {
        public Issue13790Model(Action<object> onNavigate, Action<object> onDelete)
        {
            NavigateCommand = new Command(onNavigate);
            DeleteCommand = new Command(onDelete);
        }

        public string Name { get; set; }
        public string Comment { get; set; }
        public bool IsReallyAVeggie { get; set; }
        public string Image { get; set; }

        public ICommand NavigateCommand { get; }
        public ICommand DeleteCommand { get; }
    }

    public class Issue13790GroupedModel : ObservableCollection<Issue13790Model>
    {
        private readonly IList<Issue13790Model> _shadowList = new List<Issue13790Model>();
        private bool _expanded;

        public Issue13790GroupedModel()
        {
            IsExpanded = true;
            ExpandCollapseCommand = new Command(() => ToggleExpandCollapse());
        }

        public string LongName { get; set; }
        public string ShortName { get; set; }

        public bool IsExpanded
        {
            get => _expanded;
            private set
            {
                _expanded = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsExpanded)));
            }
        }

        public ICommand ExpandCollapseCommand { get; }

        private void ToggleExpandCollapse()
        {
            if (IsExpanded)
            {
                CollapseInternal();
            }
            else
            {
                ExpandInternal();
            }
        }

        public void Collapse()
        {
            if (!IsExpanded)
            {
                return;
            }

            CollapseInternal();
        }

        public void Expand()
        {
            if (IsExpanded)
            {
                return;
            }

            ExpandInternal();
        }

        private void CollapseInternal()
        {
            foreach (var item in this)
            {
                _shadowList.Add(item);
            }
            Clear();
            IsExpanded = false;
        }

        private void ExpandInternal()
        {
            foreach (var item in _shadowList)
            {
                Add(item);
            }
            _shadowList.Clear();
            IsExpanded = true;
        }
    }

    internal class Issue13790ViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Issue13790GroupedModel> grouped;

        public Issue13790ViewModel()
        {
            Grouped = new ObservableCollection<Issue13790GroupedModel>();
            LoadData();
        }

        private void LoadData()
        {
            // Direct assignment + propertychanged does have the same effect as...
            Grouped = new ObservableCollection<Issue13790GroupedModel>(GetGroups());

            // ... adding items one-by-one using collectionchanged event:
            foreach (var g in GetGroups())
            {
                Grouped.Add(g);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private IEnumerable<Issue13790GroupedModel> GetGroups()
        {
            var veggieGroup = new Issue13790GroupedModel() { LongName = "vegetables", ShortName = "v" };
            var fruitGroup = new Issue13790GroupedModel() { LongName = "fruit", ShortName = "f" };
            var veggie1Group = new Issue13790GroupedModel() { LongName = "vegetables1", ShortName = "v1" };
            var fruit1Group = new Issue13790GroupedModel() { LongName = "fruit1", ShortName = "f1" };
            var veggie2Group = new Issue13790GroupedModel() { LongName = "vegetables2", ShortName = "v2" };
            var fruit2Group = new Issue13790GroupedModel() { LongName = "fruit2", ShortName = "f2" };

            veggieGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "celery", IsReallyAVeggie = true, Comment = "try ants on a log" });
            veggieGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "tomato", IsReallyAVeggie = false, Comment = "pairs well with basil" });
            veggieGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "zucchini", IsReallyAVeggie = true, Comment = "zucchini bread > bannana bread" });
            veggieGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "peas", IsReallyAVeggie = true, Comment = "like peas in a pod" });
            fruitGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "banana", IsReallyAVeggie = false, Comment = "available in chip form factor" });
            fruitGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "strawberry", IsReallyAVeggie = false, Comment = "spring plant" });
            fruitGroup.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "cherry", IsReallyAVeggie = false, Comment = "topper for icecream" });

            veggie1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "celery", IsReallyAVeggie = true, Comment = "try ants on a log" });

            veggie1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "tomato", IsReallyAVeggie = false, Comment = "pairs well with basil" });
            veggie1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "zucchini", IsReallyAVeggie = true, Comment = "zucchini bread > bannana bread" });
            veggie1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "peas", IsReallyAVeggie = true, Comment = "like peas in a pod" });
            fruit1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "banana", IsReallyAVeggie = false, Comment = "available in chip form factor" });
            fruit1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "strawberry", IsReallyAVeggie = false, Comment = "spring plant" });
            fruit1Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "cherry", IsReallyAVeggie = false, Comment = "topper for icecream" });

            veggie2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "celery", IsReallyAVeggie = true, Comment = "try ants on a log" });
            veggie2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "tomato", IsReallyAVeggie = false, Comment = "pairs well with basil" });
            veggie2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "zucchini", IsReallyAVeggie = true, Comment = "zucchini bread > bannana bread" });
            veggie2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "peas", IsReallyAVeggie = true, Comment = "like peas in a pod" });
            fruit2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "banana", IsReallyAVeggie = false, Comment = "available in chip form factor" });
            fruit2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "strawberry", IsReallyAVeggie = false, Comment = "spring plant" });
            fruit2Group.Add(new Issue13790Model(OnNavigate, OnDelete) { Name = "cherry", IsReallyAVeggie = false, Comment = "topper for icecream" });

            yield return veggieGroup;
            yield return fruitGroup;
            yield return veggie1Group;
            yield return fruit1Group;
            yield return veggie2Group;
            yield return fruit2Group;
        }


        private void OnNavigate(object obj)
        {
        }


        private void OnDelete(object obj)
        {
            // This method does not really delete anything,
            // it's just a handler to simulate changes in Grouped list

            Device.BeginInvokeOnMainThread(async () =>
            {
                Grouped.Clear();
                await Task.Delay(2000);
                LoadData();
            });
        }


        public ObservableCollection<Issue13790GroupedModel> Grouped
        {
            get => grouped;
            private set
            {
                grouped = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Grouped)));
            }
        }
    }
}