namespace Xamarin.Platform
{
	public static class PickerExtensions
	{ 
		public static void UpdateTitle(this NativePicker nativePicker, IPicker picker) =>
			UpdatePicker(nativePicker, picker);

		public static void UpdateTitleColor(this NativePicker nativePicker, IPicker picker)
		{
			var hintColorSwitcher = new TextColorSwitcher(nativePicker.HintTextColors);
			nativePicker.UpdateTitleColor(picker, hintColorSwitcher);
		}

		public static void UpdateTitleColor(this NativePicker nativePicker, IPicker picker, TextColorSwitcher hintColorSwitcher)
		{
			hintColorSwitcher.UpdateTextColor(nativePicker, picker.TitleColor);
		}

		public static void UpdateTextColor(this NativePicker nativePicker, IPicker picker)
		{
			var textColorSwitcher = new TextColorSwitcher(nativePicker.TextColors);
			nativePicker.UpdateTitleColor(picker, textColorSwitcher);
		}

		public static void UpdateTextColor(this NativePicker nativePicker, IPicker picker, TextColorSwitcher textColorSwitcher)
		{
			textColorSwitcher.UpdateTextColor(nativePicker, picker.TextColor);
		}

		public static void UpdateSelectedIndex(this NativePicker nativePicker, IPicker picker) =>
			UpdatePicker(nativePicker, picker);

		internal static void UpdatePicker(this NativePicker nativePicker, IPicker picker)
		{
			nativePicker.Hint = picker.Title;
			nativePicker.UpdateTitleColor(picker);

			if (picker.SelectedIndex == -1 || picker.Items == null || picker.SelectedIndex >= picker.Items.Count)
				nativePicker.Text = null;
			else
				nativePicker.Text = picker.Items[picker.SelectedIndex];

			nativePicker.SetSelectedItem(picker);
		}

		internal static void SetSelectedItem(this NativePicker nativePicker, IPicker picker)
		{
			if (picker == null || nativePicker == null)
				return;

			int index = picker.SelectedIndex;

			if (index == -1)
			{
				picker.SelectedItem = null;
				return;
			}

			if (picker.ItemsSource != null)
			{
				picker.SelectedItem = picker.ItemsSource[index];
				return;
			}

			if (picker.Items != null)
				picker.SelectedItem = picker.Items[index];
		}
	}
}