using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Widget;

namespace Xamarin.Platform
{
	public class MauiAppCompatActivity<TApplication> : AppCompatActivity where TApplication: App
	{

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
		protected override void OnCreate(Bundle savedInstanceState)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
		{
			base.OnCreate(savedInstanceState);

			TApplication app = (TApplication)Activator.CreateInstance(typeof(TApplication));

			var host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);

			var content = app.GetWindowFor(null!).Page.View;

			CoordinatorLayout parent = new CoordinatorLayout(BaseContext);
			NestedScrollView main = new NestedScrollView(BaseContext);

			SetContentView(parent, new ViewGroup.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));
			
			parent.AddView(main, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
			
			main.AddView(content.ToNative(app.Context!), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

		}

		//configure native services like HandlersContext, ImageSourceHandlers etc.. 
		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
			services.AddSingleton<IHandlersContext>(provider => new HandlersContext(provider, this));
		}
	}
}
