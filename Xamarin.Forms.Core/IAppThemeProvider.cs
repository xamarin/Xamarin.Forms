using System.ComponentModel;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IAppThemeProvider
	{
		AppTheme RequestedTheme { get; }
	}
}