using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using System;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Widget;
using Google.Android.Material.AppBar;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.Content.Res;
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Xamarin.Platform
{
	public class MauiAppCompatActivity : AppCompatActivity
	{

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
		protected override void OnCreate(Bundle savedInstanceState)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
		{
			base.OnCreate(savedInstanceState);

			if (App.Current as MauiApp == null)
				throw new InvalidOperationException($"App is not {nameof(MauiApp)}");

			var mauiApp = (MauiApp)App.Current;

			if (mauiApp.Services == null)
				throw new InvalidOperationException("App was not initialized");

			var window = mauiApp.GetWindowFor(null!);

			window.HandlersContext = new HandlersContext(mauiApp.Services, this);

			//Hack for now we set this on the App Static but this should be on IFrameworkElement
			App.Current.SetHandlerContext(window.HandlersContext);

			var content = window.Page.View;

			SetContentView(Resource.Layout.activity_main);

			AndroidX.AppCompat.Widget.Toolbar? toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

			var page = FindViewById<ViewGroup>(Resource.Id.Page);

			//CoordinatorLayout parent = new CoordinatorLayout(this);
			//NestedScrollView main = new NestedScrollView(this);

			//SetContentView(parent, new ViewGroup.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));

			//AddToolbar(parent);

			//parent.AddView(main, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

			page.AddView(content.ToNative(window.HandlersContext), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
		}

		void AddToolbar(ViewGroup parent)
		{	
		//	Toolbar toolbar = new Toolbar(this);
			var appbarLayout = new AppBarLayout(this);
			//AndroidX.AppCompat.Widget.Toolbar? toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);

			AToolbar? toolbar = LayoutInflater.Inflate(Resource.Id.toolbar, null).JavaCast<AToolbar>();
			SetSupportActionBar(toolbar);

			//var themeID = global::Android.Resource.Style.ThemeOverlayMaterialDarkActionBar;
		//	var toolbarResource = Resource.Layout.Toolbar;
			//appbarLayout.Context?.SetTheme(themeID);
			appbarLayout.AddView(toolbar, new ViewGroup.LayoutParams(AppBarLayout.LayoutParams.MatchParent, global::Android.Resource.Attribute.ActionBarSize));
			SetSupportActionBar(toolbar);
			parent.AddView(appbarLayout, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

		}
	}
}
