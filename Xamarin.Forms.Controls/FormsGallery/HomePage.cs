using System;
using Xamarin.Forms;

namespace FormsGallery
{
	class HomePage : ContentPage
	{
		public HomePage()
		{
			// Define command for the items in the TableView.
			Command<Type> navigateCommand =
				new Command<Type>(async (Type pageType) =>
				{
					Page page = (Page)Activator.CreateInstance(pageType);
					await this.Navigation.PushAsync(page);
				});

			this.Title = "Forms Gallery";
			this.Content = new TableView
			{
				Intent = TableIntent.Menu,
				Root = new TableRoot
					{
						new TableSection("Views for Presentation")
						{
							new TextCell
							{
								Text = "Label",
								Command = navigateCommand,
								CommandParameter = typeof(LabelDemoPage)
							},

							new TextCell
							{
								Text = "Image",
								Command = navigateCommand,
								CommandParameter = typeof(ImageDemoPage)
							},

							new TextCell
							{
								Text = "BoxView",
								Command = navigateCommand,
								CommandParameter = typeof(BoxViewDemoPage)
							},

							new TextCell
							{
								Text = "WebView",
								Command = navigateCommand,
								CommandParameter = typeof(WebViewDemoPage)
							},

							new TextCell
							{
								Text = "Map",
								Command = navigateCommand,
								CommandParameter = typeof(MapDemoPage)
							}
						},

						new TableSection("Views that Initiate Commands")
						{
							new TextCell
							{
								Text = "Button",
								Command = navigateCommand,
								CommandParameter = typeof(ButtonDemoPage)
							},

							new TextCell
							{
								Text = "SearchBar",
								Command = navigateCommand,
								CommandParameter = typeof(SearchBarDemoPage)
							}
						},

						new TableSection("Views for Common Data Types")
						{
							new TextCell
							{
								Text = "Slider (double)",
								Command = navigateCommand,
								CommandParameter = typeof(SliderDemoPage)
							},

							new TextCell
							{
								Text = "Stepper (double)",
								Command = navigateCommand,
								CommandParameter = typeof(StepperDemoPage)
							},

							new TextCell
							{
								Text = "Switch (bool)",
								Command = navigateCommand,
								CommandParameter = typeof(SwitchDemoPage)
							},

							new TextCell
							{
								Text = "DatePicker",
								Command = navigateCommand,
								CommandParameter = typeof(DatePickerDemoPage)
							},

							new TextCell
							{
								Text = "TimePicker",
								Command = navigateCommand,
								CommandParameter = typeof(TimePickerDemoPage)
							}
						},

						new TableSection("Views for Editing Text")
						{
							new TextCell
							{
								Text = "Entry (single line)",
								Command = navigateCommand,
								CommandParameter = typeof(EntryDemoPage)
							},

							new TextCell
							{
								Text = "Editor (multiple lines)",
								Command = navigateCommand,
								CommandParameter = typeof(EditorDemoPage)
							}
						},

						new TableSection("Views to Indicate Activity")
						{
							new TextCell
							{
								Text = "ActivityIndicator",
								Command = navigateCommand,
								CommandParameter = typeof(ActivityIndicatorDemoPage)
							},

							new TextCell
							{
								Text = "ProgressBar",
								Command = navigateCommand,
								CommandParameter = typeof(ProgressBarDemoPage)
							}
						},

						new TableSection("Views that Display Collections")
						{
							new TextCell
							{
								Text = "Picker",
								Command = navigateCommand,
								CommandParameter = typeof(PickerDemoPage)
							},

							new TextCell
							{
								Text = "ListView",
								Command = navigateCommand,
								CommandParameter = typeof(ListViewDemoPage)
							},

							new TextCell
							{
								Text = "TableView for a menu",
								Command = navigateCommand,
								CommandParameter = typeof(TableViewMenuDemoPage)
							},

							new TextCell
							{
								Text = "TableView for a form",
								Command = navigateCommand,
								CommandParameter = typeof(TableViewFormDemoPage)
							}
						},

						new TableSection("Cells")
						{
							new TextCell
							{
								Text = "TextCell",
								Command = navigateCommand,
								CommandParameter = typeof(TextCellDemoPage)
							},

							new TextCell
							{
								Text = "ImageCell",
								Command = navigateCommand,
								CommandParameter = typeof(ImageCellDemoPage)
							},

							new TextCell
							{
								Text = "SwitchCell",
								Command = navigateCommand,
								CommandParameter = typeof(SwitchCellDemoPage)
							},

							new TextCell
							{
								Text = "EntryCell",
								Command = navigateCommand,
								CommandParameter = typeof(EntryCellDemoPage)
							}
						},

						new TableSection("Layouts with Single Content")
						{
							new TextCell
							{
								Text = "ContentView",
								Command = navigateCommand,
								CommandParameter = typeof(ContentViewDemoPage)
							},
							new TextCell
							{
								Text = "Frame",
								Command = navigateCommand,
								CommandParameter = typeof(FrameDemoPage)
							},
							new TextCell
							{
								Text = "ScrollView",
								Command = navigateCommand,
								CommandParameter = typeof(ScrollViewDemoPage)
							}
						},

						new TableSection("Layouts with Multiple Children")
						{
							new TextCell
							{
								Text = "StackLayout",
								Command = navigateCommand,
								CommandParameter = typeof(StackLayoutDemoPage)
							},
							new TextCell
							{
								Text = "AbsoluteLayout",
								Command = navigateCommand,
								CommandParameter = typeof(AbsoluteLayoutDemoPage)
							},
							new TextCell
							{
								Text = "Grid",
								Command = navigateCommand,
								CommandParameter = typeof(GridDemoPage)
							},
							new TextCell
							{
								Text = "RelativeLayout",
								Command = navigateCommand,
								CommandParameter = typeof(RelativeLayoutDemoPage)
							}
						},

						new TableSection("Pages")
						{
							new TextCell
							{
								Text = "ContentPage",
								Command = navigateCommand,
								CommandParameter = typeof(ContentPageDemoPage)
							},
							new TextCell
							{
								Text = "NavigationPage",
								Command = navigateCommand,
								CommandParameter = typeof(NavigationPageDemoPage)
							},
							new TextCell
							{
								Text = "MasterDetailPage",
								Command = navigateCommand,
								CommandParameter = typeof(MasterDetailPageDemoPage)
							},
							new TextCell
							{
								Text = "TabbedPage",
								Command = navigateCommand,
								CommandParameter = typeof(TabbedPageDemoPage)
							},
							new TextCell
							{
								Text = "CarouselPage",
								Command = navigateCommand,
								CommandParameter = typeof(CarouselPageDemoPage)
							}
						}
					}
			};
		}
	}
}
