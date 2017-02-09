using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public class FormsApplicationPage : System.Windows.Window
	{
		Application _application;
		Platform _platform;

		protected FormsApplicationPage()
		{
			System.Windows.Application.Current.Startup += OnLaunching;
			//System.Windows.Application.Current.Activated += OnActivated;
			//System.Windows.Application.Current.Deactivated += OnDeactivated;
            System.Windows.Application.Current.Exit += OnClosing;
            
            

            MessagingCenter.Send(this, Forms.WPFDeviceInfo.BWPorientationChangedName, this.ToDeviceOrientation());
			SizeChanged += OnOrientationChanged;
			//DeserializePropertyStore ();
		}
        

        protected void LoadApplication(Application application)
		{
			Application.Current = application;
			application.PropertyChanged += ApplicationOnPropertyChanged;
			_application = application;

			// Hack around the fact that OnLaunching will haev already happened by this point, sad but needed.
			application.SendStart();

			SetMainPage();
		}

		void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "MainPage")
				SetMainPage();
		}

		void OnActivated(object sender, System.EventArgs e)
		{
			// TODO : figure out consistency of get this to fire
			// Check whether tombstoned (terminated, but OS retains information about navigation state and state dictionarys) or dormant
			_application.SendResume();
		}

		// when app gets tombstoned, user press back past first page
		async void OnClosing(object sender, ExitEventArgs e)
		{
			// populate isolated storage.
			//SerializePropertyStore ();
			await _application.SendSleepAsync();
		}

		async void OnDeactivated(object sender, System.EventArgs e)
		{
			// populate state dictionaries, properties
			//SerializePropertyStore ();
			await _application.SendSleepAsync();
		}

		void OnLaunching(object sender, StartupEventArgs e)
		{
			// TODO : not currently firing, is fired before MainPage ctor is called
			_application.SendStart();
		}

		void OnOrientationChanged(object sender, SizeChangedEventArgs e)
		{
			MessagingCenter.Send(this, Forms.WPFDeviceInfo.BWPorientationChangedName, this.ToDeviceOrientation());
		}

		void SetMainPage()
		{
			if (_platform == null)
				_platform = new Platform(this);

			_platform.SetPage(_application.MainPage);

			if (!ReferenceEquals(Content, _platform))
				Content = _platform.GetCanvas();
		}
	}
}