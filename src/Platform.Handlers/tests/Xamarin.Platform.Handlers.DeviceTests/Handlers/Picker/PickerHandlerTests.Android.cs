using Xamarin.Forms;
using AColor = Android.Graphics.Color;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class PickerHandlerTests
	{
		NativePicker GetNativePicker(PickerHandler pickerHandler) =>
			(NativePicker)pickerHandler.View;

		string GetNativeTitle(PickerHandler pickerHandler) =>
			GetNativePicker(pickerHandler).Hint;

		Color GetNativeTitleColor(PickerHandler pickerHandler)
		{
			var currentTextColorInt = GetNativePicker(pickerHandler).CurrentTextColor;
			var currentTextColor = new AColor(currentTextColorInt);
			return currentTextColor.ToColor();
		}
	}
}