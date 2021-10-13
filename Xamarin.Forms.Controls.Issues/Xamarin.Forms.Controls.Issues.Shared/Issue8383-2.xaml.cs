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
	[Issue(IssueTracker.None, 0,
		"[Bug] CollectionView infinite scroll does not work with a list on iOS",
		PlatformAffected.iOS)]
	public partial class Issue83832 : TestContentPage
	{
#if APP
		int _remainingItemsThresholdReachedCounter;
#endif
		public Issue83832()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new Issue83832ViewModel();
		}
#if APP
		void CollectionView_RemainingItemsThresholdReached(object sender, System.EventArgs e)
		{
			RemainingThresholdCount.Text = $"RemainingItemsThresholdReached {++_remainingItemsThresholdReachedCounter} time(s)";
		}
#endif
	}

	public class Issue83832ViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<string> ListItems { get; private set; } = new ObservableCollection<string>();

		Command _remainingItemsReachedCommand;
		int _numberOfItems;

		public Issue83832ViewModel()
		{
			Initialize();
		}

		public Command RemainingItemsReachedCommand
		{
			get
			{
				return (_remainingItemsReachedCommand) ?? (_remainingItemsReachedCommand = new Command(() =>
				{
					for (int i = 0; i < 25; i++)
					{
						ListItems.Add(_numberOfItems++.ToString());
					}
				}
				));
			}
		}

		void Initialize()
		{
			for (int i = 0; i < 50; i++)
			{
				ListItems.Add(_numberOfItems++.ToString());
			}
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