using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class PickerHandlerTests
	{
		NativePicker GetNativePicker(PickerHandler pickerHandler) =>
			(NativePicker)pickerHandler.View;

		string GetNativeTitle(PickerHandler pickerHandler) =>
			GetNativePicker(pickerHandler).Text;

		Color GetNativeTitleColor(PickerHandler pickerHandler) =>
			GetNativePicker(pickerHandler).TextColor.ToColor();

		string GetNativeText(PickerHandler pickerHandler) =>
			 GetNativePicker(pickerHandler).Text;

		async Task ValidateNativeItemsSource(IPicker picker, int itemsCount)
		{
			var expected = await GetValueAsync(picker, handler =>
			{
				var pickerView = GetNativePicker(handler).UIPickerView;
				var model = (PickerSource)pickerView.Model;
				return model.GetRowsInComponent(pickerView, 0);
			});
			Assert.Equal(expected, itemsCount);
		}

		async Task ValidateNativeSelectedIndex(IPicker slider, int selectedIndex)
		{
			var expected = await GetValueAsync(slider, handler =>
			{
				var pickerView = GetNativePicker(handler).UIPickerView;
				var model = (PickerSource)pickerView.Model;
				return model.SelectedIndex;
			});
			Assert.Equal(expected, selectedIndex);
		}
	}
}