using System;
using UIKit;
using Xamarin.Platform.Hosting;

namespace Xamarin.Platform
{
	public static class HandlerExtensions
	{
		public static UIView ToNative(this IView view)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));

			var handler = view.Handler;

			if (handler == null)
			{
				//handler = Registrar.Handlers.GetHandler(view.GetType());
				handler = App.Current?.Handlers?.GetHandler(view.GetType());
				
				if (handler == null)
					throw new System.Exception($"Handler not found for view {view}");

				view.Handler = handler;
			}

			handler.SetVirtualView(view);

			if (!(handler.NativeView is UIView result))
			{
				throw new InvalidOperationException($"Unable to convert {view} to {typeof(UIView)}");
			}

			return result;
		}
	}
}