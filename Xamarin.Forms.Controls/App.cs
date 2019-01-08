using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms.Controls.Issues;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls
{

	public class App : Application
	{
		public const string AppName = "XamarinFormsControls";
		static string s_insightsKey;

		// ReSharper disable once InconsistentNaming
		public static int IOSVersion = -1;

		public static List<string> AppearingMessages = new List<string>();

		static Dictionary<string, string> s_config;
		readonly ITestCloudService _testCloudService;

		public const string DefaultMainPageId = "ControlGalleryMainPage";

		public App()
		{
			_testCloudService = DependencyService.Get<ITestCloudService>();

			SetMainPage(CreateDefaultMainPage());

			//TestMainPageSwitches();

			//SetupImagesTests();
		}

		private void SetupImagesTests()
		{
			MainPage = new NavigationPage(CreateRootPage());

			Page CreateRootPage()
			{
				bool? toolbarIcon = null;
				bool? titleIcon = null;
				bool? backgroundImage = null;
				ToolbarItem toolbarItem;

				ContentPage thisPage = null;
				thisPage = new ContentPage
				{
					Title = "Image Source Tests",
					ToolbarItems =
					{
						(toolbarItem = new ToolbarItem("MENU", null, delegate { }))
					},
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							Padding = 20,
							Children =
							{
								new Button
								{
									Text = "Toggle Title Icon",
									Command = new Command(() =>
									{
										if (titleIcon == null)
										{
											titleIcon = true;
											NavigationPage.SetTitleIcon(thisPage, "bank.png");
										}
										else if (titleIcon == true)
										{
											titleIcon = false;
											NavigationPage.SetTitleIcon(thisPage, "calculator.png");
										}
										else
										{
											titleIcon = null;
											NavigationPage.SetTitleIcon(thisPage, null);
										}
									})
								},
								new Button
								{
									Text = "Toggle Menu Icon",
									Command = new Command(() =>
									{
										if (toolbarIcon == null)
										{
											toolbarIcon = true;
											toolbarItem.Icon = "bank.png";
										}
										else if (toolbarIcon == true)
										{
											toolbarIcon = false;
											toolbarItem.Icon = "calculator.png";
										}
										else
										{
											toolbarIcon = null;
											toolbarItem.Icon = null;
										}
									})
								},
								new Button
								{
									Text = "Toggle Background",
									Command = new Command(() =>
									{
										if (backgroundImage == null)
										{
											backgroundImage = true;
											thisPage.BackgroundImage = "photo.jpg";
										}
										else if (backgroundImage == true)
										{
											backgroundImage = false;
											thisPage.BackgroundImage = "oasis.jpg";
										}
										else
										{
											backgroundImage = null;
											thisPage.BackgroundImage = null;
										}
									})
								},
								new Button
								{
									Text = "ListView Context Actions",
									Command = new Command(() => MainPage.Navigation.PushAsync(CreateListViewContextActionsPage()))
								},
								new Button
								{
									Text = "Image View",
									Command = new Command(() => MainPage.Navigation.PushAsync(CreateImageViewPage()))
								},
								new Button
								{
									Text = "Buttons",
									Command = new Command(() => MainPage.Navigation.PushAsync(CreateButtonsPage()))
								},
								new Button
								{
									Text = "Slider",
									Command = new Command(() => MainPage.Navigation.PushAsync(CreateSliderPage()))
								},
							}
						}
					}
				};
				return thisPage;
			}

			Page CreateListViewContextActionsPage()
			{
				return new ContentPage
				{
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							Padding = 20,
							Children =
							{
								new Label
								{
									Text = "Each of the items should have the 'bank.png' as the context menu icon.",
									LineBreakMode = LineBreakMode.WordWrap,
								},
								new ListView
								{
									Margin = new Thickness(-20, 0, -20, -20),
									ItemsSource = new[] { "one", "two", "three", "four", "five" },
									ItemTemplate = new DataTemplate(() =>
									{
										var cell = new TextCell();
										cell.ContextActions.Add(new MenuItem
										{
											Text = "bank",
											Icon = "bank.png"
										});
										cell.SetBinding(TextCell.TextProperty, new Binding("."));
										return cell;
									}),
								}
							}
						}
					}
				};
			}

			Page CreateImageViewPage()
			{
				Image image = null;
				ActivityIndicator loading = null;
				var page = new ContentPage
				{
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							Padding = 20,
							Children =
							{
								new Label
								{
									Text = "Tap the buttons to swap out the images.",
									LineBreakMode = LineBreakMode.WordWrap,
								},
								new Grid
								{
									Children =
									{
										(image = new Image
										{
											WidthRequest = 200,
											HeightRequest = 200,
											Source = "bank.png",
										}),
										(loading = new ActivityIndicator()),
									}
								},
								new Button
								{
									Text = "Clear Image",
									Command = new Command(() => image.Source = null)
								},
								new Button
								{
									Text = "Resource Image",
									Command = new Command(() => image.Source = ImageSource.FromFile("bank.png"))
								},
								new Button
								{
									Text = "Embedded Image",
									Command = new Command(() => image.Source = ImageSource.FromResource("Xamarin.Forms.Controls.GalleryPages.crimson.jpg", typeof(App)))
								},
								new Button
								{
									Text = "Stream Image",
									Command = new Command(() => image.Source = ImageSource.FromStream(() => typeof(App).Assembly.GetManifestResourceStream("Xamarin.Forms.Controls.coffee.png")))
								},
								new Button
								{
									Text = "URI Image",
									Command = new Command(() => image.Source = new UriImageSource
									{
										Uri = new Uri("https://raw.githubusercontent.com/xamarin/Xamarin.Forms/master/banner.png"),
										CachingEnabled = false
									})
								},
								new Button
								{
									Text = "Font Image",
									Command = new Command(() =>
									{
										var fontFamily = "";
										switch (Device.RuntimePlatform)
										{
											case Device.iOS:
												fontFamily = "Ionicons";
												break;
											case Device.UWP:
												fontFamily = "Assets/Fonts/ionicons.ttf#ionicons";
												break;
											case Device.Android:
											default:
												fontFamily = "fonts/ionicons.ttf#";
												break;
										}
										image.Source = new FontImageSource
										{
											Color = Color.Black,
											FontFamily = fontFamily,
											Glyph = "\uf233",
											Size = 100,
										};
									})
								},
							}
						}
					}
				};
				loading.SetBinding(ActivityIndicator.IsRunningProperty, new Binding(Image.IsLoadingProperty.PropertyName));
				loading.SetBinding(ActivityIndicator.IsVisibleProperty, new Binding(Image.IsLoadingProperty.PropertyName));
				loading.BindingContext = image;
				return page;
			}

			Page CreateButtonsPage()
			{
				return new ContentPage
				{
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							Padding = 20,
							Children =
							{
								new Label
								{
									Text = "The default Button type.",
									LineBreakMode = LineBreakMode.WordWrap,
								},
								new Button
								{
									Text = "Image & Text",
									Image = "bank.png"
								},
								new Button
								{
									Image = "bank.png"
								},
								new Button
								{
									Text = "Just Text",
									Image = null
								},
								new Label
								{
									Text = "The ImageButton type.",
									LineBreakMode = LineBreakMode.WordWrap,
								},
								new ImageButton
								{
									HeightRequest = 100,
									Source = "bank.png"
								},
								new ImageButton
								{
									HeightRequest = 100,
									Source = new UriImageSource
									{
										Uri = new Uri("https://raw.githubusercontent.com/xamarin/Xamarin.Forms/master/banner.png"),
										CachingEnabled = false
									}
								},
							}
						}
					}
				};
			}

			Page CreateSliderPage()
			{
				Slider slider = null;
				return new ContentPage
				{
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							Padding = 20,
							Children =
							{
								new Label
								{
									Text = "Tap the buttons to swap out the thumb image.",
									LineBreakMode = LineBreakMode.WordWrap,
								},
								(slider = new Slider
								{
									Minimum = 0,
									Maximum = 1,
									Value = 0.5,
									HeightRequest = 50
								}),
								new Button
								{
									Text = "Bank",
									Command = new Command(() => slider.ThumbImage = "bank.png")
								},
								new Button
								{
									Text = "Calculator",
									Command = new Command(() => slider.ThumbImage = "calculator.png")
								},
								new Button
								{
									Text = "<none>",
									Command = new Command(() => slider.ThumbImage = null)
								},
							}
						}
					}
				};
			}
		}

		protected override void OnStart()
		{
			//TestIssue2393();
		}

		async Task TestBugzilla44596()
		{
			await Task.Delay(50);
			// verify that there is no gray screen displayed between the blue splash and red MasterDetailPage.
			SetMainPage(new Bugzilla44596SplashPage(async () =>
			{
				var newTabbedPage = new TabbedPage();
				newTabbedPage.Children.Add(new ContentPage { BackgroundColor = Color.Red, Content = new Label { Text = "Success" } });
				MainPage = new MasterDetailPage
				{
					Master = new ContentPage { Title = "Master", BackgroundColor = Color.Red },
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
			view.Navigated += (s, e) => MainPage.DisplayAlert("Navigated", $"If this popup appears multiple times, this test has failed", "ok"); ;

			MainPage.Navigation.PushAsync(new ContentPage { Content = view, Title = "Issue 2393" });
			//// Uncomment to verify that there is no gray screen displayed between the blue splash and red MasterDetailPage.
			//SetMainPage(new Bugzilla44596SplashPage(() =>
			//{
			//	var newTabbedPage = new TabbedPage();
			//	newTabbedPage.Children.Add(new ContentPage { BackgroundColor = Color.Red, Content = new Label { Text = "yay" } });
			//	MainPage = new MasterDetailPage
			//	{
			//		Master = new ContentPage { Title = "Master", BackgroundColor = Color.Red },
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
			var master = new ContentPage { Title = "Master", Content = layout, BackgroundColor = Color.SkyBlue, Icon ="menuIcon" };
			master.On<iOS>().SetUseSafeArea(true);
			var mdp = new MasterDetailPage
			{
				AutomationId = DefaultMainPageId,
				Master = master,
				Detail = CoreGallery.GetMainPage()
			};
			master.Icon.AutomationId = "btnMDPAutomationID";
			mdp.SetAutomationPropertiesName("Main page");
			mdp.SetAutomationPropertiesHelpText("Main page help text");
			mdp.Master.Icon.SetAutomationPropertiesHelpText("This as MDP icon");
			mdp.Master.Icon.SetAutomationPropertiesName("MDPICON");
			return mdp;

			//Device.SetFlags(new[] { "Shell_Experimental" });
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
						(MainPage as MasterDetailPage)?.Detail.Navigation.PushAsync((pageForms as Page));
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
	}
}