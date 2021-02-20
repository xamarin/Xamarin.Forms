using System;
using System.Collections.Generic;

namespace Xamarin.Platform
{
	public abstract class MauiApp : App
	{
		public abstract IWindow GetWindowFor(IActivationState state);

		public MauiApp()
		{
			//Current = this;
		}

		//public static MauiApp? Current { get; private set; }
	}
}
