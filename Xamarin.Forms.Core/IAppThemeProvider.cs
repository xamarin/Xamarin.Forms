using Xamarin.Essentials;

namespace Xamarin.Forms.Core
{
	public interface IAppThemeProvider
	{
		AppTheme RequestedTheme { get; }
	}
}