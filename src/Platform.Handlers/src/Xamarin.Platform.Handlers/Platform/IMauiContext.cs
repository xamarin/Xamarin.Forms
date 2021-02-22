using System;

namespace Xamarin.Platform
{
	public interface IMauiContext
	{
		IServiceProvider Provider { get; }

		IMauiServiceProvider Handlers { get; }

#if __ANDROID__
		global::Android.Content.Context Context { get; }
#elif __IOS__

#endif
	}
}
