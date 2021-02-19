using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace Xamarin.Platform
{
	public class MauiUIApplicationDelegate<TApplication> : UIApplicationDelegate, IUIApplicationDelegate where TApplication : App
	{
	}
}
