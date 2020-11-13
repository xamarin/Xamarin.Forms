using System;
using ElmSharp;
using Xamarin.Platform.Handlers;

namespace Xamarin.Platform
{
	public static class HandlerExtensions
	{
		public static EvasObject ToNative(this IView view, CoreUIAppContext context, bool isRoot = true)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));
			_ = context ?? throw new ArgumentNullException(nameof(context));

			var handler = view.Handler;

			if (handler == null)
			{
				handler = Registrar.Handlers.GetHandler(view.GetType());

				if (handler is ITizenViewHandler thandler)
					thandler.SetContext(context);

				view.Handler = handler;
			}

			handler.SetVirtualView(view);

			if (!(handler.NativeView is EvasObject result))
			{
				throw new InvalidOperationException($"Unable to convert {view} to {typeof(EvasObject)}");
			}

			// Root content view should register to LayoutUpdated() callback.
			if (isRoot && handler is LayoutHandler layoutHandler)
			{
				layoutHandler.RegisterOnLayoutUpdated();
			}

			return result;
		}
	}
}