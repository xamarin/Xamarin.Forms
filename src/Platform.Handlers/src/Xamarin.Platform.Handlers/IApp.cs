using System;

namespace Xamarin.Platform
{
	public interface IApp
	{
		IServiceProvider? Services { get; }

		IHandlerServiceProvider? Handlers { get; }
	}
}