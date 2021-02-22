using System;
using Android.Content;
using Xamarin.Platform.Hosting;
using AView = Android.Views.View;

namespace Xamarin.Platform
{
	public static class HandlerExtensions
	{
		//this will get removed
		public static AView ToNativeOld(this IView view, Context context)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));
			_ = context ?? throw new ArgumentNullException(nameof(context));

			var handler = view.Handler;

			if (handler == null)
			{

				handler = Registrar.Handlers.GetHandler(view.GetType());

				if (handler == null)
					throw new Exception($"Handler not found for view {view}");

				if (handler is IAndroidViewHandler ahandler)
					ahandler.SetContext(context);

				view.Handler = handler;
			}

			handler.SetVirtualView(view);

			if (!(handler.NativeView is AView result))
			{
				throw new InvalidOperationException($"Unable to convert {view} to {typeof(AView)}");
			}

			return result;
		}

		public static AView ToNative(this IView view, IMauiContext context)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));
			_ = context ?? throw new ArgumentNullException(nameof(context));

			var handler = view.Handler;

			if (handler == null)
			{

				if (context.Handlers != null)
				{
					handler = context.Handlers.GetHandler(view.GetType());
				}
				else
				{
					handler = Registrar.Handlers.GetHandler(view.GetType());
				}

				if (handler == null)
					throw new Exception($"Handler not found for view {view}");

				if (handler is IAndroidViewHandler ahandler)
					ahandler.SetContext(context.Context);

				view.Handler = handler;
			}

			handler.SetVirtualView(view);

			if (!(handler.NativeView is AView result))
			{
				throw new InvalidOperationException($"Unable to convert {view} to {typeof(AView)}");
			}

			return result;
		}
	}
}