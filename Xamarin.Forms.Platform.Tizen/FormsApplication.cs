using System;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms.Internals;
using Tizen.Applications;
using ElmSharp;
using EButton = ElmSharp.Button;
using EProgressBar = ElmSharp.ProgressBar;
using EColor = ElmSharp.Color;
using ELabel = ElmSharp.Label;

namespace Xamarin.Forms.Platform.Tizen
{
	public class FormsApplication : CoreUIApplication
	{
		Platform _platform;
		Application _application;
		bool _isInitialStart;
		int _pageBusyCount;
		Native.Dialog _pageBusyDialog;
		Native.Window _window;

		protected FormsApplication()
		{
			_isInitialStart = true;
			_pageBusyCount = 0;
		}

		/// <summary>
		/// Gets the main window or <c>null</c> if it's not set.
		/// </summary>
		/// <value>The main window or <c>null</c>.</value>
		public Native.Window MainWindow
		{
			get
			{
				return _window;
			}

			private set
			{
				_window = value;
			}
		}

		protected override void OnPreCreate()
		{
			base.OnPreCreate();
			Application.ClearCurrent();
			CreateWindow();
		}

		protected override void OnTerminate()
		{
			base.OnTerminate();
			MessagingCenter.Unsubscribe<Page, AlertArguments>(this, "Xamarin.SendAlert");
			MessagingCenter.Unsubscribe<Page, bool>(this, "Xamarin.BusySet");
			MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, "Xamarin.ShowActionSheet");
			if (_platform != null)
			{
				_platform.Dispose();
			}
		}

		protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
		{
			base.OnAppControlReceived(e);

			if (!_isInitialStart && _application != null)
			{
				_application.SendResume();
			}
			_isInitialStart = false;
		}

		protected override void OnPause()
		{
			base.OnPause();
			if (_application != null)
			{
				_application.SendSleepAsync();
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (_application != null)
			{
				_application.SendResume();
			}
		}

		public void LoadApplication(Application application)
		{
			if (null == MainWindow)
			{
				throw new NullReferenceException("MainWindow is not prepared. This method should be called in OnCreated().");
			}
			if (null == application)
			{
				throw new ArgumentNullException("application");
			}
			_application = application;
			Application.Current = application;
			application.SendStart();
			application.PropertyChanged += new PropertyChangedEventHandler(this.AppOnPropertyChanged);
			SetPage(_application.MainPage);
		}

		void AppOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if ("MainPage" == args.PropertyName)
			{
				SetPage(_application.MainPage);
			}
		}

		void ShowActivityIndicatorDialog(bool enabled)
		{
			if (null == _pageBusyDialog)
			{
				_pageBusyDialog = new Native.Dialog(Forms.Context.MainWindow)
				{
					Orientation = PopupOrientation.Top,
				};

				var activity = new EProgressBar(_pageBusyDialog)
				{
					Style = "process_large",
					IsPulseMode = true,
				};
				activity.PlayPulse();
				activity.Show();

				_pageBusyDialog.Content = activity;

			}
			_pageBusyCount = Math.Max(0, enabled ? _pageBusyCount + 1 : _pageBusyCount - 1);
			if (_pageBusyCount > 0)
			{
				_pageBusyDialog.Show();
			}
			else
			{
				_pageBusyDialog.Dismiss();
				_pageBusyDialog = null;
			}
		}

		void SetPage(Page page)
		{
			if (!Forms.IsInitialized)
			{
				throw new InvalidOperationException("Call Forms.Init (UIApplication) before this");
			}
			if (_platform != null)
			{
				_platform.SetPage(page);
				return;
			}
			MessagingCenter.Subscribe<Page, bool>(this, Page.BusySetSignalName, delegate (Page sender, bool enabled)
				{
					ShowActivityIndicatorDialog(enabled);
				}, null);

			MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, delegate (Page sender, AlertArguments arguments)
				{
					Native.Dialog alert = new Native.Dialog(Forms.Context.MainWindow);
					alert.Title = arguments.Title;
					var message = arguments.Message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace(Environment.NewLine, "<br>");
					alert.Text = message;

					EButton cancel = new EButton(alert) { Text = arguments.Cancel };
					alert.NegativeButton = cancel;
					cancel.Clicked += (s, evt) =>
					{
						arguments.SetResult(false);
						alert.Dismiss();
					};

					if (arguments.Accept != null)
					{
						EButton ok = new EButton(alert) { Text = arguments.Accept };
						alert.NeutralButton = ok;
						ok.Clicked += (s, evt) =>
						{
							arguments.SetResult(true);
							alert.Dismiss();
						};
					}

					alert.BackButtonPressed += (s, evt) =>
					{
						arguments.SetResult(false);
						alert.Dismiss();
					};

					alert.Show();
				}, null);

