using System;
using Android.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Xamarin.Platform
{
	public class MauiApplication<TApplication> : global::Android.App.Application where TApplication : App
	{
		public MauiApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
		{
		
		}
		IHost? _host;
		public override void OnCreate()
		{
			TApplication app = (TApplication)Activator.CreateInstance(typeof(TApplication));
			_host = app.CreateBuilder().ConfigureServices(ConfigureNativeServices).Build(app);
			//_host.Start();
			base.OnCreate();
		}
		
		//configure native services like HandlersContext, ImageSourceHandlers etc.. 
		void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{
		}
	}
}
