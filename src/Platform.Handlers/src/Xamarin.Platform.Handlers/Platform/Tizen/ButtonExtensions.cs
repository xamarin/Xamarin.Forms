using Xamarin.Platform.Tizen;
using EButton = ElmSharp.Button;

namespace Xamarin.Platform
{
	public static class ButtonExtensions
	{
		public static void UpdateColor(this EButton nativeButton, IButton button)
		{
			if (nativeButton is Button eButton)
			{
				eButton.TextColor = button.Color.ToNative();
			}
		}

		public static void UpdateText(this EButton nativeButton, IButton button)
		{
			nativeButton.Text = button.Text ?? "";
		}
	}
}
