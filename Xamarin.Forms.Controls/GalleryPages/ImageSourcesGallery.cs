using System;

namespace Xamarin.Forms.Controls
{
	public class ImageSourcesGallery : NavigationPage
	{
		public ImageSourcesGallery()
			: base(new RootPage())
		{
		}

		static Picker CreateImageSourcePicker(string title, Action<Func<ImageSource>> onSelected)
		{
			var items = new[]
			{
				new ImageSourcePickerItem
				{
					Text = "<none>",
					Getter = () => null
				},
				new ImageSourcePickerItem
				{
					Text = "App Resource",
					Getter = () => ImageSource.FromFile("bank.png")
				},
				new ImageSourcePickerItem
				{
					Text = "Embedded",
					Getter = () => ImageSource.FromResource("Xamarin.Forms.Controls.GalleryPages.crimson.jpg", typeof(App))
				},
				new ImageSourcePickerItem
				{
					Text = "Stream",
					Getter = () => ImageSource.FromStream(() => typeof(App).Assembly.GetManifestResourceStream("Xamarin.Forms.Controls.coffee.png"))
				},
				new ImageSourcePickerItem
				{
					Text = "URI",
					Getter = () => new UriImageSource
					{
						Uri = new Uri("https://beehive.blob.core.windows.net/staticimages/FeatureImages/MutantLizard01.png"),
						CachingEnabled = false
					}
				},
				new ImageSourcePickerItem
				{
					Text = "Font Glyph",
					Getter = () =>
					{
						var fontFamily = "";
						switch (Device.RuntimePlatform)
						{
							case Device.iOS:
								fontFamily = "Ionicons";
								break;
							case Device.UWP:
								fontFamily = "Assets/Fonts/ionicons.ttf#ionicons";
								break;
							case Device.Android:
							default:
								fontFamily = "fonts/ionicons.ttf#";
								break;
						}
						return new FontImageSource
						{
							Color = Color.Black,
							FontFamily = fontFamily,
							Glyph = "\uf233",
							Size = 24,
						};
					}
				},
			};

			var picker = new Picker
			{
				Title = title,
				ItemsSource = items,
				ItemDisplayBinding = new Binding("Text"),
			};

			picker.SelectedIndexChanged += (sender, e) =>
			{
				var item = (ImageSourcePickerItem)picker.SelectedItem;
				var text = item.Text;
				onSelected?.Invoke(() => item.Getter());
			};

			return picker;
		}

		class ImageSourcePickerItem
		{
			public string Text { get; set; }

			public Func<ImageSource> Getter { get; set; }
		}

		class RootPage : ContentPage
		{
			ToolbarItem _toolbarItem;

			public RootPage()
			{
				Title = "Image Source Tests";

				ToolbarItems.Add(_toolbarItem = new ToolbarItem("MENU", null, delegate
				{
				}));

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Padding = 20,
						Spacing = 10,
						Children =
						{
							CreateImageSourcePicker("Change Title Icon", getter => NavigationPage.SetTitleIcon(this, getter())),
							CreateImageSourcePicker("Change Toolbar Icon", getter => _toolbarItem.Icon = getter()),
							CreateImageSourcePicker("Change Background", getter => BackgroundImage = getter()),
							new Button
							{
								Text = "ListView Context Actions",
								Command = new Command(() => Navigation.PushAsync(new ListViewContextActionsPage()))
							},
							new Button
							{
								Text = "Image View",
								Command = new Command(() => Navigation.PushAsync(new ImageViewPage()))
							},
							new Button
							{
								Text = "Buttons",
								Command = new Command(() => Navigation.PushAsync(new ButtonsPage()))
							},
							new Button
							{
								Text = "Slider",
								Command = new Command(() => Navigation.PushAsync(new SliderPage()))
							},
						}
					}
				};
			}
		}

		class ListViewContextActionsPage : ContentPage
		{
			ImageSource _source;
			ListView _listView;

			public ListViewContextActionsPage()
			{
				Title = "ListView Context Actions";

				var items = new[] { "one", "two", "three", "four", "five" };

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Padding = 20,
						Children =
						{
							new Label
							{
								Text = "Select the item source from the picker and then view the context menu of each item.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
							CreateImageSourcePicker("Select Icon Source", getter =>
							{
								_source = getter();
								_listView.ItemsSource = null;
								_listView.ItemsSource = items;
							}),
							(_listView = new ListView
							{
								Margin = new Thickness(-20, 0, -20, -20),
								ItemsSource = items,
								ItemTemplate = new DataTemplate(() =>
								{
									var cell = new TextCell();
									cell.ContextActions.Add(new MenuItem
									{
										Text = "bank",
										Icon = _source
									});
									cell.SetBinding(TextCell.TextProperty, new Binding("."));
									return cell;
								}),
							})
						}
					}
				};
			}
		}

		class ImageViewPage : ContentPage
		{
			Image _image = null;
			ActivityIndicator _loading = null;

			public ImageViewPage()
			{
				Title = "Image View";

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Padding = 20,
						Children =
						{
							CreateImageSourcePicker("Select Image Source", getter => _image.Source = getter()),
							new Grid
							{
								Children =
								{
									(_image = new Image
									{
										WidthRequest = 200,
										HeightRequest = 200,
										Source = "bank.png",
									}),
									(_loading = new ActivityIndicator
									{
										WidthRequest = 100,
										HeightRequest = 100
									}),
								}
							},
						}
					}
				};

				_loading.SetBinding(ActivityIndicator.IsRunningProperty, new Binding(Image.IsLoadingProperty.PropertyName));
				_loading.SetBinding(ActivityIndicator.IsVisibleProperty, new Binding(Image.IsLoadingProperty.PropertyName));
				_loading.BindingContext = _image;
			}
		}

		class ButtonsPage : ContentPage
		{
			Button _buttonWithImageAndText;
			Button _buttonWithImage;
			ImageButton _imageButton;

			public ButtonsPage()
			{
				Title = "Buttons";

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Padding = 20,
						Children =
						{
							CreateImageSourcePicker("Select Image Source", getter =>
							{
								_buttonWithImageAndText.Image = getter();
								_buttonWithImage.Image = getter();
								_imageButton.Source = getter();
							}),
							new Label
							{
								Text = "The default Button type.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
							(_buttonWithImageAndText = new Button
							{
								Text = "Image & Text",
								Image = "bank.png"
							}),
							(_buttonWithImage = new Button
							{
								Image = "bank.png"
							}),
							new Button
							{
								Text = "Just Text",
								Image = null
							},
							new Label
							{
								Text = "The ImageButton type.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
							(_imageButton = new ImageButton
							{
								Padding = 10,
								Source = "bank.png",
							}),
						}
					}
				};
			}
		}

		class SliderPage : ContentPage
		{
			Slider _slider = null;

			public SliderPage()
			{
				Title = "Slider";

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Padding = 20,
						Children =
						{
							CreateImageSourcePicker("Select Image Source", getter => _slider.ThumbImage = getter()),
							(_slider = new Slider
							{
								Minimum = 0,
								Maximum = 1,
								Value = 0.5,
								HeightRequest = 50
							}),
						}
					}
				};
			}
		}
	}
}
