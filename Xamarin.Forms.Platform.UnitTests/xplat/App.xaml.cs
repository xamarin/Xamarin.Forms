using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Platform.UnitTests
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new PlatformTestsConsole();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
