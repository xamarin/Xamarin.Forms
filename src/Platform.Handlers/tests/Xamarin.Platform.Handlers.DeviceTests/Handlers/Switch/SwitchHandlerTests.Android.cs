using Xamarin.Forms;
using ASwitch = AndroidX.AppCompat.Widget.SwitchCompat;
using System.Threading.Tasks;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class SwitchHandlerTests
	{
		ASwitch GetNativeSwitch(SwitchHandler switchHandler) =>
			(ASwitch)switchHandler.View;

		bool GetNativeIsChecked(SwitchHandler switchHandler) =>
			GetNativeSwitch(switchHandler).Checked;

		Task ValidateOnColor(ISwitch switchStub, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeSwitch(CreateHandler(switchStub)).AssertContainsColor(color);
			});
		}

		Task ValidateThumbColor(ISwitch switchStub, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeSwitch(CreateHandler(switchStub)).AssertContainsColor(color);
			});
		}
	}
}