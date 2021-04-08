using Tizen.Applications;
using Xamarin.Platform;

namespace Sample.Tizen
{
	class Program : CoreUIApplication
	{
		protected override void OnCreate()
		{
			base.OnCreate();
			var app = new MyApp();
			var context = new CoreUIAppContext(this);
			var view = app.CreateView().ToNative(context);
			context.SetContent(view);
		}

		static void Main(string[] args)
		{
			var app = new Program();
			app.Run(args);
		}
	}
}
