using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github , 7051, "[iOS] MasterDetailPage - Master icon disappearing on InsertPageBefore", PlatformAffected.iOS)]
	public class Issue7051 : TestMasterDetailPage
	{
		public ICommand AboutCommand { get; set; }
		public ICommand MainPageCommand { get; set; }

		public Issue7051()
		{
			//AboutCommand = new Command<ContentPage>(async (page) => await About(page));
			//MainPageCommand = new Command<ContentPage>(async (page) => await Main(page));
		}

		protected override void Init()
		{
			AboutCommand = new Command<ContentPage>(async (page) => await About(page));
			MainPageCommand = new Command<ContentPage>(async (page) => await Main(page));
			Master = new MasterPage(AboutCommand, MainPageCommand);
			Detail = new NavigationPage(new MainDetailPage());
		}

		private async Task About(ContentPage page)
		{
			var root = Detail.Navigation.NavigationStack[0];
			Detail.Navigation.InsertPageBefore(page, root);
			await Detail.Navigation.PopToRootAsync();
		}

		private async Task Main(ContentPage page)
		{
			var root = Detail.Navigation.NavigationStack[0];
			Detail.Navigation.InsertPageBefore(page, root);
			await Detail.Navigation.PopToRootAsync();
		}

		class MasterPage : ContentPage
		{
			readonly ICommand _aboutCommand;
			readonly ICommand _mainPageCommand;

			public MasterPage(ICommand aboutCommand, ICommand mainPageCommand)
			{
				Title = "Master";
				var contentStack = new StackLayout();
				contentStack.Spacing = 15;
				contentStack.Margin = new Thickness(0,40,0,0);

				var mainPageLabel = new Label
				{
					Text = "Main Page",
					FontSize = 20,
					TextColor = Color.Black,
					Margin = new Thickness(20)
				};

				var aboutLabel = new Label
				{
					Text = "About",
					FontSize = 20,
					TextColor = Color.Black,
					Margin = new Thickness(20)
				};							
				

				Content = contentStack;
				_aboutCommand = aboutCommand;
				_mainPageCommand = mainPageCommand;
				mainPageLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = _mainPageCommand, CommandParameter = new MainDetailPage() });
				aboutLabel.GestureRecognizers.Add(new TapGestureRecognizer { Command = _aboutCommand, CommandParameter = new AboutDetailPage() });
				contentStack.Children.Add(mainPageLabel);
				contentStack.Children.Add(aboutLabel);
			}
		}

		class MainDetailPage : ContentPage
		{
			public MainDetailPage()
			{
				BackgroundColor = Color.AliceBlue;
			}
		}

		class AboutDetailPage : ContentPage
		{
			public AboutDetailPage()
			{
				BackgroundColor = Color.DarkRed;
			}
		}
	}
}
