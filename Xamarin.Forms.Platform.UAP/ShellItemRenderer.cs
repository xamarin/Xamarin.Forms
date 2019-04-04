namespace Xamarin.Forms.Platform.UWP
{
	public class ShellItemRenderer : IAppearanceObserver
	{
		public ShellItemRenderer(ShellRenderer renderer)
		{

		}

		#region IAppearanceObserver

		void IAppearanceObserver.OnAppearanceChanged(ShellAppearance appearance)
		{
			// if (appearance != null)
			// 	SetAppearance(appearance);
			// else
			// 	ResetAppearance();
		}

		public ShellItem ShellItem { get; internal set; }

		#endregion IAppearanceObserver

	}
}