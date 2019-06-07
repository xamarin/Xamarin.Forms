using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Sandbox
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShellPage : Shell
	{
		async void Handle_Clicked(object sender, System.EventArgs e)
		{
			var page = new ContentPage()
			{
				
				Content = 

				new ScrollView()
				{
					Content = new StackLayout()
					{
						Children =
					{
						new Label()
						{
							Text = "test1"
						},

						new Label()
						{
							Text = "test"
						},

						new Label()
						{
							Text = "test"
						},

						new Label()
						{
							Text = "test"
						},
						new Button()
						{
							Text = "click",
							Command = new Command(async () =>
							{
								await Navigation.PopAsync();
							})

						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						},
						new Label()
						{
							Text = "test"
						}
					}
					}
				}
			};

			Shell.SetNavBarIsVisible(page, true);
			page.Padding = 0;
			await Navigation.PushAsync(page);
		}



		async void Handle2_Clicked(object sender, System.EventArgs e)
		{
			var page = new ContentPage()
			{
					Content = new StackLayout()
					{
						Children =
						{
							new Label()
							{
								Text = "top"
							},
							new Label()
							{
								Text = "test1"
							},

							new Label()
							{
								Text = "test1"
							},

							new Label()
							{
								Text = "test1"
							},

							new Label()
							{
								Text = "test1"
							},

							new Label()
							{
								Text = "test1"
							},


						}
					}
			};

			Shell.SetBackgroundColor(page, Color.Purple);

			Shell.SetNavBarIsVisible(page, true);
			//page.Padding = 0;
			await Navigation.PushAsync(page);
		}

		public ShellPage()
		{
			InitializeComponent();
		}
	}
}