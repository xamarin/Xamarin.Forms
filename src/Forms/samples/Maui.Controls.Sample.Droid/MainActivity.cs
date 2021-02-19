using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Platform;
using System.Linq;

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

			var host= app.CreateBuilder().Build(app);

			var content = app.GetWindowFor(null).Page.View;

			page.AddView(content.ToNative(this), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

		}
	}
}