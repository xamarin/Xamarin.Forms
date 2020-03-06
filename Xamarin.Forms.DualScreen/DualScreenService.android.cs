﻿using System;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using Android.Views;
using Microsoft.Device.Display;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(DualScreenService.DualScreenServiceImpl))]

namespace Xamarin.Forms.DualScreen
{
	public class DualScreenService
	{
		public static void Init(Activity activity)
		{
			DependencyService.Register<DualScreenServiceImpl>();
			DualScreenServiceImpl.Init(activity);
		}

		internal class DualScreenServiceImpl : IDualScreenService, Platform.Android.DualScreen.IDualScreenService
		{
			public event EventHandler OnScreenChanged;
			ScreenHelper _helper;
			bool _isDuo = false;
			bool IsDuo => (_helper == null || _HingeService == null || _mainActivity == null || _hingeSensor == null) ? false : _isDuo;
			HingeSensor _hingeSensor;
			static Activity _mainActivity;
			static DualScreenServiceImpl _HingeService;
			bool _isLandscape;
			Size _pixelScreenSize;
			object _hingeAngleLock = new object();
			TaskCompletionSource<int> _gettingHingeAngle;

			public DualScreenServiceImpl()
			{
				_HingeService = this;
				if (_mainActivity != null)
					Init(_mainActivity);
			}

			public static void Init(Activity activity)
			{
				if (_HingeService == null)
				{
					_mainActivity = activity;
					return;
				}

				if (activity == _mainActivity && _HingeService._helper != null)
					return;

				if (_mainActivity is IDeviceInfoProvider oldDeviceInfoProvider)
					oldDeviceInfoProvider.ConfigurationChanged -= _HingeService.ConfigurationChanged;

				_mainActivity = activity;

				if (_mainActivity == null)
					return;

				bool isDuo = _HingeService._isDuo = ScreenHelper.IsDualScreenDevice(_mainActivity);
				if (!isDuo)
					return;

				var screenHelper = _HingeService._helper ?? new ScreenHelper();
				isDuo = screenHelper.Initialize(_mainActivity);
				_HingeService._isDuo = isDuo;

				if (!isDuo)
				{
					_HingeService._helper = null;					
					_HingeService._hingeSensor = null;
					return;
				}

				_HingeService._helper = screenHelper;
				_HingeService._hingeSensor = new HingeSensor(_mainActivity);
				if (_mainActivity is IDeviceInfoProvider newDeviceInfoProvider)
				{
					newDeviceInfoProvider.ConfigurationChanged += _HingeService.ConfigurationChanged;
				}
			}

			void ConfigurationChanged(object sender, EventArgs e)
			{				
				if(IsDuo)
					_helper?.Update();

				bool screenChanged = false;
				if (_isLandscape != IsLandscape)
				{
					_isLandscape = IsLandscape;
					screenChanged = true;
				}

				if (_mainActivity != null)
				{
					using (DisplayMetrics display = _mainActivity.Resources.DisplayMetrics)
					{
						var scalingFactor = display.Density;
						_pixelScreenSize = new Size(display.WidthPixels, display.HeightPixels);
						var newSize = new Size(_pixelScreenSize.Width / scalingFactor, _pixelScreenSize.Height / scalingFactor);

						if (newSize != ScaledScreenSize)
						{
							ScaledScreenSize = newSize;
							screenChanged = true;
						}
					}
				}

				if(screenChanged)
					OnScreenChanged?.Invoke(this, e);
			}

			public Size ScaledScreenSize
			{
				get;
				private set;
			}

			public bool IsSpanned
				=> IsDuo && (_helper?.IsDualMode ?? false);

			void StartListeningForHingeChanges()
			{
				if (!IsDuo)
					return;

				_hingeSensor.OnSensorChanged += OnSensorChanged;
				_hingeSensor.StartListening();
			}

