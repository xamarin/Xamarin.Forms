using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.SwipeView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11808, "[Bug] [iOS] CollectionView with SwipeViews in the ItemTemplate leaves some swipe items opened upon refreshing the ItemSource",
		PlatformAffected.iOS)]
	public partial class Issue11808 : TestContentPage
	{
		public Issue11808()
		{
#if APP
			Title = "Issue 11808";
			InitializeComponent();
			BindingContext = this;
#endif
		}

		public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();

		public ICommand RefreshCommand { get; private set; }

		protected override void Init()
		{
			RefreshCommand = new Command(RefreshData);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			RefreshData();
		}

		void RefreshData()
		{
			Items.Clear();

			for (int i = 0; i < 10; i++)
			{
				Items.Add($"Item {i + 1} (Swipe to Left)");
			}
		}
	}
}