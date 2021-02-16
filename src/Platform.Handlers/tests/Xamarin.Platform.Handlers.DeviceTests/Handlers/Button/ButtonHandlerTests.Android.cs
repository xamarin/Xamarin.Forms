using AndroidX.AppCompat.Widget;
using Xamarin.Forms;
using AColor = global::Android.Graphics.Color;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ButtonHandlerTests
	{
		AppCompatButton GetNativeButton(ButtonHandler buttonHandler) =>
			(AppCompatButton)buttonHandler.View;

		string GetNativeText(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).Text;

		Color GetNativeTextColor(ButtonHandler buttonHandler)
		{
			uint currentTextColorInt = (uint)GetNativeButton(buttonHandler).CurrentTextColor;
			AColor currentTextColor = new AColor(currentTextColorInt);

			return currentTextColor.ToColor();
		}
	}
}