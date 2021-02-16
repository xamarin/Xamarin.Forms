using AndroidX.AppCompat.Widget;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class ButtonHandlerTests
	{
		AppCompatButton GetNativeButton(ButtonHandler buttonHandler) =>
			(AppCompatButton)buttonHandler.View;

		string GetNativeText(ButtonHandler buttonHandler) =>
			GetNativeButton(buttonHandler).Text;

		Color GetNativeTextColor(ButtonHandler buttonHandler) =>
			((uint)GetNativeButton(buttonHandler).CurrentTextColor).ToColor();
	}
}