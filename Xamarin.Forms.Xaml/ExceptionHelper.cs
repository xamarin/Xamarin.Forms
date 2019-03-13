using System;

namespace Xamarin.Forms.Xaml
{
	static class ExceptionHelper
	{
		internal static void ThrowUnhandledException(HydrationContext context, Exception e)
		{
			if (context.ExceptionHandler != null)
				context.ExceptionHandler(e);
			else
				throw (e);
		}

	}
}