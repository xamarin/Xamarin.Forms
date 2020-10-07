using Foundation;
using UIKit;
using Xamarin.Platform;

using Xamarin.Forms;
#if !NET6_0
using Xamarin.Forms.Platform.iOS;
#endif

using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Platform.Hosting;
using Sample.Services;

namespace Sample.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
#if !NET6_0
	public class AppDelegate : FormsApplicationDelegate, IUIApplicationDelegate
#else
	public class AppDelegate : UIApplicationDelegate, IUIApplicationDelegate
#endif
	{
		UIWindow _window;

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			_window = new UIWindow();

			var (host, app) = App.CreateDefaultBuilder()
							//.RegisterHandler<IButton, CustomHandlers.CustomPinkTextButtonHandler>()
							//.RegisterHandlers(new Dictionary<Type, Type>
							//{
							//	{ typeof(Xamarin.Platform.VerticalStackLayout),typeof(LayoutHandler) },
							//	{ typeof(Xamarin.Platform.HorizontalStackLayout),typeof(LayoutHandler) },
							//	{ typeof(Xamarin.Forms.FlexLayout),typeof(LayoutHandler) },
							//	{ typeof(Xamarin.Forms.StackLayout),typeof(LayoutHandler) },
							//})
							//.ConfigureServices(ConfigureExtraServices)
							.Init<MyApp>();

			var page = app.GetStartup() as Pages.MainPage;

			_window.RootViewController = new UIViewController
			{
				View = page.GetContentView().ToNative()
			};

			_window.MakeKeyAndVisible();

			// In 5 seconds, add and remove some controls so we can see that working
			Task.Run(async () => {

				await Task.Delay(5000).ConfigureAwait(false);

				void addLabel()
				{
					(content as VerticalStackLayout).Add(new Label { Text = "I show up after 5 seconds" });
					var first = (content as VerticalStackLayout).Children.First();
					(content as VerticalStackLayout).Remove(first); 
				};

				_window.BeginInvokeOnMainThread(addLabel);
			});

			return true;
		}

		void ConfigureExtraServices(HostBuilderContext ctx, IServiceCollection services)
		{
			if (ctx.HostingEnvironment.IsDevelopment())
			{
				System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
				{
					if (certificate.Issuer.Equals("CN=localhost"))
						return true;
					return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
				};
			}

			services.AddSingleton<ITextService, Services.iOSTextService>();
		}

		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message)
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background execution this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transition from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive.
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}
}