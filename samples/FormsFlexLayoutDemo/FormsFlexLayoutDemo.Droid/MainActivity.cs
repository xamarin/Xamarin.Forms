using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace FormsFlexLayoutDemo.Droid
{
	[Activity(Label = "FormsFlexLayoutDemo", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{



            ToolbarResource = Resource.Layout.Toolbar;

            TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			global::Xamarin.Forms.FlexLayout.RegisterEngine(typeof(Xamarin.FlexLayoutEngine.Flex.FlexEngine));
            LoadApplication(new FormsFlexLayoutDemo.App());
		}
	}
}

