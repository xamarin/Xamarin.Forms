using System;
using Xamarin.Forms;

namespace FormsGallery
{
	class TableViewFormDemoPage : ContentPage
	{
		public TableViewFormDemoPage()
		{
			Label header = new Label
			{
				Text = "TableView for a form",
				FontSize = 30,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Center
			};

			TableView tableView = new TableView
			{
				Intent = TableIntent.Form,
				Root = new TableRoot("TableView Title")
				{
					new TableSection("Table Section")
					{
						new TextCell
						{
							Text = "Text Cell",
							Detail = "With Detail Text",
						},
						new ImageCell
						{
                            // Some differences with loading images in initial release.
                            ImageSource =  ImageSource.FromUri(new Uri("http://xamarin.com/images/index/ide-xamarin-studio.png")),
							Text = "Image Cell",
							Detail = "With Detail Text",
						},
						new SwitchCell
						{
							Text = "Switch Cell"
						},
						new EntryCell
						{
							Label = "Entry Cell",
							Placeholder = "Type text here"
						},
						new ViewCell
						{
							View = new Label
							{
								Text = "A View Cell can be anything you want!"
							}
						}
					}
				}
			};

			// Build the page.
			this.Content = new StackLayout
			{
				Children =
				{
					header,
					tableView
				}
			};
		}
	}
}
