using System;
using System.Collections.Generic;
using System.Text;
using Context = global::Android.Content.Context;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class Platform
	{
		public static Context DefaultContext { get; set; }
			= new global::AndroidX.AppCompat.App.AppCompatActivity();

		public static void Init(Context context)
		{
			DefaultContext = context;
		}
	}
}
