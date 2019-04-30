using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Preserve(AllMembers = true)]
	public partial class Issue5949_2 : ContentPage
	{
		const string BackButton = "5949GoBack";

		public Issue5949_2()
		{
#if APP
			InitializeComponent();
			BindingContext = new _5949ViewModel();
#endif
		}

		[Preserve(AllMembers = true)]
		class _5949ViewModel
		{
			public _5949ViewModel()
			{
				Items = new List<string>
				{
					"one", "two", "three"
				};
			}

			public List<string> Items { get; set; }
		}

		void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(LoginPage());
		}

		ContentPage LoginPage()
		{
			var page = new ContentPage
			{
				Title = "Issue 5949"
			};

			var button = new Button { Text = "Back", AutomationId = BackButton };

			button.Clicked += ButtonClicked;

			page.Content = button;

			return page;
		}

		private void ButtonClicked(object sender, EventArgs e)
		{
			Application.Current.MainPage = new Issue5949_1();
		}
	}
}