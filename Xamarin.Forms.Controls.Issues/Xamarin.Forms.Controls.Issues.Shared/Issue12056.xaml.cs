using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

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
	[Issue(IssueTracker.Github, 12056,
		"[Bug] Snap Points not working in CollectionView with GridItemsLayout",
		PlatformAffected.Android)]
	public partial class Issue12056 : TestContentPage
	{
		readonly Issue12056ViewModel _viewModel;

		public Issue12056()
		{
            _viewModel = new Issue12056ViewModel();
#if APP
			InitializeComponent();
	
            BindingContext = _viewModel;
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel.Items.Count == 0)
                _viewModel.IsBusy = true;
        }

		protected override void Init()
		{
	
		}
	}

    [Preserve(AllMembers = true)]
    public class Issue12056Model
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    [Preserve(AllMembers = true)]
    public class Issue12056GroupModel<TCollectionItem>
        : ObservableCollection<TCollectionItem>
    {
        public string GroupContent { get; private set; }

        public Issue12056GroupModel(string content, IList<TCollectionItem> items)
        {
            items ??= new TCollectionItem[0];

            foreach (var item in items)
            {
                Add(item);
            }

            GroupContent = content;
        }
    }

    [Preserve(AllMembers = true)]
	public class Issue12056ViewModel :BindableObject
    {
		bool _isBusy = false;

        public Issue12056ViewModel()
        {
            Items = new ObservableCollection<Issue12056GroupModel<Issue12056Model>>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Issue12056GroupModel<Issue12056Model>> Items { get; set; }

        public Command LoadItemsCommand { get; set; }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();

                await Task.Delay(2000);

                var temp = new List<Issue12056GroupModel<Issue12056Model>>();

                for (int i = 0; i < 5; i++)
                {
                    var items  = new List<Issue12056Model>()
                    {
                        new Issue12056Model { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                        new Issue12056Model { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                        new Issue12056Model { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                        new Issue12056Model { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                        new Issue12056Model { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                        new Issue12056Model { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
                    };

                    temp.Add(new Issue12056GroupModel<Issue12056Model>($"group-{i}", items));
                }

                foreach (var item in temp)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}