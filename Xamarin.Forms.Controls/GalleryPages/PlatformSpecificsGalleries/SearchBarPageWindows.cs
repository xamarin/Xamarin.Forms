using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls.GalleryPages.PlatformSpecificsGalleries
{
	public class SearchBarPageWindows : ContentPage
	{
		public SearchBarPageWindows()
		{
			SearchBar searchBar = new SearchBar
			{
				Placeholder = "Placeholder for SearchBar"
			};

			var toggleAutoMaximizeButton = new Button
			{
				Text = "Toggle AutoMaximizeSuggestionArea",
				Command = new Command(() => searchBar.On<Windows>().SetAutoMaximizeSuggestionArea(!searchBar.On<Windows>().AutoMaximizeSuggestionArea()))
			};

			StackLayout content = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					new Label { Text = "Enter the word 'Item' to see various autocompletes below. If needed, the SearchBar will shift upwards if AutoMaximizeSuggestionArea is true; by default, it is false." },
					searchBar,
					toggleAutoMaximizeButton
				}
			};

			var searchableValues = new ObservableCollection<string>();
			for (int i = 0; i <= 50; i++)
				searchableValues.Add("item " + i);

			searchBar.On<Windows>().SetTextChangedAction(() =>
			{
				if (searchBar.Text.Length == 0)
					searchBar.On<Windows>().Suggestions().Clear();
				else
				{
					var filtered = searchableValues.Where(i => i.Contains(searchBar.Text.ToLower()));
					searchBar.On<Windows>().Suggestions().Clear();
					foreach (string i in filtered)
						searchBar.On<Windows>().Suggestions().Add(i);
				}
			});

			Content = content;
		}
	}
}
