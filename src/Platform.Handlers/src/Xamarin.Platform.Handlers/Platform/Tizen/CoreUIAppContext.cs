using System;
using System.Reflection;
using Tizen.Common;
using Tizen.Applications;
using ElmSharp;
using ElmSharp.Wearable;
using Xamarin.Platform.Tizen;
using ELayout = ElmSharp.Layout;

namespace Xamarin.Platform
{
	public class CoreUIAppContext
	{
		DisplayResolutionUnit _displayResolutionUnit = DisplayResolutionUnit.DP;
		double _viewPortWidth = -1;

		public CoreUIApplication CurrentApplication { get; private set; }

		public string ResourceDir => CurrentApplication.DirectoryInfo.Resource;

		public EvasObject NativeParent => BaseLayout;

		public Window MainWindow { get; set; }

		public ELayout BaseLayout { get; set; }

		public CircleSurface? BaseCircleSurface { get; set; }

		public DeviceType DeviceType => DeviceInfo.GetDeviceType();

		public DisplayResolutionUnit DisplayResolutionUnit
		{
			get => _displayResolutionUnit;
			set
			{
				_displayResolutionUnit = value;
				DeviceInfo.DisplayResolutionUnit = _displayResolutionUnit;
			}
		}

		public double ViewportWidth
		{
			get => _viewPortWidth;
			set
			{
				_viewPortWidth = value;
				ViewportWidth = _viewPortWidth;
			}
		}

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		public CoreUIAppContext(CoreUIApplication application) : this(application, CreateDefaultWindow())
		{
		}

		public CoreUIAppContext(CoreUIApplication application, Window window)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
		{
			_ = application ?? throw new ArgumentNullException(nameof(application));
			_ = window ?? throw new ArgumentNullException(nameof(window));

			if (DisplayResolutionUnit == DisplayResolutionUnit.VP && ViewportWidth < 0)
				throw new InvalidOperationException($"ViewportWidth should be set in case of DisplayResolutionUnit == VP");

			Elementary.Initialize();
			Elementary.ThemeOverlay();
			CurrentApplication = application;
			MainWindow = window;
			InitializeMainWindow();

			if (DotnetUtil.TizenAPIVersion < 5)
			{
				// We should set the env variable to support IsolatedStorageFile on tizen 4.0 or lower version.
				Environment.SetEnvironmentVariable("XDG_DATA_HOME", CurrentApplication.DirectoryInfo.Data);
			}
		}

		public void SetContent(EvasObject content)
		{
			content.SetAlignment(-1, -1);
			content.SetWeight(1, 1);
			content.Show();
			BaseLayout.SetContent(content);
		}

		static Window CreateDefaultWindow()
		{
			return GetPreloadedWindow() ?? new Window("XamarinWindow");
		}

		static Window? GetPreloadedWindow()
		{
			var type = typeof(Window);
			// Use reflection to avoid breaking compatibility. ElmSharp.Window.CreateWindow() is has been added since API6.
			var methodInfo = type.GetMethod("CreateWindow", BindingFlags.NonPublic | BindingFlags.Static);

			return (Window?)methodInfo?.Invoke(null, new object[] { "FormsWindow" });
		}

		void InitializeMainWindow()
		{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Possible null reference assignment.
			BaseLayout = (ELayout)MainWindow.GetType().GetProperty("BaseLayout")?.GetValue(MainWindow);
			BaseCircleSurface = (CircleSurface)MainWindow.GetType().GetProperty("BaseCircleSurface")?.GetValue(MainWindow);
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

			if (BaseLayout == null)
			{
				var conformant = new Conformant(MainWindow);
				conformant.Show();

				var layout = new ApplicationLayout(conformant);
				layout.Show();

				BaseLayout = layout;

				if (DeviceType == DeviceType.Watch)
				{
					BaseCircleSurface = new CircleSurface(conformant);
				}
				conformant.SetContent(BaseLayout);

				if (DeviceType == DeviceType.Watch)
				{
					BaseCircleSurface = new CircleSurface(conformant);
				}
			}

			MainWindow.Active();
			MainWindow.Show();
			MainWindow.AvailableRotations = DisplayRotation.Degree_0 | DisplayRotation.Degree_90 | DisplayRotation.Degree_180 | DisplayRotation.Degree_270;
			
			MainWindow.Deleted += (s, e) => CurrentApplication.Exit();

			MainWindow.RotationChanged += (sender, e) =>
			{
				// TODO : should update later
			};

			MainWindow.BackButtonPressed += (sender, e) => CurrentApplication.Exit();
		}
	}
}
