using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace bvbbf
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			Device.Info.PropertyChanged += InfoOnPropertyChanged;
			Device.Info.BeginDeviceOrientationNotifications();

			MainPage = new NavigationPage(new bvbbf.MainPage());
		}

		void InfoOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			if(propertyChangedEventArgs.PropertyName == "DeviceOrientation")
				System.Diagnostics.Debug.WriteLine("D: " + Device.Info.DeviceOrientation);

			if (propertyChangedEventArgs.PropertyName == "ScreenOrientation")
				System.Diagnostics.Debug.WriteLine("S: " + Device.Info.ScreenOrientation);

			if (propertyChangedEventArgs.PropertyName == "PageOrientation")
				System.Diagnostics.Debug.WriteLine("P: " + Device.Info.PageOrientation.Page + ", " + Device.Info.PageOrientation.LayoutOrientation);
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
