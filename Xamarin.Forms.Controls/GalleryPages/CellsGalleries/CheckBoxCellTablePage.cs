namespace Xamarin.Forms.Controls
{
	public class CheckBoxCellTablePage : ContentPage
	{
		public CheckBoxCellTablePage ()
		{
			Title = "CheckBoxCell Table Gallery - Legacy";

			if (Device.RuntimePlatform == Device.iOS && Device.Idiom == TargetIdiom.Tablet)
				Padding = new Thickness(0, 0, 0, 60);

			var tableSection = new TableSection ("Section One") {
				new CheckBoxCell { Text = "text 1", IsChecked = true, CheckedColor = Color.HotPink, UncheckedColor = Color.DeepPink },
				new CheckBoxCell { Text = "text 2" },
				new CheckBoxCell { Text = "text 3", IsChecked = true },
				new CheckBoxCell { Text = "text 4", IsChecked = false },
				new CheckBoxCell { Text = "text 5", IsChecked = true, CheckedColor = Color.HotPink, UncheckedColor = Color.DeepPink },
				new CheckBoxCell { Text = "text 6" },
				new CheckBoxCell { Text = "text 7", IsChecked = true },
				new CheckBoxCell { Text = "text 8", IsChecked = false },
				new CheckBoxCell { Text = "text 9", IsChecked = true, CheckedColor = Color.HotPink, UncheckedColor = Color.DeepPink },
				new CheckBoxCell { Text = "text 10" },
				new CheckBoxCell { Text = "text 11", IsChecked = true },
				new CheckBoxCell { Text = "text 12", IsChecked = false },
				new CheckBoxCell { Text = "text 13", IsChecked = true },
				new CheckBoxCell { Text = "text 14" },
				new CheckBoxCell { Text = "text 15", IsChecked = true },
				new CheckBoxCell { Text = "text 16", IsChecked = false, CheckedColor = Color.HotPink, UncheckedColor = Color.DeepPink },
			};

			var tableSectionTwo = new TableSection ("Section Two") {
				new CheckBoxCell { Text = "text 17", IsChecked = true },
				new CheckBoxCell { Text = "text 18" },
				new CheckBoxCell { Text = "text 19", IsChecked = true },
				new CheckBoxCell { Text = "text 20", IsChecked = false },
				new CheckBoxCell { Text = "text 21", IsChecked = true },
				new CheckBoxCell { Text = "text 22" },
				new CheckBoxCell { Text = "text 23", IsChecked = true },
				new CheckBoxCell { Text = "text 24", IsChecked = false },
				new CheckBoxCell { Text = "text 25", IsChecked = true },
				new CheckBoxCell { Text = "text 26" },
				new CheckBoxCell { Text = "text 27", IsChecked = true, CheckedColor = Color.HotPink, UncheckedColor = Color.DeepPink },
				new CheckBoxCell { Text = "text 28", IsChecked = false },
				new CheckBoxCell { Text = "text 29", IsChecked = true },
				new CheckBoxCell { Text = "text 30" },
				new CheckBoxCell { Text = "text 31", IsChecked = true },
				new CheckBoxCell { Text = "text 32", IsChecked = false },
			};

			var root = new TableRoot ("Text Cell table") {
				tableSection,
				tableSectionTwo
			};

			var table = new TableView {
				AutomationId = CellTypeList.CellTestContainerId,
				Root = root,
			};

			Content = table;
		}
	}
}