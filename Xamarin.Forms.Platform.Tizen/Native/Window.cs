using System;
using ElmSharp;
using EWindow = ElmSharp.Window;
using ELayout = ElmSharp.Layout;

namespace Xamarin.Forms.Platform.Tizen.Native
{
	public class Window : EWindow
	{
		ELayout _layout;
		Conformant _conformant;

		/// <summary>
		/// Initializes a new instance of the Window class.
		/// </summary>
		public Window() : base("FormsWindow")
		{
			Initialize();
		}

		/// <summary>
		/// Notifies that the window has been closed.
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		/// Notifies that the back button has been pressed.
		/// </summary>
		public event EventHandler BackButtonPressed;

		/// <summary>
		/// Gets the current orientation.
		/// </summary>
		public DisplayOrientations CurrentOrientation
		{
			get
			{
				if (IsRotationSupported)
				{
					return GetDisplayOrientation();
				}
				else
				{
					return DisplayOrientations.None;
				}
			}
		}

		/// <summary>
		/// Gets or sets the orientation of a rectangular screen.
		/// </summary>
		public DisplayOrientations AvailableOrientations
		{
			get
			{
				if (IsRotationSupported)
				{
					return (DisplayOrientations)AvailableRotations;
				}
				else
				{
					return DisplayOrientations.None;
				}
			}
			set
			{
				if (IsRotationSupported)
				{
					AvailableRotations = (DisplayRotation)value;
				}
			}
		}

		public ELayout BaseLayout
		{
			get
			{
				return _layout;
			}

			private set
			{
				_layout = value;
			}
		}

		/// <summary>
		/// Sets the main page of Window.
		/// </summary>
		/// <param name="content">ElmSharp.EvasObject type page to be set.</param>
		public void SetMainPage(EvasObject content)
		{
			_layout.SetContent(content);
		}

		void Initialize()
		{
			// events
			Deleted += (sender, e) =>
			{
				Closed?.Invoke(this, EventArgs.Empty);
			};
			CloseRequested += (sender, e) =>
			{
				Unrealize();
			};

			KeyGrab(EvasKeyEventArgs.PlatformBackButtonName, false);
			KeyUp += (s, e) =>
			{
				if (e.KeyName == EvasKeyEventArgs.PlatformBackButtonName)
				{
					BackButtonPressed?.Invoke(this, EventArgs.Empty);
				}
			};

			Active();
			AutoDeletion = false;
			Show();

			_conformant = new Conformant(this);
			_conformant.Show();

			// Create the base (default) layout for the application
			_layout = new ELayout(_conformant);
			_layout.SetTheme("layout", "application", "default");
			_layout.Show();

			_conformant.SetContent(_layout);
			BaseLayout = _layout;
			AvailableOrientations = DisplayOrientations.Portrait | DisplayOrientations.Landscape | DisplayOrientations.PortraitFlipped | DisplayOrientations.LandscapeFlipped;
		}
		DisplayOrientations GetDisplayOrientation()
		{
			switch (Rotation)
			{
				case 0:
				return Native.DisplayOrientations.Portrait;

				case 90:
				return Native.DisplayOrientations.Landscape;

				case 180:
				return Native.DisplayOrientations.PortraitFlipped;

				case 270:
				return Native.DisplayOrientations.LandscapeFlipped;

				default:
				return Native.DisplayOrientations.None;
			}
		}
	}
}
