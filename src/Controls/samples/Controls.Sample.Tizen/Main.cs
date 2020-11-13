using Microsoft.Maui;
using Maui.Controls.Sample;

#if !NET6_0
using Microsoft.Maui.Controls;
#endif

namespace Sample.Tizen
{
	class Program : MauiApplication<MyApp>
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			//Microsoft.Maui.Controls.Essentials.Platform.Init(this);
		}

		static void Main(string[] args)
		{
			var app = new Program();
			app.Run(args);
		}
	}
}
