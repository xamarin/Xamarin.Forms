using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Platform;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sample.Services;
using Xamarin.Platform.Handlers;
using Xamarin.Forms;

namespace Sample.Droid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		ViewGroup _page;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

			_page = FindViewById<ViewGroup>(Resource.Id.Page);

			IView content;
#if __REGISTRAR__
			Platform.Init();
			var page = new Pages.MainPage(null);
			content = page.View;
#else
			var app = App.CreateDefaultBuilder()
							//.RegisterHandlers(new Dictionary<Type, Type>
							//{
							//	{ typeof(Xamarin.Platform.VerticalStackLayout),typeof(LayoutHandler) },
							//	{ typeof(Xamarin.Platform.HorizontalStackLayout),typeof(LayoutHandler) },
							//	{ typeof(Xamarin.Forms.FlexLayout),typeof(LayoutHandler) },
							//	{ typeof(Xamarin.Forms.StackLayout),typeof(LayoutHandler) },
							//})
							//.ConfigureServices(ConfigureExtraServices)
							.Init<MyApp>();

			var page = app.Windows.FirstOrDefault()?.Page;
			//var window = new Sample.MainWindow(new Pages.MainPage(null));
			//content = window.Page.View;
			content = page.View;
#endif
			
			Add(content);

			// In 5 seconds, add and remove some controls so we can see that working
			Task.Run(async () => {

				await Task.Delay(5000).ConfigureAwait(false);

				void addLabel()
				{
					(content as VerticalStackLayout).Add(new Label { Text = "I show up after 5 seconds" });
					var first = (content as VerticalStackLayout).Children.First();
					(content as VerticalStackLayout).Remove(first);
				};

				new Handler(Looper.MainLooper).Post(addLabel);

			});
		}

		void ConfigureExtraServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddSingleton<ITextService, Services.DroidTextService>();
		}

		void Add(params IView[] views)
		{
			foreach (var view in views)
			{
				_page.AddView(view.ToNative(this), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent , ViewGroup.LayoutParams.MatchParent));
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}