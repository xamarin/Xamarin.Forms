using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using System;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Widget;
using MauiApplication = Microsoft.Maui.Application;

namespace Microsoft.Maui
{
	public class MauiAppCompatActivity : AppCompatActivity
	{
		MauiApplication? _app;
		IWindow? _window;

		AndroidApplicationLifecycleState _currentState;
		AndroidApplicationLifecycleState _previousState;

		public MauiAppCompatActivity()
		{
			_previousState = AndroidApplicationLifecycleState.Uninitialized;
			_currentState = AndroidApplicationLifecycleState.Uninitialized;
		}

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (MauiApplication.Current == null)
				throw new InvalidOperationException($"App is not {nameof(Application)}");

			_app = MauiApplication.Current;

			if (_app == null || _app.Services == null)
				throw new InvalidOperationException("App was not intialized");

			_app.OnCreated();

			_previousState = _currentState;
			_currentState = AndroidApplicationLifecycleState.OnCreate;

			_window = _app.MainWindow;

			if (_window != null)
			{
				_window.Show();

				_window.MauiContext = new HandlersContext(_app.Services, this);

				_app.MainWindow = _window;

				// Hack for now we set this on the App Static but this should be on IFrameworkElement
				MauiApplication.Current?.SetHandlerContext(_window.MauiContext);

				var content = _window.Content?.View;

				CoordinatorLayout parent = new CoordinatorLayout(this);
				NestedScrollView main = new NestedScrollView(this);

				SetContentView(parent, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
				parent.AddView(main, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

				main.AddView(content?.ToNative(_window.MauiContext), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
			}
		}

		protected override void OnStart()
		{
			base.OnStart();

			_previousState = _currentState;
			_currentState = AndroidApplicationLifecycleState.OnStart;
		}

		protected override void OnPause()
		{
			base.OnPause();

			_previousState = _currentState;
			_currentState = AndroidApplicationLifecycleState.OnPause;

			UpdateApplicationLifecycleState();
		}

		protected override void OnResume()
		{
			base.OnResume();

			_previousState = _currentState;
			_currentState = AndroidApplicationLifecycleState.OnResume;

			UpdateApplicationLifecycleState();
		}

		protected override void OnRestart()
		{
			_previousState = _currentState;
			_currentState = AndroidApplicationLifecycleState.OnRestart;

			UpdateApplicationLifecycleState();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_previousState = _currentState;
			_currentState = AndroidApplicationLifecycleState.OnDestroy;

			UpdateApplicationLifecycleState();
		}

		void UpdateApplicationLifecycleState()
		{
			if (_previousState == AndroidApplicationLifecycleState.OnCreate && _currentState == AndroidApplicationLifecycleState.OnStart)
				_app?.OnCreated();
			else if (_previousState == AndroidApplicationLifecycleState.OnRestart && _currentState == AndroidApplicationLifecycleState.OnStart)
				_app?.OnResumed();
			else if (_previousState == AndroidApplicationLifecycleState.OnPause && _currentState == AndroidApplicationLifecycleState.OnStop)
				_app?.OnPaused();
			else if (_currentState == AndroidApplicationLifecycleState.OnDestroy)
				_app?.OnStopped();
		}
	}
}