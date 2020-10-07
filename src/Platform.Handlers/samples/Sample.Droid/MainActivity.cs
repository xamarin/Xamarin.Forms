using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Platform;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sample.Services;
using Xamarin.Platform.Hosting;
using Xamarin.Platform.Handlers;

namespace Sample.Droid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		ViewGroup _page;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			//Xamarin.Essentials.Platform.Init(this, savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

			_page = FindViewById<ViewGroup>(Resource.Id.Page);


#if __REGISTRAR__
			UseRegistrar();
			Add(GetContentView());
#else
			var (host,app) = App.CreateDefaultBuilder()
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
			Add(page.GetContentView());
#endif

		}

		static void UseRegistrar()
		{
			Xamarin.Platform.Registrar.Handlers.Register<IButton, ButtonHandler>();
			Xamarin.Platform.Registrar.Handlers.Register<Xamarin.Platform.VerticalStackLayout, LayoutHandler>();
			Xamarin.Platform.Registrar.Handlers.Register<Xamarin.Platform.HorizontalStackLayout, LayoutHandler>();
			Xamarin.Platform.Registrar.Handlers.Register<Xamarin.Forms.FlexLayout, LayoutHandler>();
			Xamarin.Platform.Registrar.Handlers.Register<Xamarin.Forms.StackLayout, LayoutHandler>();
			//RegistrarHandlers.Handlers.Register<Entry, EntryHandler>();
			Xamarin.Platform.Registrar.Handlers.Register<Label, LabelHandler>();
		}

		static IView GetContentView()
		{
			var verticalStack = new VerticalStackLayout() { Spacing = 5, BackgroundColor = Xamarin.Forms.Color.AntiqueWhite };
			var horizontalStack = new HorizontalStackLayout() { Spacing = 2 };

			var label = new Label { Text = "This top part is a Xamarin.Platform.VerticalStackLayout" };

			verticalStack.Add(label);

			var button = new Xamarin.Forms.Button();
			var button2 = new Button()
			{
				Color = Xamarin.Forms.Color.Green,
				Text = "Hello I'm a button",
				BackgroundColor = Xamarin.Forms.Color.Purple
			};

			horizontalStack.Add(button);
			horizontalStack.Add(button2);
			horizontalStack.Add(new Label { Text = "And these buttons are in a HorizontalStackLayout" });

			verticalStack.Add(horizontalStack);

			return verticalStack;
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