using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class BoxViewHandlerTests
	{
		NativeBoxView GetNativeBoxView(BoxViewHandler boxViewHandler) =>
			(NativeBoxView)boxViewHandler.View;

		Color GetNativeColor(BoxViewHandler boxViewHandler) =>
			GetNativeBoxView(boxViewHandler).Color;

		CornerRadius GetNativeCornerRadius(BoxViewHandler boxViewHandler) =>
			GetNativeBoxView(boxViewHandler).CornerRadius;
	}
}