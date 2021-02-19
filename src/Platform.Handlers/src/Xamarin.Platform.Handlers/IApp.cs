using System;
using System.Collections.Generic;

namespace Xamarin.Platform
{
	public interface IApp
	{
		IServiceProvider? Services { get; }

		public IHandlersContext? Context { get; }

		IWindow GetWindowFor(Dictionary<string, string> state);
	}
}