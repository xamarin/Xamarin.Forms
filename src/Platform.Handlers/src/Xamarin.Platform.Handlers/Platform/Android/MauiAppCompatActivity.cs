using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using System;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Widget;

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

			CoordinatorLayout parent = new CoordinatorLayout(BaseContext);
			NestedScrollView main = new NestedScrollView(BaseContext);

			SetContentView(parent, new ViewGroup.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));
			
			parent.AddView(main, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
			
			main.AddView(content.ToNative(window.HandlersContext), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
		}
	}
}
