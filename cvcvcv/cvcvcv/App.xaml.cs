using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace cvcvcv
{
	public class EditorAndButtonReproPage : ContentPage
	{
		public EditorAndButtonReproPage()
		{
			BackgroundColor = Color.Gray;
			Padding = 50;
			var editor = new Editor { HorizontalOptions = LayoutOptions.FillAndExpand };
			var editorButton = new Button { Text = "OK", HorizontalOptions = LayoutOptions.End };
			var editorLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Children = { editor, editorButton }, VerticalOptions = LayoutOptions.Start };
			var endtry = new Entry { Placeholder = "Entry", HorizontalOptions = LayoutOptions.FillAndExpand };
			var entryButton = new Button { Text = "OK", HorizontalOptions = LayoutOptions.End };
			var entryLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Children = { endtry, entryButton }, VerticalOptions = LayoutOptions.Start };
			Content = new StackLayout { Children = { editorLayout, entryLayout } };
		}
	}

	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new EditorAndButtonReproPage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
