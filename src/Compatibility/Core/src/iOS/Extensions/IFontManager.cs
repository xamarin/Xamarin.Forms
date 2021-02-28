using Microsoft.Maui.Controls.Internals;
using UIKit;

namespace Microsoft.Maui.Controls.Compatibility.Platform.iOS
{
	internal interface IFontManager
	{
		string DefaultFontName { get; }

		UIFont GetFont(Font self);

		UIFont GetFont(IFontElement element);
	}
}