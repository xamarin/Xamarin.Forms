﻿using System;
using System.ComponentModel;
using Windows.ApplicationModel;

namespace Xamarin.Forms.Platform.UWP
{
	public abstract class WindowsBasePage : Windows.UI.Xaml.Controls.Page
	{

		Application _application;

		public WindowsBasePage()
		{
			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				Windows.UI.Xaml.Application.Current.Suspending += OnApplicationSuspending;
				Windows.UI.Xaml.Application.Current.Resuming += OnApplicationResuming;
			}
		}

		internal Platform Platform { get; private set; }

		protected abstract Platform CreatePlatform();

		protected void LoadApplication(Application application)
		{
			if (application == null)
				throw new ArgumentNullException("application");

			_application = application;
			Application.SetCurrentApplication(application);
			if (_application.MainPage != null)
				RegisterWindow(_application.MainPage);
			application.PropertyChanged += OnApplicationPropertyChanged;

			_application.SendStart();
		}

		protected void RegisterWindow(Page page)
		{
			if (page == null)
				throw new ArgumentNullException("page");

			Platform = CreatePlatform();
			Platform.SetPage(page);
		}

		void OnApplicationPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "MainPage")
			{
				if (Platform == null)
					RegisterWindow(_application.MainPage);
				Platform.SetPage(_application.MainPage);
			}
		}

		void OnApplicationResuming(object sender, object e)
		{
			Application.Current?.SendResume();
		}

		async void OnApplicationSuspending(object sender, SuspendingEventArgs e)
		{
			var sendSleepTask = Application.Current?.SendSleepAsync();
			if (sendSleepTask == null)
				return;

			SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
			try
			{
				await sendSleepTask;
			}
			finally
			{
				deferral.Complete();
			}
		}
	}
}