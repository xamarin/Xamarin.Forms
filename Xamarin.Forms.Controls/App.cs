using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms.Controls.GalleryPages.RadioButtonGalleries;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls
{
	public class Bible : ContentPage
	{
		ScrollView scrollview;

		public Bible()
		{
			scrollview = new ScrollView();
			var layut = new StackLayout();
			var label = new Label();
			label.Text = "Adapted from \"The Colors of Animals\" by Sir John Lubbock in A Book of Natural History (1902, ed. David Starr Jordan)  The color of animals is by no means a matter of chance; it depends on many considerations, but in the majority of cases tends to protect the animal from danger by rendering it less conspicuous. Perhaps it may be said that if coloring is mainly protective, there ought to be but few brightly colored animals.There are, however, not a few cases in which vivid colors are themselves protective. The kingfisher itself, though so brightly colored, is by no means easy to see. The blue harmonizes with the water, and the bird as it darts along the stream looks almost like a flash of sunlight.  Desert animals are generally the color of the desert.Thus, for instance, the lion, the antelope, and the wild donkey are all sand - colored. “Indeed,” says Canon Tristram, “in the desert, where neither trees, brushwood, nor even undulation of the surface afford the slightest protection to its foes, a modification of color assimilated to that of the surrounding country is absolutely necessary. Hence, without exception, the upper plumage of every bird, and also the fur of all the smaller mammals and the skin of all the snakes and lizards, is of one uniform sand color.”  The next point is the color of the mature caterpillars, some of which are brown.This probably makes the caterpillar even more conspicuous among the green leaves than would otherwise be the case. Let us see, then, whether the habits of the insect will throw any light upon the riddle.What would you do if you were a big caterpillar? Why, like most other defenseless creatures, you would feed by night, and lie concealed by day. So do these caterpillars. When the morning light comes, they creep down the stem of the food plant, and lie concealed among the thick herbage and dry sticks and leaves, near the ground, and it is obvious that under such circumstances the brown color really becomes a protection. It might indeed be argued that the caterpillars, having become brown, concealed themselves on the ground, and that we were reversing the state of things. But this is not so, because, while we may say as a general rule that large caterpillars feed by night and lie concealed by day, it is by no means always the case that they are brown; some of them still retaining the green color. We may then conclude that the habit of concealing themselves by day came first, and that the brown color is a later adaptation.  The example of the mature caterpillar in the third paragraph is primarily intended to demonstrate _____________.";

			var button = new Button
			{	
				Text = "Click me"
			};
			button.Clicked += Button_Clicked;
			layut.Children.Add(label);
			layut.Children.Add(button);
			scrollview.Content = layut;
			Content = scrollview;
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			scrollview.ScrollToAsync(0, 0, false);
		}
	}
	public class App : Application
	{
		public const string AppName = "XamarinFormsControls";

		// ReSharper disable once InconsistentNaming
		public static int IOSVersion = -1;

		public static List<string> AppearingMessages = new List<string>();

		static Dictionary<string, string> s_config;
		readonly ITestCloudService _testCloudService;

		public const string DefaultMainPageId = "ControlGalleryMainPage";

		public static bool PreloadTestCasesIssuesList { get; set; } = true;
		public App()
		{
			_testCloudService = DependencyService.Get<ITestCloudService>();

			var bible = new Bible();
			SetMainPage(CreateDefaultMainPage());

			//TestMainPageSwitches();

			//SetMainPage(new ImageSourcesGallery());
		}

		protected override void OnStart()
		{
			//TestIssue2393();
		}

		async Task TestBugzilla44596()
		{
			await Task.Delay(50);
			// verify that there is no gray screen displayed between the blue splash and red FlyoutPage.
			SetMainPage(new Bugzilla44596SplashPage(async () =>
			{
				var newTabbedPage = new TabbedPage();
				newTabbedPage.Children.Add(new ContentPage { BackgroundColor = Color.Red, Content = new Label { Text = "Success" } });
				MainPage = new FlyoutPage
				{
					Flyout = new ContentPage { Title = "Flyout", BackgroundColor = Color.Red },
					Detail = newTabbedPage
				};

				await Task.Delay(50);
				SetMainPage(CreateDefaultMainPage());
			}));
		}

		async Task TestBugzilla45702()
		{
			await Task.Delay(50);
			// verify that there is no crash when switching MainPage from MDP inside NavPage
			SetMainPage(new Bugzilla45702());
		}

		void TestIssue2393()
		{
			MainPage = new NavigationPage();

			// Hand off to website for sign in process
			var view = new WebView { Source = new Uri("http://google.com") };
			view.Navigated += (s, e) => MainPage.DisplayAlert("Navigated", $"If this popup appears multiple times, this test has failed", "ok");
			;

			MainPage.Navigation.PushAsync(new ContentPage { Content = view, Title = "Issue 2393" });
			//// Uncomment to verify that there is no gray screen displayed between the blue splash and red FlyoutPage.
			//SetMainPage(new Bugzilla44596SplashPage(() =>
			//{
			//	var newTabbedPage = new TabbedPage();
			//	newTabbedPage.Children.Add(new ContentPage { BackgroundColor = Color.Red, Content = new Label { Text = "yay" } });
			//	MainPage = new FlyoutPage
			//	{
			//		Flyout = new ContentPage { Title = "Flyout", BackgroundColor = Color.Red },
			//		Detail = newTabbedPage
			//	};
			//}));

			//// Uncomment to verify that there is no crash when switching MainPage from MDP inside NavPage
			//SetMainPage(new Bugzilla45702());

			//// Uncomment to verify that there is no crash when rapidly switching pages that contain lots of buttons
			//SetMainPage(new Issues.Issue2004());
		}

		async Task TestMainPageSwitches()
		{
			await TestBugzilla45702();

			await TestBugzilla44596();
		}

		public Page CreateDefaultMainPage()
		{
			var layout = new StackLayout { BackgroundColor = Color.Red };
			layout.Children.Add(new Label { Text = "This is master Page" });
			var master = new ContentPage { Title = "Flyout", Content = layout, BackgroundColor = Color.SkyBlue, IconImageSource = "menuIcon" };
			master.On<iOS>().SetUseSafeArea(true);
			var mdp = new FlyoutPage
			{
				AutomationId = DefaultMainPageId,
				Flyout = master,
				Detail = CoreGallery.GetMainPage()
			};
			master.IconImageSource.AutomationId = "btnMDPAutomationID";
			mdp.SetAutomationPropertiesName("Main page");
			mdp.SetAutomationPropertiesHelpText("Main page help text");
			mdp.Flyout.IconImageSource.SetAutomationPropertiesHelpText("This as MDP icon");
			mdp.Flyout.IconImageSource.SetAutomationPropertiesName("MDPICON");
			return mdp;
			//return new XamStore.StoreShell();
		}

		protected override void OnAppLinkRequestReceived(Uri uri)
		{
			var appDomain = "http://" + AppName.ToLowerInvariant() + "/";

			if (!uri.ToString().ToLowerInvariant().StartsWith(appDomain))
				return;

			var url = uri.ToString().Replace(appDomain, "");

			var parts = url.Split('/');
			if (parts.Length == 2)
			{
				var isPage = parts[0].Trim().ToLower() == "gallery";
				if (isPage)
				{
					string page = parts[1].Trim();
					var pageForms = Activator.CreateInstance(Type.GetType(page));

					if (pageForms is AppLinkPageGallery appLinkPageGallery)
					{
						appLinkPageGallery.ShowLabel = true;
						(MainPage as FlyoutPage)?.Detail.Navigation.PushAsync((pageForms as Page));
					}
				}
			}

			base.OnAppLinkRequestReceived(uri);
		}

		public static Dictionary<string, string> Config
		{
			get
			{
				if (s_config == null)
					LoadConfig();

				return s_config;
			}
		}

		public static ContentPage MenuPage { get; set; }

		public void SetMainPage(Page rootPage)
		{
			MainPage = rootPage;
		}

		static Assembly GetAssembly(out string assemblystring)
		{
			assemblystring = typeof(App).AssemblyQualifiedName.Split(',')[1].Trim();
			var assemblyname = new AssemblyName(assemblystring);
			return Assembly.Load(assemblyname);
		}

		static void LoadConfig()
		{
			s_config = new Dictionary<string, string>();

			string keyData = LoadResource("controlgallery.config").Result;
			string[] entries = keyData.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string entry in entries)
			{
				string[] parts = entry.Split(':');
				if (parts.Length < 2)
					continue;

				s_config.Add(parts[0].Trim(), parts[1].Trim());
			}
		}

		static async Task<string> LoadResource(string filename)
		{
			Assembly assembly = GetAssembly(out string assemblystring);

			Stream stream = assembly.GetManifestResourceStream($"{assemblystring}.{filename}");
			string text;
			using (var reader = new StreamReader(stream))
				text = await reader.ReadToEndAsync();
			return text;
		}

		public bool NavigateToTestPage(string test)
		{
			try
			{
				// Create an instance of the main page
				var root = CreateDefaultMainPage();

				// Set up a delegate to handle the navigation to the test page
				EventHandler toTestPage = null;

				toTestPage = async delegate (object sender, EventArgs e)
				{
					await Current.MainPage.Navigation.PushModalAsync(TestCases.GetTestCases());
					TestCases.TestCaseScreen.PageToAction[test]();
					Current.MainPage.Appearing -= toTestPage;
				};

				// And set that delegate to run once the main page appears
				root.Appearing += toTestPage;

				SetMainPage(root);

				return true;
			}
			catch (Exception ex)
			{
				Log.Warning("UITests", $"Error attempting to navigate directly to {test}: {ex}");

			}

			return false;
		}

		public void Reset()
		{
			SetMainPage(CreateDefaultMainPage());
		}

		public void PlatformTest()
		{
			SetMainPage(new GalleryPages.PlatformTestsGallery.PlatformTestsConsole());
		}
	}
}
