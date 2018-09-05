﻿using System;
using System.ComponentModel;
using Windows.ApplicationModel;

namespace Xamarin.Forms.Platform.UWP
{
	public abstract class WindowsBasePage : Windows.UI.Xaml.Controls.Page
	{
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

			Application.SetCurrentApplication(application);
			Platform = CreatePlatform();
			if (Application.Current.MainPage != null)
				Platform.SetPage(Application.Current.MainPage);
			application.PropertyChanged += OnApplicationPropertyChanged;

			Application.Current.SendStart();
		}

		void OnApplicationPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "MainPage")
				Platform.SetPage(Application.Current.MainPage);
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