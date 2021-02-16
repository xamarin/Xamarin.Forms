using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13631,
		"[Bug] Wrong behavior using BindableLayout wrapped by a RefreshView",
		PlatformAffected.UWP)]
	public partial class Issue13631 : TestContentPage
	{
		public Issue13631()
		{
#if APP
			InitializeComponent();
            var viewModel = new Issue13631ViewModel();
            BindingContext = viewModel;

            CreateUI(viewModel);
#endif
		}

#if APP
		void CreateUI(Issue13631ViewModel viewModel)
        {
            RefreshView refreshView = new RefreshView
            {
                RefreshColor = Color.Red
            };

            refreshView.SetBinding(RefreshView.IsRefreshingProperty, "IsRefreshing");
            refreshView.SetBinding(RefreshView.CommandProperty, "RefreshCommand");

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnTapGestureRecognizerTapped;

            DataTemplate itemTemplate = new DataTemplate(() =>
            {
                Grid templateGrid = new Grid
                {
                    BackgroundColor = Color.Transparent
                };

                templateGrid.GestureRecognizers.Add(tapGestureRecognizer);

                Frame templateFrame = new Frame
                {
                    BackgroundColor = Color.Red,
                    CornerRadius = 12,
                    Margin = new Thickness(12)
                };

                Label templateLabel = new Label
                {
                    TextColor = Color.White
                };

                templateLabel.SetBinding(Label.TextProperty, "Text1");

                templateFrame.Content = templateLabel;

                templateGrid.Children.Add(templateFrame);

                return templateGrid;
            });

            var scrollView = new ScrollView();

            var stackLayout = new StackLayout();
            BindableLayout.SetItemsSource(stackLayout, viewModel.Items);
            BindableLayout.SetItemTemplate(stackLayout, itemTemplate);

            scrollView.Content = stackLayout;
            refreshView.Content = scrollView;

            Container.Children.Add(refreshView);
        }
    
        void OnTapGestureRecognizerTapped(object sender, System.EventArgs e)
        {
            if (((View)sender).BindingContext is Issue13631Model item)
                DisplayAlert("Issue 13631", item.Text1, "Ok");
        }
#endif

		protected override void Init()
		{
		}
	}

	public class Issue13631Model
	{
		public string Text1 { get; set; }
		public string Text2 { get; set; }
	}

	public class Issue13631ViewModel : BindableObject
	{
		ObservableCollection<Issue13631Model> _items;
		bool _isRefreshing;

		public Issue13631ViewModel()
		{
			Items = new ObservableCollection<Issue13631Model>();
			LoadItems();
		}

		public ObservableCollection<Issue13631Model> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		public bool IsRefreshing
		{
			get { return _isRefreshing; }
			set
			{
				_isRefreshing = value;
				OnPropertyChanged();
			}
		}

		public ICommand RefreshCommand => new Command(ExecuteRefresh);

		void LoadItems()
		{
			for (int i = 0; i < 20; i++)
				Items.Add(new Issue13631Model
				{
					Text1 = $"Lorem ipsum {i + 1}",
					Text2 = "Ut enim ad minim veniam"
				});
		}

		void ExecuteRefresh()
		{
			IsRefreshing = true;

			Items.Clear();
			LoadItems();

			IsRefreshing = false;
		}
	}
}