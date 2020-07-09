using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Markup;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.None, 11311, "[Regression] CollectionView NSRangeException", PlatformAffected.iOS)]
	public class Issue11311 : TestTabbedPage
	{
		const string Success = "Success";

		protected override void Init()
		{
			Device.SetFlags(new[] { "Markup_Experimental" });

			Children.Add(FirstPage());
			Children.Add(CollectionViewPage());
		}

		ContentPage FirstPage() 
		{
			var firstPage = new ContentPage();
			firstPage.Title = "The first page";

			firstPage.Content = new Label { Text = Success };

			firstPage.Appearing += (sender, args) => {
				if (firstPage.Parent is TabbedPage tabbedPage
				&& tabbedPage.Children[1] is ContentPage collectionViewPage
				&& collectionViewPage.Content is RefreshView refreshView)
				{
					refreshView.IsRefreshing = true;
				}
			};

			return firstPage;
		}

		ContentPage CollectionViewPage()
		{
			BindingContext = new CollectionViewModel();

			var refreshView = new RefreshView
			{
				Content = new CollectionView
				{
					ItemTemplate = new GreenBoxDataTemplate(),
					Footer = new BoxView { BackgroundColor = Color.Red, HeightRequest = 53 }
				}.Bind(CollectionView.ItemsSourceProperty, nameof(CollectionViewModel.ScoreCollectionList))

			}.Bind(RefreshView.CommandProperty, nameof(CollectionViewModel.PopulateCollectionCommand))
			 .Bind(RefreshView.IsRefreshingProperty, nameof(CollectionViewModel.IsRefreshing));

			var page = new ContentPage
			{
				Title = "CollectionView Page",
				Content = refreshView
			};

			return page;
		}

		class CollectionViewModel : INotifyPropertyChanged
		{
			bool _isRefreshing;
			IEnumerable<int> _scoreCollectionList;

			public CollectionViewModel()
			{
				PopulateCollectionCommand = new Command(ExecuteRefreshCommand);
				_scoreCollectionList = Enumerable.Empty<int>();
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public ICommand PopulateCollectionCommand { get; }

			public IEnumerable<int> ScoreCollectionList
			{
				get => _scoreCollectionList;
				set
				{
					_scoreCollectionList = value;
					OnPropertyChanged();
				}
			}

			public bool IsRefreshing
			{
				get => _isRefreshing;
				set
				{
					if (IsRefreshing != value)
					{
						_isRefreshing = value;
						OnPropertyChanged();
					}
				}
			}

			void ExecuteRefreshCommand()
			{
				ScoreCollectionList = Enumerable.Range(0, 100);
				IsRefreshing = false;
			}

			void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		class GreenBoxDataTemplate : DataTemplate
		{
			public GreenBoxDataTemplate() : base(CreateTemplate)
			{

			}

			static View CreateTemplate() => new StackLayout
			{
				Children =
				{
					new BoxView
					{
						BackgroundColor = Color.Black,
						HeightRequest = 5
					},
					new BoxView
					{
						BackgroundColor = Color.Green,
						HeightRequest = 50
					},
					new BoxView
					{
						BackgroundColor = Color.Black,
						HeightRequest = 5
					}
				}
			};
		}

#if UITEST
		[Test]
		public void CollectionViewWithFooterShouldNotCrashOnDisplay()
		{
			// If this hasn't already crashed, the test is passing
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}
