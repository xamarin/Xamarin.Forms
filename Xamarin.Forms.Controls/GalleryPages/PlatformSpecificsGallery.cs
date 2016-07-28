using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls
{
	public class PlatformSpecificsGallery : ContentPage
	{
		Page _originalRoot;

		public PlatformSpecificsGallery()
		{
			var mdpButton = new Button { Text = "Master Detail Page" };

			mdpButton.Clicked += (sender, args) => { SetRoot(CreateMdpPage()); };

			Content = new StackLayout
			{
				Children = { mdpButton }
			};
		}

		void SetRoot(Page page)
		{
			var app = Application.Current as App;
			if (app == null)
			{
				return;
			}

			_originalRoot = app.MainPage;
			app.SetMainPage (page);
		}

		void RestoreOriginal()
		{
			if (_originalRoot == null)
			{
				return;
			}

			var app = Application.Current as App;
			app?.SetMainPage (_originalRoot);
		}

		public class NavItem
		{
			public NavItem(string text, string imageSource, ICommand command)
			{
				Text = text;
				ImageSource = imageSource;
				Command = command;
			}

			public string Text { get; set; }
			public string ImageSource { get; set; }
			public ICommand Command { get; set; }
		}

		public class NavList : ListView
		{
			public NavList(IEnumerable<NavItem> items)
			{
				var cell = new DataTemplate(typeof(ImageCell));

				cell.SetBinding(TextCell.TextProperty, "Text");
				cell.SetBinding(ImageCell.ImageSourceProperty, "ImageSource");
				cell.SetBinding(TextCell.CommandProperty, "Command");

				ItemTemplate = cell;
				ItemsSource = items;

				RowHeight = 80;
				WidthRequest = 48;
			}
		}

		static Layout CreateCollapseStyleChanger(MasterDetailPage page)
		{
			var collapseStylePicker = new Picker();
			string[] collapseStyles = Enum.GetNames(typeof(CollapseStyle));
			foreach (string collapseStyle in collapseStyles)
			{
				collapseStylePicker.Items.Add(collapseStyle);
			}

			collapseStylePicker.SelectedIndex =
				collapseStyles.IndexOf(Enum.GetName(typeof(CollapseStyle), page.On<Windows>().GetCollapseStyle()));

			collapseStylePicker.SelectedIndexChanged += (sender, args) =>
			{
				page.On<Windows>()
					.SetCollapseStyle(
						(CollapseStyle)Enum.Parse(typeof(CollapseStyle), collapseStylePicker.Items[collapseStylePicker.SelectedIndex]));
			};

			var layout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { new Label { Text = "Select Collapse Style" }, collapseStylePicker }
			};

			return layout;
		}

		static Layout CreateCollapseWidthAdjuster(MasterDetailPage page)
		{
			var adjustCollapseWidthLabel = new Label() { Text = "Adjust Collapsed Width", VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center};
			var adjustCollapseWidthEntry = new Entry { Text = page.On<Windows>().CollapsedPaneWidth().ToString() }; 
			var adjustCollapseWidthButton = new Button { Text = "Change" };
			adjustCollapseWidthButton.Clicked += (sender, args) =>
			{
				double newWidth;
				if (double.TryParse(adjustCollapseWidthEntry.Text, out newWidth))
				{
					page.On<Windows>().CollapsedPaneWidth(newWidth);
				}
			};
			
			var adjustCollapsedWidthSection = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				Children = { adjustCollapseWidthLabel, adjustCollapseWidthEntry, adjustCollapseWidthButton}
			};

			return adjustCollapsedWidthSection;
		}

		MasterDetailPage CreateMdpPage()
		{
			var page = new MasterDetailPage();

			page.On<Windows>()
				.SetCollapseStyle(CollapseStyle.Partial);
			page.MasterBehavior = MasterBehavior.Popover;

			var master = new ContentPage { Title = "Master Detail Page" };
			var masterContent = new StackLayout { Spacing = 10, Margin = new Thickness(0, 10, 5, 0)};

			// Build the navigation pane items
			var navItems = new List<NavItem>
			{
				new NavItem("Return To Gallery", "coffee.png", new Command(RestoreOriginal)),
				new NavItem("Display Alert", "coffee.png", new Command(() => DisplayAlert("Hey!", "This is an alert", "OK")))
			};

			var navList = new NavList(navItems);

			// And add them to the navigation pane's content
			masterContent.Children.Add(navList);
			master.Content = masterContent;

			var detail = new ContentPage { Title = "Detail" };
			var detailContent = new StackLayout { VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill };
			detailContent.Children.Add(new Label
			{ 
				HeightRequest = 200,
				Text = "Features",
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			});

			detailContent.Children.Add(CreateCollapseStyleChanger(page));
			detailContent.Children.Add(CreateCollapseWidthAdjuster(page));

			detail.Content = detailContent;

			page.Master = master;
			page.Detail = detail;

			return page;
		}
	}
}