			void StopListeningForHingeChanges()
			{
				if (!IsDuo)
					return;

				_hingeSensor.OnSensorChanged -= OnSensorChanged;
				_hingeSensor.StopListening();
			}

			void OnSensorChanged(object sender, HingeSensor.HingeSensorChangedEventArgs e)
			{
				SetHingeAngle(e.HingeAngle);
			}

			void SetHingeAngle(int hingeAngle)
			{
				TaskCompletionSource<int> toSet = null;
				lock (_hingeAngleLock)
				{
					StopListeningForHingeChanges();
					toSet = _gettingHingeAngle;
					_gettingHingeAngle = null;
				}

				if (toSet != null)
					toSet.SetResult(hingeAngle);
			}

			public Task<int> GetHingeAngleAsync()
			{
				if (!IsDuo)
					return Task.FromResult(0);

				Task<int> returnValue = null;
				lock (_hingeAngleLock)
				{
					if (_gettingHingeAngle == null)
					{
						_gettingHingeAngle = new TaskCompletionSource<int>();
						StartListeningForHingeChanges();
					}

					returnValue = _gettingHingeAngle.Task;
				}

				return returnValue;
			}

			public Rectangle GetHinge()
			{
				if (!IsDuo)
					return Rectangle.Zero;
								
				var hinge = _helper.GetHingeBoundsDip();

				if (hinge == null)
					return Rectangle.Zero;
				
				var hingeDp = new Rectangle((hinge.Left), (hinge.Top), (hinge.Width()), (hinge.Height()));
				
				return hingeDp;
			}

			public bool IsLandscape
			{
				get
				{
					if (!IsDuo)
					{
						if (_mainActivity == null)
							return false;

						var orientation = _mainActivity.Resources.Configuration.Orientation;
						return orientation == global::Android.Content.Res.Orientation.Landscape;
					}

					var rotation = ScreenHelper.GetRotation(_helper.Activity);
					return (rotation == SurfaceOrientation.Rotation270 || rotation == SurfaceOrientation.Rotation90);
				}
			}

			public Point? GetLocationOnScreen(VisualElement visualElement)
			{
				var view = Platform.Android.Platform.GetRenderer(visualElement);

				if (view?.View == null)
					return null;

				int[] location = new int[2];
				view.View.GetLocationOnScreen(location);
				return new Point(view.View.Context.FromPixels(location[0]), view.View.Context.FromPixels(location[1]));
			}

			public object WatchForChangesOnLayout(VisualElement visualElement, Action action)
			{
				if (action == null)
					return null;

				var view = Platform.Android.Platform.GetRenderer(visualElement);
				var androidView = view?.View;

				if (androidView == null || !androidView.IsAlive())
					return null;

				ViewTreeObserver.IOnGlobalLayoutListener listener = null;
				listener = new GenericGlobalLayoutListener(() =>
				{
					if (!androidView.IsAlive())
					{
						action = null;
						androidView = null;
						try
						{
							_mainActivity?.Window?.DecorView?.RootView?.ViewTreeObserver?.RemoveOnGlobalLayoutListener(listener);
						}
						catch
						{
							// just in case something along the call path here is disposed of
						}

						return;
					}

					action?.Invoke();
				});

				androidView.ViewTreeObserver.AddOnGlobalLayoutListener(listener);
				return listener;
			}

			public void StopWatchingForChangesOnLayout(VisualElement visualElement, object handle)
			{
				if (handle == null)
					return;

				var view = Platform.Android.Platform.GetRenderer(visualElement);
				var androidView = view?.View;

				if (androidView == null || !androidView.IsAlive())
					return;

				if (handle is ViewTreeObserver.IOnGlobalLayoutListener vto)
				{
					try
					{
						view.View.ViewTreeObserver.RemoveOnGlobalLayoutListener(vto);
					}
					catch
					{
						// just in case something along the call path here is disposed of
					}
				}
			}
		}
	}
}