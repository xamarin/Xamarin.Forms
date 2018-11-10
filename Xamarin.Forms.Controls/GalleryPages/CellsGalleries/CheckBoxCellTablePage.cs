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
				new CheckBoxCell { Text = "text 1", On = true },
				new CheckBoxCell { Text = "text 2" },
				new CheckBoxCell { Text = "text 3", On = true },
				new CheckBoxCell { Text = "text 4", On = false },
				new CheckBoxCell { Text = "text 5", On = true },
				new CheckBoxCell { Text = "text 6" },
				new CheckBoxCell { Text = "text 7", On = true },
				new CheckBoxCell { Text = "text 8", On = false },
				new CheckBoxCell { Text = "text 9", On = true },
				new CheckBoxCell { Text = "text 10" },
				new CheckBoxCell { Text = "text 11", On = true },
				new CheckBoxCell { Text = "text 12", On = false },
				new CheckBoxCell { Text = "text 13", On = true },
				new CheckBoxCell { Text = "text 14" },
				new CheckBoxCell { Text = "text 15", On = true },
				new CheckBoxCell { Text = "text 16", On = false },
			};

			var tableSectionTwo = new TableSection ("Section Two") {
				new CheckBoxCell { Text = "text 17", On = true },
				new CheckBoxCell { Text = "text 18" },
				new CheckBoxCell { Text = "text 19", On = true },
				new CheckBoxCell { Text = "text 20", On = false },
				new CheckBoxCell { Text = "text 21", On = true },
				new CheckBoxCell { Text = "text 22" },
				new CheckBoxCell { Text = "text 23", On = true },
				new CheckBoxCell { Text = "text 24", On = false },
				new CheckBoxCell { Text = "text 25", On = true },
				new CheckBoxCell { Text = "text 26" },
				new CheckBoxCell { Text = "text 27", On = true },
				new CheckBoxCell { Text = "text 28", On = false },
				new CheckBoxCell { Text = "text 29", On = true },
				new CheckBoxCell { Text = "text 30" },
				new CheckBoxCell { Text = "text 31", On = true },
				new CheckBoxCell { Text = "text 32", On = false },
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