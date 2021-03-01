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
		/// Create is called to start an application.
		/// </summary>
		void Run();

		/// <summary>
		/// Create is called to start an application.
		/// </summary>
		/// <param name="window">Window that will be added to the Windows property and made the MainWindow of the Application.</param>
		void Run(IWindow window);

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
	}
}