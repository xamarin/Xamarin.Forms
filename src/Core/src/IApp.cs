using System;

namespace Microsoft.Maui
{
	/// <summary>
	/// Encapsulates the app and its available services.
	/// </summary>
	public interface IApp
	{
		/// <summary>
		/// Gets a collection of application-scoped services.
		/// </summary>
		IServiceProvider? Services { get; }

		/// <summary>
		/// Called when App is first created.
		/// </summary>
		void Create();

		/// <summary>
		/// Called when App content will start interacting with the user.
		/// </summary>
		void Resume();

		/// <summary>
		/// Called when App is not visible to the user.
		/// </summary>
		void Pause();

		/// <summary>
		/// Called before the App is closed.
		/// </summary>
		void Stop();
	}
}