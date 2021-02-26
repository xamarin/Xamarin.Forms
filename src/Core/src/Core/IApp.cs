using System;

namespace Microsoft.Maui
{
	/// <summary>
	/// Encapsulates the app and its available services.
	/// </summary>
	public interface IApp
	{
		/// <summary>
		/// Gets or sets the main window of the application.
		/// </summary>
		IWindow? MainWindow { get; set; }

		/// <summary>
		/// Gets the instantiated windows in an application.
		/// </summary>
		WindowCollection Windows { get; }

		/// <summary>
		/// Gets a collection of application-scoped services.
		/// </summary>
		IServiceProvider? Services { get; }

		/// <summary>
		/// Called when the application is first created.
		/// </summary>
		void Create();

		/// <summary>
		/// Called when the application content will start interacting with the user.
		/// </summary>
		void Resume();

		/// <summary>
		/// Called when the application is not visible to the user.
		/// </summary>
		void Pause();

		/// <summary>
		/// Called before the application is closed.
		/// </summary>
		void Stop();
	}
}