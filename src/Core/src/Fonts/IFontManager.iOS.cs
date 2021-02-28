using UIKit;

namespace Microsoft.Maui
{
	public interface IFontManager
	{
		string DefaultFontName { get; }

		UIFont GetFont(Font font);
	}
}