using System;

namespace Microsoft.Maui
{
	/// <summary>
	/// Encapsulates the window and its available services.
	/// </summary>
	public interface IWindow
	{
		/// <summary>
		/// Gets the .NET MAUI Context.
		/// </summary>
		public IMauiContext? MauiContext { get; set; }

		/// <summary>
		/// Gets the window's logical child page.
		/// </summary>
		public IPage? Content { get; set; }

		/// <summary>
		/// Gets a value that indicates whether the window is active.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Opens a window and returns without waiting for the newly opened window to close.
		/// </summary>
		void Show();

		/// <summary>
		/// Attempts to bring the window to the foreground and activates it.
		/// </summary>
		void Activate();

		/// <summary>
		/// Makes a window invisible.
		/// </summary>
		void Hide();

		/// <summary>
		/// Manually closes a Window.
		/// </summary>
		void Close();

		/// <summary>
		/// Called when the Window is created.
		/// </summary>
		void OnCreated();

		/// <summary>
		/// Called when the application is not visible to the user.
		/// </summary>
		void OnResumed();

		/// <summary>
		/// Called before the application is closed.
		/// </summary>
		void OnPaused();

		/// <summary>
		/// Called when the Window is closed.
		/// </summary>
		void OnStopped();

		/// <summary>
		/// This event is raised when the window is closed.
		/// </summary>
		event EventHandler? Closed;

		/// <summary>
		/// This event is raised when the window is created.
		/// </summary>
		event EventHandler? Created;

		/// <summary>
		/// This event is raised when the window is resumed.
		/// </summary>
		event EventHandler? Resumed;

		/// <summary>
		/// This event is raised when the window is paused.
		/// </summary>
		event EventHandler? Paused;

		/// <summary>
		/// This event is raised when the window is closed.
		/// </summary>
		event EventHandler? Stopped;
	}
}