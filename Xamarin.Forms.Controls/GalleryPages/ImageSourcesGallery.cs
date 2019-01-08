using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Controls
{
	public class ImageSourcesGallery : NavigationPage
	{
		public ImageSourcesGallery()
			: base(new RootPage())
		{
		}

		class RootPage : ContentPage
		{
			bool? _toolbarIcon = null;
			bool? _titleIcon = null;
			bool? _backgroundImage = null;
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
						Children =
						{
							new Button
							{
								Text = "Toggle Title Icon",
								Command = new Command(() =>
								{
									if (_titleIcon == null)
									{
										_titleIcon = true;
										NavigationPage.SetTitleIcon(this, "bank.png");
									}
									else if (_titleIcon == true)
									{
										_titleIcon = false;
										NavigationPage.SetTitleIcon(this, "calculator.png");
									}
									else
									{
										_titleIcon = null;
										NavigationPage.SetTitleIcon(this, null);
									}
								})
							},
							new Button
							{
								Text = "Toggle Menu Icon",
								Command = new Command(() =>
								{
									if (_toolbarIcon == null)
									{
										_toolbarIcon = true;
										_toolbarItem.Icon = "bank.png";
									}
									else if (_toolbarIcon == true)
									{
										_toolbarIcon = false;
										_toolbarItem.Icon = "calculator.png";
									}
									else
									{
										_toolbarIcon = null;
										_toolbarItem.Icon = null;
									}
								})
							},
							new Button
							{
								Text = "Toggle Background",
								Command = new Command(() =>
								{
									if (_backgroundImage == null)
									{
										_backgroundImage = true;
										BackgroundImage = "photo.jpg";
									}
									else if (_backgroundImage == true)
									{
										_backgroundImage = false;
										BackgroundImage = "oasis.jpg";
									}
									else
									{
										_backgroundImage = null;
										BackgroundImage = null;
									}
								})
							},
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
			public ListViewContextActionsPage()
			{
				Title = "ListView Context Actions";

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						Padding = 20,
						Children =
						{
							new Label
							{
								Text = "Each of the items should have the 'bank.png' as the context menu icon.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
							new ListView
							{
								Margin = new Thickness(-20, 0, -20, -20),
								ItemsSource = new[] { "one", "two", "three", "four", "five" },
								ItemTemplate = new DataTemplate(() =>
								{
									var cell = new TextCell();
									cell.ContextActions.Add(new MenuItem
									{
										Text = "bank",
										Icon = "bank.png"
									});
									cell.SetBinding(TextCell.TextProperty, new Binding("."));
									return cell;
								}),
							}
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
							new Label
							{
								Text = "Tap the buttons to swap out the images.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
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
							new Button
							{
								Text = "Clear Image",
								Command = new Command(() => _image.Source = null)
							},
							new Button
							{
								Text = "Resource Image",
								Command = new Command(() => _image.Source = ImageSource.FromFile("bank.png"))
							},
							new Button
							{
								Text = "Embedded Image",
								Command = new Command(() => _image.Source = ImageSource.FromResource("Xamarin.Forms.Controls.GalleryPages.crimson.jpg", typeof(App)))
							},
							new Button
							{
								Text = "Stream Image",
								Command = new Command(() => _image.Source = ImageSource.FromStream(() => typeof(App).Assembly.GetManifestResourceStream("Xamarin.Forms.Controls.coffee.png")))
							},
							new Button
							{
								Text = "URI Image",
								Command = new Command(() => _image.Source = new UriImageSource
								{
									Uri = new Uri("https://raw.githubusercontent.com/xamarin/Xamarin.Forms/master/banner.png"),
									CachingEnabled = false
								})
							},
							new Button
							{
								Text = "Font Image",
								Command = new Command(() =>
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
									_image.Source = new FontImageSource
									{
										Color = Color.Black,
										FontFamily = fontFamily,
										Glyph = "\uf233",
										Size = 100,
									};
								})
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
							new Label
							{
								Text = "The default Button type.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
							new Button
							{
								Text = "Image & Text",
								Image = "bank.png"
							},
							new Button
							{
								Image = "bank.png"
							},
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
							new ImageButton
							{
								HeightRequest = 100,
								Padding = 10,
								Source = "bank.png"
							},
							new ImageButton
							{
								HeightRequest = 100,
								Padding = 10,
								Source = new UriImageSource
								{
									Uri = new Uri("https://raw.githubusercontent.com/xamarin/Xamarin.Forms/master/banner.png"),
									CachingEnabled = false
								}
							},
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
							new Label
							{
								Text = "Tap the buttons to swap out the thumb image.",
								LineBreakMode = LineBreakMode.WordWrap,
							},
							(_slider = new Slider
							{
								Minimum = 0,
								Maximum = 1,
								Value = 0.5,
								HeightRequest = 50
							}),
							new Button
							{
								Text = "Bank",
								Command = new Command(() => _slider.ThumbImage = "bank.png")
							},
							new Button
							{
								Text = "Calculator",
								Command = new Command(() => _slider.ThumbImage = "calculator.png")
							},
							new Button
							{
								Text = "<none>",
								Command = new Command(() => _slider.ThumbImage = null)
							},
						}
					}
				};
			}
		}
	}
}
