using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.TestCasesPages
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1777, "Adding picker items when picker is in a ViewCell breaks",
		PlatformAffected.WinPhone)]
	public class Issue1777 : ContentPage
	{
		Picker _pickerNormal = null;
		Picker _pickerTable = null;

		public Issue1777()
		{
			var stackLayout = new StackLayout();
			Content = stackLayout;

			var tableView = new TableView();
			stackLayout.Children.Add(tableView);

			var tableRoot = new TableRoot();
			tableView.Root = tableRoot;

			var tableSection = new TableSection("Table");
			tableRoot.Add(tableSection);

			var viewCell = new ViewCell();
			tableSection.Add(viewCell);

			var contentView = new ContentView();
			contentView.HorizontalOptions = LayoutOptions.FillAndExpand;
			viewCell.View = contentView;

			_pickerTable = new Picker();
			_pickerTable.HorizontalOptions = LayoutOptions.FillAndExpand;
			contentView.Content = _pickerTable;

			var label = new Label();
			label.Text = "Normal";
			stackLayout.Children.Add(label);

			_pickerNormal = new Picker();
			stackLayout.Children.Add(_pickerNormal);

			var button = new Button();
			button.Clicked += button_Clicked;
			button.Text = "do magic";
			stackLayout.Children.Add(button);

			//button_Clicked(button, EventArgs.Empty);
			_pickerTable.SelectedIndex = 0;
			_pickerNormal.SelectedIndex = 0;
		}

		void button_Clicked(object sender, EventArgs e)
		{
			_pickerTable.Items.Add("test " + _pickerTable.Items.Count);
			_pickerNormal.Items.Add("test " + _pickerNormal.Items.Count);
		}
	}
}