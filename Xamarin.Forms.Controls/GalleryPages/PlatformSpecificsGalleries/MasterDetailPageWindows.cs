using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.PlatformSpecificsGalleries
{
	public class MasterDetailPageWindows : MasterDetailPage
	{
		const string CommandBarActionTitle = "Hey!";
		const string CommandBarActionMessage = "Command Bar Item Clicked";
		const string CommandBarActionDismiss = "OK";

		public MasterDetailPageWindows(ICommand restore)
		{
			On<Windows>()
				.SetCollapseStyle(CollapseStyle.Partial);
			MasterBehavior = MasterBehavior.Popover;

			var master = new ContentPage { Title = "Master Detail Page" };
			var masterContent = new StackLayout { Spacing = 10, Margin = new Thickness(0, 10, 5, 0) };

			// Build the navigation pane items
			var navItems = new List<NavItem>
			{
				new NavItem("Display Alert", "\uE171", new Command(() => DisplayAlert("Alert", "This is an alert", "OK"))),
				new NavItem("Return To Gallery", "\uE106", restore),
				new NavItem("Save", "\uE105", new Command(() => DisplayAlert("Save", "Fake save dialog", "OK"))),
				new NavItem("Audio", "\uE189", new Command(() => DisplayAlert("Audio", "Never gonna give you up...", "OK")))
			};

			var navList = new NavList(navItems);

			// And add them to the navigation pane's content
			masterContent.Children.Add(navList);
			master.Content = masterContent;

			var detail = new ContentPage { Title = "This is the detail page's Title" };
			var detailContent = new StackLayout { VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill };
			detailContent.Children.Add(new Label
			{
				Text = "Platform Features",
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			});

			detailContent.Children.Add(CreateCollapseStyleChanger(this));
			detailContent.Children.Add(CreateToolbarPlacementChanger(this));
			detailContent.Children.Add(CreateCollapseWidthAdjuster(this));
			detailContent.Children.Add(CreateAddRemoveToolBarItemButtons(this));

			detail.Content = detailContent;

			Master = master;

			AddToolBarItems(this);

			Detail = detail;
		}

		void AddToolBarItems(Page page)
		{
			Action action = () => page.DisplayAlert(CommandBarActionTitle, CommandBarActionMessage, CommandBarActionDismiss);

			var tb1 = new ToolbarItem("Primary 1", "coffee.png", action, ToolbarItemOrder.Primary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_primary1"
			};

			var tb2 = new ToolbarItem("Primary 2", "coffee.png", action, ToolbarItemOrder.Primary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_primary2"
			};

			var tb3 = new ToolbarItem("Seconday 1", "coffee.png", action, ToolbarItemOrder.Secondary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_secondary3"
			};

			var tb4 = new ToolbarItem("Secondary 2", "coffee.png", action, ToolbarItemOrder.Secondary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_secondary4"
			};

			page.ToolbarItems.Add(tb1);
			page.ToolbarItems.Add(tb2);
			page.ToolbarItems.Add(tb3);
			page.ToolbarItems.Add(tb4);
		}

		static Layout CreateAddRemoveToolBarItemButtons(Page page)
		{
			var layout = new StackLayout { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.Center };
			layout.Children.Add(new Label { Text = "Toolbar Items:" });

			var buttonLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center
			};

			layout.Children.Add(buttonLayout);

			var addPrimary = new Button { Text = "Add Primary", BackgroundColor = Color.Gray };
			var addSecondary = new Button { Text = "Add Secondary", BackgroundColor = Color.Gray };
			var remove = new Button { Text = "Remove", BackgroundColor = Color.Gray };

			buttonLayout.Children.Add(addPrimary);
			buttonLayout.Children.Add(addSecondary);
			buttonLayout.Children.Add(remove);

			Action action = () => page.DisplayAlert(CommandBarActionTitle, CommandBarActionMessage, CommandBarActionDismiss);

			addPrimary.Clicked += (sender, args) =>
			{
				int index = page.ToolbarItems.Count(item => item.Order == ToolbarItemOrder.Primary) + 1;
				page.ToolbarItems.Add(new ToolbarItem($"Primary {index}", "coffee.png", action, ToolbarItemOrder.Primary));
			};

			addSecondary.Clicked += (sender, args) =>
			{
				int index = page.ToolbarItems.Count(item => item.Order == ToolbarItemOrder.Secondary) + 1;
				page.ToolbarItems.Add(new ToolbarItem($"Secondary {index}", "coffee.png", action, ToolbarItemOrder.Secondary));
			};

			remove.Clicked += (sender, args) =>
			{
				if (page.ToolbarItems.Any())
				{
					page.ToolbarItems.RemoveAt(0);
				}
			};

			return layout;
		}

		static Layout CreateChanger(Type enumType, string defaultOption, Action<Picker> selectedIndexChanged, string label)
		{
			var picker = new Picker();
			string[] options = Enum.GetNames(enumType);
			foreach (string option in options)
			{
				picker.Items.Add(option);
			}

			picker.SelectedIndex = options.IndexOf(defaultOption);

			picker.SelectedIndexChanged += (sender, args) => { selectedIndexChanged(picker); };

			var changerLabel = new Label { Text = label, VerticalOptions = LayoutOptions.Center };

			var layout = new Grid
			{
				HorizontalOptions = LayoutOptions.Center,
				ColumnDefinitions = new ColumnDefinitionCollection
				{
					new ColumnDefinition { Width = 150 },
					new ColumnDefinition { Width = 100 }
				},
				Children = { changerLabel, picker }
			};

			Grid.SetColumn(changerLabel, 0);
			Grid.SetColumn(picker, 1);

			return layout;
		}

		static Layout CreateCollapseStyleChanger(MasterDetailPage page)
		{
			Type enumType = typeof(CollapseStyle);

			return CreateChanger(enumType,
				Enum.GetName(enumType, page.On<Windows>().GetCollapseStyle()),
				picker =>
				{
					page.On<Windows>().SetCollapseStyle((CollapseStyle)Enum.Parse(enumType, picker.Items[picker.SelectedIndex]));
				},
				"Select Collapse Style");
		}

		static Layout CreateCollapseWidthAdjuster(MasterDetailPage page)
		{
			var adjustCollapseWidthLabel = new Label
			{
				Text = "Adjust Collapsed Width",
				VerticalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.Center
			};
			var adjustCollapseWidthEntry = new Entry { Text = page.On<Windows>().CollapsedPaneWidth().ToString() };
			var adjustCollapseWidthButton = new Button { Text = "Change", BackgroundColor = Color.Gray };
			adjustCollapseWidthButton.Clicked += (sender, args) =>
			{
				double newWidth;
				if (double.TryParse(adjustCollapseWidthEntry.Text, out newWidth))
				{
					page.On<Windows>().CollapsedPaneWidth(newWidth);
				}
			};

			var adjustCollapsedWidthSection = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				Orientation = StackOrientation.Horizontal,
				Children = { adjustCollapseWidthLabel, adjustCollapseWidthEntry, adjustCollapseWidthButton }
			};

			return adjustCollapsedWidthSection;
		}

		static Layout CreateToolbarPlacementChanger(MasterDetailPage page)
		{
			Type enumType = typeof(ToolbarPlacement);

			return CreateChanger(enumType,
				Enum.GetName(enumType, page.On<Windows>().GetToolbarPlacement()),
				picker =>
				{
					page.On<Windows>().SetToolbarPlacement((ToolbarPlacement)Enum.Parse(enumType, picker.Items[picker.SelectedIndex]));
				}, "Select Toolbar Placement");
		}

		public class NavItem
		{
			public NavItem(string text, string icon, ICommand command)
			{
				Text = text;
				Icon = icon;
				Command = command;
			}

			public ICommand Command { get; set; }

			public string Icon { get; set; }

			public string Text { get; set; }
		}

		public class NavList : ListView
		{
			public NavList(IEnumerable<NavItem> items)
			{
				ItemsSource = items;
				ItemTapped += (sender, args) => (args.Item as NavItem)?.Command.Execute(null);

				ItemTemplate = new DataTemplate(() =>
				{
					var grid = new Grid();
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 48 });
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 200 });

					grid.Margin = new Thickness(0, 10, 0, 10);

					var text = new Label
					{
						VerticalOptions = LayoutOptions.Fill
					};
					text.SetBinding(Label.TextProperty, "Text");

					var glyph = new Label
					{
						FontFamily = "Segoe MDL2 Assets",
						FontSize = 24,
						HorizontalTextAlignment = TextAlignment.Center
					};

					glyph.SetBinding(Label.TextProperty, "Icon");

					grid.Children.Add(glyph);
					grid.Children.Add(text);

					Grid.SetColumn(glyph, 0);
					Grid.SetColumn(text, 1);

					grid.WidthRequest = 48;

					var cell = new ViewCell
					{
						View = grid
					};

					return cell;
				});
			}
		}
	}
}