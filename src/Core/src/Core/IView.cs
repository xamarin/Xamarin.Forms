namespace Microsoft.Maui
{
	/// <summary>
	/// Represents a .NET MAUI View. Views are user-interface objects such as labels, buttons,
	/// and sliders that are commonly known as controls or widgets in other graphical programming
	/// environments.
	/// </summary>
	public interface IView : IFrameworkElement
	{
		/// <summary>
		/// Fires before native handler has created view that gets set on handler.
		/// </summary>
		void OnCreating();

		/// <summary>
		/// Fires after native handler has set the native view.
		/// </summary>
		void OnCreated();

		/// <summary>
		/// Native view is about to attach to a native visual tree.
		/// </summary>
		void OnAttaching();

		/// <summary>
		/// Native view is attached to native visual tree (view might not be visible.
		/// </summary>
		void OnAttached();

		/// <summary>
		/// Native view is about to get detached from the native visual tree.
		/// </summary>
		void OnDetaching();

		/// <summary>
		/// Native view has been removed from the visual tree.
		/// </summary>
		void OnDetached();

		/// <summary>
		/// The Handler property on xplat view is about to get set to null.
		/// </summary>
		void OnDestroying();

		/// <summary>
		/// The Handler property on the xplat view has been set to null.
		/// </summary>
		void OnDestroyed();
	}
}