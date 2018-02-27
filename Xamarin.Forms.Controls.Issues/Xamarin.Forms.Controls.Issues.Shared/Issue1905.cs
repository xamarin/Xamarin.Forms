using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls.TestCasesPages
{

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1905, "AlertView doesn't scroll when text is to large", PlatformAffected.iOS | PlatformAffected.Android)]
	public class Issue1905 : ContentPage
	{

		public Issue1905()
		{
			var btn = new Button { Text = "Show alert" };
			btn.Clicked += async (object sender, EventArgs e) =>
			{
				await DisplayAlert("Long Message", "Start - kajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfjkajsd;lfkjals;kddfjkl;asdadjsf;lkasjdfl;kasjdf;lkajsd;flkjasd;flkjasd;lkdfjkakls;dfjkl;asdjfkl;asdjfl;kasdjadsfkl;asjdfl;kajadjs;flkjasdl;fklf;jas;dlkjf;alskdjdfakl;sddfjsakls;djfjl;aksdadfjsl;kasdjadsfl;kasdadfjskl;asjdfl;kasdjfl;kasdjfl;kasjdfl;kasjdfkl;asjdfl;kasjdfl;kasjdfl;kasjdfkl;ajsdl;kfkl;jsadkl;fjklasl;dkfjals;kdfjals;kdjfals;kdfalksjdflkajsd;lkfjasl;dkdfjk;alskdjdfjal;skjdfl;askjdfl;aksjdfakl;sdjfkl;asjdfkl;asjdfl;kajsdfl;kajsdfl;kajsdl;ffkl;jasdl;kkl;dfjadls;kdfjkals;kdjkadfsal;ksdjdfal;skddfjal;skdfjal;skdjfal;ksdfjakls;djdfakls;dfjl;askdfjal;skdjfakls;dfj - End", "Ok", "Cancel");
			};

			Content = btn;
		}



		protected override void OnAppearing()
		{

			base.OnAppearing();
		}

	}


	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1905, "Pull to refresh doesn't work if iOS 11 large titles is enabled", PlatformAffected.iOS, NavigationBehavior.PushModalAsync, issueTestNumber: 1)]
	public class Issue1905LargeTitles : TestNavigationPage
	{
		protected override void Init()
		{
			On<iOS>().SetPrefersLargeTitles(true);
		
			var page = new ContentPage
			{
				Title = "Pull Large Titles"
			};
			var lst = new ListView();
			lst.ItemsSource = new string[] { "pull to", "refresh" };
			lst.IsPullToRefreshEnabled = true;
			lst.RefreshCommand = new Command(async () =>
			{
				await Task.Delay(5000);
				lst.ItemsSource = new string[] { $"data {DateTime.Now.Second}", "refreshed" };
				lst.EndRefresh();
			});
			page.Content = lst;
			page.Appearing += async (sender, e) =>
			{
				await Task.Delay(500);
				lst.BeginRefresh();
			};
			page.ToolbarItems.Add(new ToolbarItem { Text = "Refresh", Command = new Command((obj) => lst.BeginRefresh()) });
			Navigation.PushAsync(page);

		}
	}
}
