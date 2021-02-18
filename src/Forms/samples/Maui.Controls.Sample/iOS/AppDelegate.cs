using Foundation;
using UIKit;
using Xamarin.Platform;

#if !NET6_0
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
#endif

using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Maui.Controls.Sample.Services;

namespace Maui.Controls.Sample.iOS
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

			var app = new MyApp();

			var builder = app.Builder();

			var host = builder.BuildApp(app);

			var page = app.GetWindowFor(null).Page;

			var content = page.View;

			_window.RootViewController = new UIViewController
			{
				View = content.ToNative()
			};

			_window.MakeKeyAndVisible();

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

			services.AddSingleton<ITextService, iOSTextService>();
		}

		class iOSTextService : ITextService
		{
			public string GetText() => "iOS Text Service";
		}
	}
}