			MessagingCenter.Subscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName, delegate (Page sender, ActionSheetArguments arguments)
			{
				Native.Dialog alert = new Native.Dialog(Forms.Context.MainWindow);

				alert.Title = arguments.Title;
				Box box = new Box(alert);

				if (null != arguments.Destruction)
				{
					Native.Button destruction = new Native.Button(alert)
					{
						Text = arguments.Destruction,
						TextColor = EColor.Red,
						AlignmentX = -1
					};
					destruction.Clicked += (s, evt) =>
					{
						arguments.SetResult(arguments.Destruction);
						alert.Dismiss();
					};
					destruction.Show();
					box.PackEnd(destruction);
				}

				foreach (string buttonName in arguments.Buttons)
				{
					Native.Button button = new Native.Button(alert)
					{
						Text = buttonName,
						AlignmentX = -1
					};
					button.Clicked += (s, evt) =>
					{
						arguments.SetResult(buttonName);
						alert.Dismiss();
					};
					button.Show();
					box.PackEnd(button);
				}

				box.Show();
				alert.Content = box;

				if (null != arguments.Cancel)
				{
					EButton cancel = new EButton(Forms.Context.MainWindow) { Text = arguments.Cancel };
					alert.NegativeButton = cancel;
					cancel.Clicked += (s, evt) =>
					{
						alert.Dismiss();
					};
				}

				alert.BackButtonPressed += (s, evt) =>
				{
					alert.Dismiss();
				};

				alert.Show();
			}, null);

			_platform = new Platform(this);
			if (_application != null)
			{
				_application.Platform = _platform;
			}
			_platform.SetPage(page);
		}

		void CreateWindow()
		{
			Debug.Assert(null == MainWindow);

			var window = new Native.Window();
			window.Closed += (s, e) =>
			{
				Exit();
			};
			window.RotationChanged += (sender, e) =>
			{
				switch (_window.CurrentOrientation)
				{
					case Native.DisplayOrientations.None:
						Device.Info.CurrentOrientation = Internals.DeviceOrientation.Other;
						break;

					case Native.DisplayOrientations.Portrait:
						Device.Info.CurrentOrientation = Internals.DeviceOrientation.PortraitUp;
						break;

					case Native.DisplayOrientations.Landscape:
						Device.Info.CurrentOrientation = Internals.DeviceOrientation.LandscapeLeft;
						break;

					case Native.DisplayOrientations.PortraitFlipped:
						Device.Info.CurrentOrientation = Internals.DeviceOrientation.PortraitDown;
						break;

					case Native.DisplayOrientations.LandscapeFlipped:
						Device.Info.CurrentOrientation = Internals.DeviceOrientation.LandscapeRight;
						break;
				}
			};

			MainWindow = window;
		}
		public void Run()
		{
			Run(System.Environment.GetCommandLineArgs());
		}

		/// <summary>
		/// Exits the application's main loop, which initiates the process of its termination
		/// </summary>
		public override void Exit()
		{
			if (_platform == null)
			{
				Log.Warn("Exit was already called or FormsApplication is not initialized yet.");
				return;
			}
			// before everything is closed, inform the MainPage that it is disappearing
			try
			{
				(_platform?.Page as IPageController)?.SendDisappearing();
				_platform = null;
			}
			catch (Exception e)
			{
				Log.Error("Exception thrown from SendDisappearing: {0}", e.Message);
			}

			base.Exit();
		}
	}
}
