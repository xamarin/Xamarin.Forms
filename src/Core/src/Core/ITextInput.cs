namespace Microsoft.Maui
{
	/// <summary>
	/// Represents a View which can take keyboard input.
	/// </summary>
	public interface ITextInput : IText
	{
		/// <summary>
		/// Gets the Keyboard for the Input View. 
		/// </summary>
		Keyboard Keyboard { get; }

		/// <summary>
		/// Gets a value that controls whether spell checking is enabled.
		/// </summary>
		bool IsSpellCheckEnabled { get; }

		/// <summary>
		/// Gets the maximum allowed length of input.
		/// </summary>
		int MaxLength { get; }

		/// <summary>
		/// Gets the text of the placeholder.
		/// </summary>
		string Placeholder { get; }

		/// <summary>
		/// Gets the color of the placeholder text.
		/// </summary>
		Color PlaceholderColor { get; }

		/// <summary>
		/// Gets a value that indicates whether user should be prevented from modifying the text.
		/// </summary>
		bool IsReadOnly { get; }
	}
}