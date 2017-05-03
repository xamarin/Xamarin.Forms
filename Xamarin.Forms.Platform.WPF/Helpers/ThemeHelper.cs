namespace Xamarin.Forms.Platform.WPF.Helpers
{
	class ThemeProvider : IThemeProvider
	{
		private static ThemeProvider s_themeProvider;

		public static IThemeProvider Instance => s_themeProvider ?? (s_themeProvider = new ThemeProvider());

	    public System.Windows.Media.Color AccentColor { get; set; }
	}

	
}
