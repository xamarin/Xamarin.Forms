using System;

namespace Microsoft.Maui
{
	/// <summary>
	/// Encapsulates the Application and its available services.
	/// </summary>
	public interface IApplication
	{
		/// <summary>
		/// Gets a collection of application-scoped services.
		/// </summary>
		IServiceProvider? Services { get; }

		/// <summary>
		/// Create the application main window.
		/// </summary>
		/// <param name="state">The activation state of the application.</param>
		/// <returns>Created window.</returns>
		IWindow CreateWindow(IActivationState state);

		/// <summary>
		/// Called when the application is first created.
		/// </summary>
		void OnCreated();

		/// <summary>
		/// Called when the application content will start interacting with the user.
		/// </summary>
		void OnResumed();

		/// <summary>
		/// Called when the application is not visible to the user.
		/// </summary>
		void OnPaused();

		/// <summary>
		/// Called before the application is closed.
		/// </summary>
		void OnStopped();

		/// <summary>
		/// This event is raised when the window is closed.
		/// </summary>
		event EventHandler? Created;

		/// <summary>
		/// This event is raised when the window is resumed.
		/// </summary>
		event EventHandler? Resumed;

		/// <summary>
		/// This event is raised when the window is pasued.
		/// </summary>
		event EventHandler? Paused;

		/// <summary>
		/// This event is raised when the window is closed.
		/// </summary>
		event EventHandler? Stopped;
	}
}