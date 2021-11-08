using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8383,
		"[Bug] CollectionView infinite scroll does not work with a grouped list on iOS",
		PlatformAffected.iOS)]
	public partial class Issue8383 : TestContentPage
	{
#if APP
		int _remainingItemsThresholdReachedCounter;
#endif
		public Issue8383()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new Issue8383ViewModel();
		}
#if APP
		void CollectionView_RemainingItemsThresholdReached(object sender, System.EventArgs e)
		{
			RemainingThresholdCount.Text = $"RemainingItemsThresholdReached {++_remainingItemsThresholdReachedCounter} time(s)";
		}
#endif
	}

	public class GroupedIssue8383 : List<string>
	{
		public GroupedIssue8383(IList<string> items) : base(items)
		{
			Name = $"Group {this.First()} - {this.Last()}";
		}

		public string Name { get; private set; }
	}

	public class Issue8383ViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<GroupedIssue8383> ListItems { get; private set; } = new ObservableCollection<GroupedIssue8383>();

		Command _remainingItemsReachedCommand;
		int _numberOfItems;

		public Issue8383ViewModel()
		{
			Initialize();
		}

		public Command RemainingItemsReachedCommand
		{
			get
			{
				return (_remainingItemsReachedCommand) ?? (_remainingItemsReachedCommand = new Command(() =>
				{
					var numbersList = new List<string>();
					for (int i = 0; i < 25; i++)
					{
						numbersList.Add(_numberOfItems++.ToString());
					}

					ListItems.Add(new GroupedIssue8383(numbersList));
				}
				));
			}
		}

		private void Initialize()
		{
			var numbersList = new List<string>();

			for (int i = 0; i < 50; i++)
			{
				numbersList.Add(_numberOfItems++.ToString());
			}

			ListItems.Add(new GroupedIssue8383(numbersList));
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}