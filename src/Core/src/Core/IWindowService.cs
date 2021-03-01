namespace Microsoft.Maui
{
	/// <summary>
	/// IWindowService allow to manage native windows.
	/// </summary>
	public interface IWindowService
	{
		/// <summary>
		/// Closes the app window.
		/// </summary>
		/// <param name="window">The Window which will be opened.</param>
		void Create(IWindow window);

		/// <summary>
		/// Create a new native Window instance.
		/// </summary>
		/// <param name="window">The Window which will be closed.</param>
		void Close(IWindow window);
	}
}