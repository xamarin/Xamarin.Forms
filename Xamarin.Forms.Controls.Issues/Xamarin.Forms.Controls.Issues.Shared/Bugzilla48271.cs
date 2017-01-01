using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

// Apply the default category of "Issues" to all of the tests in this assembly
// We use this as a catch-all for tests which haven't been individually categorized
#if UITEST
[assembly: NUnit.Framework.Category("Issues")]
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 48271, "ListView with custom cell causes selected item to be null", PlatformAffected.iOS)]
	public class Bugzilla48271 : TestNavigationPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			PushAsync(new ContentPage
			{
				Content = new StackLayout
				{
					Spacing = 10,
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Children =
					{
						new Button
						{
							Text = "Retain",
							Command = new Command(async () =>
							{
								await Navigation.PushAsync(new ContentPage48271(ListViewCachingStrategy.RetainElement));
							})
						},
						new Button
						{
							Text = "Recycle",
							Command = new Command(async () =>
							{
								await Navigation.PushAsync(new ContentPage48271(ListViewCachingStrategy.RecycleElement));
							})
						}
					}
				}
			});
		}
	}

	[Preserve(AllMembers = true)]
	public class ContentPage48271 : ContentPage
	{
		public ContentPage48271(ListViewCachingStrategy cachingStrategy)
		{
			BindingContext = new ViewModel48271();

			var listView = new ListView(cachingStrategy)
			{
				HasUnevenRows = true,
				ItemTemplate = new DataTemplate(() =>
				{
					var customCell = new ViewCell48271();
					customCell.SetBinding(ViewCell48271.TextProperty, new Binding("Text"));
					customCell.SetBinding(ViewCell48271.DetailProperty, new Binding("Detail"));
					return customCell;
				}),
				ItemsSource = ((ViewModel48271)BindingContext).ItemList
			};
			listView.ItemSelected += ListViewOnItemSelected;
			listView.ItemTapped += ListViewOnItemTapped;

			Content = listView;
		}

		async void ListViewOnItemTapped(object sender, ItemTappedEventArgs itemTappedEventArgs)
		{
			var item = (Model48271)itemTappedEventArgs.Item;
			await DisplayAlert("Alert", item.Text + ", " + item.Detail, "OK");
		}

		async void ListViewOnItemSelected(object sender, SelectedItemChangedEventArgs selectedItemChangedEventArgs)
		{
			var item = (Model48271)selectedItemChangedEventArgs.SelectedItem;
			await DisplayAlert("Alert", item.Text + ", " + item.Detail, "OK");
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModel48271 : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		ObservableCollection<Model48271> _itemList;
		public ObservableCollection<Model48271> ItemList
		{
			get { return _itemList; }
			set
			{
				_itemList = value;
				OnPropertyChanged();
			}
		}

		void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ViewModel48271()
		{
			ItemList = new ObservableCollection<Model48271>
			{
				new Model48271("1", "Row 1"),
				new Model48271("2", "Row 2"),
				new Model48271("3", "Row 3")
			};
		}
	}

	[Preserve(AllMembers = true)]
	public class Model48271
	{
		public string Text { get; }

		public string Detail { get; }

		public Model48271(string text, string detail)
		{
			Text = text;
			Detail = detail;
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewCell48271 : ViewCell
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(ViewCell48271), "");
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly BindableProperty DetailProperty = BindableProperty.Create("Detail", typeof(string), typeof(ViewCell48271), "");
		public string Detail
		{
			get { return (string)GetValue(DetailProperty); }
			set { SetValue(DetailProperty, value); }
		}
	}
}