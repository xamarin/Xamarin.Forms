using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Platform;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.Droid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

			var page = FindViewById<ViewGroup>(Resource.Id.Page);

			App app = new MyApp();

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			var content = app.GetWindowFor(null).Page.View;

			page.AddView(content.ToNative(app.Context), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

		}
		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddSingleton<IHandlersContext>(provider => new HandlersContext(provider, this));
		}
	}
}