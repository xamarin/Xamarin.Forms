using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml
{
	static class ExceptionHelper
	{
		internal static void ThrowUnhandledException(HydrationContext context, Exception e)
		{
			ThrowUnhandledException(context?.ExceptionHandler, e);
		}

		internal static void ThrowUnhandledException(Exception e)
		{
			ThrowUnhandledException(ResourceLoader.ExceptionHandler, e);
		}

		internal static void ThrowUnhandledException(Action<Exception> exceptionHandler, Exception e)
		{
			if (exceptionHandler != null)
				exceptionHandler(e);
			else
				throw (e);
		}
	}
}