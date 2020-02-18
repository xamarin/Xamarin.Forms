using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9013, "[Bug] [UWP] RemainingItemsThresholdReachedCommand in CollectionView is not working", PlatformAffected.UWP)]
	public partial class Issue9013 : TestContentPage
	{
		internal const string RunTestButtonText = "Run test";
		internal const string Success = "Success";

		ViewModelIssue9013 _viewModel;

		public Issue9013()
        {
#if APP
	        InitializeComponent();

			SetRunButtonClickHandler();
#endif
		}

		protected override void Init()
        {
			_viewModel = new ViewModelIssue9013();
			BindingContext = _viewModel;

#if UITEST
	        SetRunButtonClickHandler();
#endif
        }

        void SetRunButtonClickHandler()
        {
	        var grid = (Grid)Content;
	        var stackLayout = (StackLayout)grid.Children.First();
	        var button = (Button)stackLayout.Children.First();

	        button.Clicked += OnRunButtonClicked;
		}

        void OnRunButtonClicked(object sender, EventArgs e)
        {
	        var grid = (Grid)Content;
	        var collectionView = (CollectionView)grid.Children.Single(v => v is CollectionView);

	        collectionView.ScrollTo(_viewModel.ItemsCount - _viewModel.RemainingItemsThreshold - 1);
        }

#if UITEST
        [Test]
		[NUnit.Framework.Category(UITestCategories.CollectionView)]
        public void RemainingItemsThresholdReachedCommandIsWorking()
        {
			RunningApp.WaitForElement(RunTestButtonText);
			RunningApp.Tap(RunTestButtonText);
			RunningApp.WaitForElement(Success);
		}
#endif
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue9013 : INotifyPropertyChanged
	{
		int _countOfGroupsInBatch = 5;
		int _countOfItemsInGroup = 10;
		int _itemCounter = -1;

		string _testResult;

		public string TestResult
		{
			get
			{
				return _testResult;
			}
			set
			{
				if (_testResult == value)
				{
					return;
				}

				_testResult = value;
				OnPropertyChanged();
			}
		}

		public string RunTestButtonText => Issue9013.RunTestButtonText;

		public ObservableCollection<GroupIssue9013> Groups { get; set; }

		public int RemainingItemsThreshold { get; set; }

		public int ItemsCount => _itemCounter + 1;

		public Command ThresholdReachedCommand { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public ViewModelIssue9013()
		{
			TestResult = string.Empty;
			Groups = new ObservableCollection<GroupIssue9013>();
			ThresholdReachedCommand = new Command(ExecuteThresholdReachedCommand);
			RemainingItemsThreshold = 10;
			GenerateItems();
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void ExecuteThresholdReachedCommand()
		{
			GenerateItems();
			TestResult = Issue9013.Success;
		}

		void GenerateItems()
		{
			for (int i = 0; i < _countOfGroupsInBatch; i++)
			{
				var group = new GroupIssue9013();

				for (int j = 0; j < _countOfItemsInGroup; j++)
				{
					_itemCounter++;
					group.Add(new ItemIssue9013 { Text = $"Item index: {_itemCounter}" });
				}

				Groups.Add(group);
			}
		}
	}

	[Preserve(AllMembers = true)]
	public class ItemIssue9013
	{
		public string Text { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class GroupIssue9013 : ObservableCollection<ItemIssue9013>
	{
		public string Title => "Group title";
	}
}