using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.TestCasesPages
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2282, "ListView ItemTapped issue on Windows phone", PlatformAffected.WinPhone)]
	public partial class Issue2282 : ContentPage
	{
		int _counter = 0;

		public Issue2282()
		{
			Items = new ObservableCollection<string>();
			InitializeComponent();

			BindingContext = Items;
			MyListView.ItemTapped +=
				(sender, e) => { LogLabel.Text = string.Format("{0} - Item {1} Tapped!", _counter++, (string)e.Item); };
		}

		public ObservableCollection<string> Items { get; set; }

		protected override void OnAppearing()
		{
			Items.Add("First");
			Items.Add("Second");
			Items.Add("Third");
			base.OnAppearing();
		}
	}
#endif
}