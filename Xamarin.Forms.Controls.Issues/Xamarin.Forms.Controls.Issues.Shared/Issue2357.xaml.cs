using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2357, "Webview waits to load the content until webviews on previous pages are loaded",
		PlatformAffected.iOS | PlatformAffected.Android)]
	public partial class Issue2357 : MasterDetailPage
	{
		public Issue2357()
		{
			MasterViewModel = new MasterViewModel();
			MasterViewModel.PageSelectionChanged += MasterViewModelOnPageSelectionChanged;
			BindingContext = MasterViewModel;

			Detail = new NavigationPage(new ContentPage
			{
				Title = "Home",
				Content = new Label
				{
					Text = "Hello, Forms !",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand
				}
			});
			InitializeComponent();
		}

		public MasterViewModel MasterViewModel { get; set; }

		protected void ListViewOnItemTapped(object sender, ItemTappedEventArgs e)
		{
			Debug.WriteLine("ListViewOnItemTapped");

			if (((ListView)sender).SelectedItem == null)
				return;

			var menuItem = e.Item as MainMenuItem;

			if (menuItem != null)
			{
				switch (menuItem.MenuType)
				{
					case MenuType.Login:
					{
						break;
					}

					case MenuType.WebView:
					{
						var webViewViewModel = new WebViewViewModel(menuItem);
						MasterViewModel.CurrentDetailPage = new CustomWebView(webViewViewModel);
						break;
					}

					default:
					{
						//MenuType Standard
						break;
					}
				}

				((ListView)sender).SelectedItem = null;
			}
		}

		protected override async void OnAppearing()
		{
			await TryInitializeMasterViewModel();
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			//MasterViewModel.PageSelectionChanged -= MasterViewModelOnPageSelectionChanged;
			base.OnDisappearing();
		}

		async void MasterViewModelOnPageSelectionChanged(object sender, NavigationEventArgs eventArgs)
		{
			Debug.WriteLine("MasterViewModelOnPageSelectionChanged");
			IsPresented = false;
			Page page = eventArgs.Page;
			await Detail.Navigation.PushAsync(page, true);
		}

		async Task TryInitializeMasterViewModel()
		{
			while (true)
			{
				string errorMessage;
				try
				{
					await MasterViewModel.InitializeAsync();
					break;
				}
				catch (Exception ex)
				{
					Insights.Report(ex, Insights.Severity.Error);
					errorMessage = ex.Message;
				}

				if (!string.IsNullOrWhiteSpace(errorMessage))
				{
					bool retry = await DisplayAlert("Error", errorMessage, "Retry", "Close Application");
					if (retry)
					{
						continue;
					}
				}

				break;
			}
		}
	}

	internal class CustomWebView : ContentPage
	{
		WebView _titledWebView;

		public CustomWebView()
		{
			_titledWebView = new WebView();
			_titledWebView.SetBinding(WebView.SourceProperty, new Binding("Url"));
			_titledWebView.Navigating += WebView_OnNavigating;
			this.SetBinding(TitleProperty, "Title");
			Content = _titledWebView;
		}

		public CustomWebView(WebViewViewModel webViewViewModel) : this()
		{
			Debug.WriteLine("New WebView");

			_titledWebView.BindingContext = webViewViewModel;
		}

		static Uri GetSourceUrl(WebViewSource source)
		{
			Debug.Assert(source != null, "source cannot be null.");

			var urlWebViewSource = source as UrlWebViewSource;
			if (urlWebViewSource != null)
			{
				if (urlWebViewSource.Url.IsValidAbsoluteUrl())
				{
					return new Uri(urlWebViewSource.Url);
				}
			}

			throw new InvalidOperationException("WebViewSource is Invalid. Only UrlWebViewSource is accepted.");
		}

		static void WebView_OnNavigating(object sender, WebNavigatingEventArgs e)
		{
			Debug.WriteLine("OS: " + Device.RuntimePlatform + " Current Url: " + GetSourceUrl(((WebView)sender).Source) +
							"Destination Url: " + e.Url + " " + DateTime.Now);

			if (e.Url.IsValidAbsoluteUrl())
			{
				var destinationUri = new Uri(e.Url);
				Uri sourceUri = GetSourceUrl(((WebView)sender).Source);
				if (sourceUri.HasSameHost(destinationUri))
				{
					if (destinationUri == sourceUri)
					{
						//Do nothing. This happens on webview load
						Debug.WriteLine("WebView_OnNavigating Same URI");
						return;
					}

					//If it reaches here, A link could have been clicked.
					e.Cancel = true;
					Debug.WriteLine("WebView_OnNavigating Same Host but different Uri");
				}
				else
				{
					//if external link is clicked
					Debug.WriteLine("WebView_OnNavigating, DIfferent Uri, so open in Native Browser");
					e.Cancel = true;
					Device.OpenUri(new Uri(e.Url));
				}
			}
		}
	}

	public static class UriExtensions
	{
		public static bool HasSameHost(this Uri sourceUri, Uri destinationUri, UriFormat uriFormat = UriFormat.Unescaped)
		{
			Debug.Assert(sourceUri != null, "sourceUri cannot be null.");
			Debug.Assert(destinationUri != null, "destinationUri cannot be null.");

			return destinationUri.GetComponents(UriComponents.Host, uriFormat) ==
					sourceUri.GetComponents(UriComponents.Host, uriFormat);
		}
	}

	public static class StringExtensions
	{
		public static bool IsValidAbsoluteUrl(this string stringValue)
		{
			Uri result;
			return !string.IsNullOrWhiteSpace(stringValue) && Uri.TryCreate(stringValue, UriKind.Absolute, out result) &&
					(result.Scheme == "http" || result.Scheme == "https");
		}
	}

	public delegate void PageSelectionChanged(object sender, NavigationEventArgs e);

	public class MasterViewModel : ViewModelBase1
	{
		Page _currentDetailPage;
		ObservableCollection<MainMenuItem> _mainMenuItems;

		public MasterViewModel()
		{
			_mainMenuItems = new ObservableCollection<MainMenuItem>(Enumerable.Empty<MainMenuItem>());
		}

		public Page CurrentDetailPage
		{
			get { return _currentDetailPage; }
			set
			{
				_currentDetailPage = value;

				PageSelectionChanged handler = PageSelectionChanged;
				if (handler != null)
				{
					handler(null, new NavigationEventArgs(value));
				}
			}
		}

		public ObservableCollection<MainMenuItem> MainMenuItems
		{
			get { return _mainMenuItems; }
			set
			{
				_mainMenuItems = value;
				OnPropertyChanged("MainMenuItems");
			}
		}

		public Task InitializeAsync()
#pragma warning restore 1998
		{
			var items = new List<MainMenuItem>();
			items.Add(new MainMenuItem
			{
				Title = "SHORT",
				MenuType = MenuType.WebView,
				Uri = new Uri("http://api.morgans.bluearc-uat.com/mobile/SamplePage.aspx?page=Portfolio")
			});
			items.Add(new MainMenuItem
			{
				Title = "LONG",
				MenuType = MenuType.WebView,
				Uri = new Uri("http://api.morgans.bluearc-uat.com/mobile/SamplePage.aspx?page=long")
			});

			MainMenuItems = new ObservableCollection<MainMenuItem>(items);
			return Task.FromResult(true);
		}

		public static event PageSelectionChanged PageSelectionChanged;
	}

	public class WebViewViewModel : ViewModelBase1
	{
		string _title;
		string _url;

		public WebViewViewModel(MainMenuItem menuItem)
		{
			Debug.WriteLine("New WebViewViewModel");
			_title = menuItem.Title;
			_url = menuItem.Uri.AbsoluteUri;
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}

		public string Url
		{
			get { return _url; }
			set
			{
				Debug.WriteLine("WebViewViewModel Url Changed");
				_url = value;
				OnPropertyChanged("Url");
			}
		}
	}

	public interface IMenuService
	{
		Task<IEnumerable<MainMenuItem>> GetMenuItemsAsync();
	}

	public class MainMenuItem
	{
		public object Id { get; set; }

		public MenuType MenuType { get; set; }

		public string Title { get; set; }

		public Uri Uri { get; set; }
	}

	public enum MenuType
	{
		Login,
		WebView,
		Standard
	}

	public class ViewModelBase1 : INotifyPropertyChanged
	{
		bool _isBusy;

		/// <summary>
		/// Default constructor
		/// </summary>
		public ViewModelBase1()
		{
			//Make sure validation is performed on startup
			Validate();
		}

		/// <summary>
		/// An aggregated error message
		/// </summary>
		public string Error
		{
			get { return Errors.Aggregate(new StringBuilder(), (b, s) => b.AppendLine(s)).ToString().Trim(); }
		}

		/// <summary>
		/// Value indicating if a spinner should be shown
		/// </summary>
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				if (_isBusy != value)
				{
					_isBusy = value;

					OnPropertyChanged("IsBusy");
					OnIsBusyChanged();
				}
			}
		}

		/// <summary>
		/// Returns true if the current state of the ViewModel is valid
		/// </summary>
		public bool IsValid
		{
			get { return Errors.Count == 0; }
		}

		/// <summary>
		/// A list of errors if IsValid is false
		/// </summary>
		protected List<string> Errors { get; } = new List<string>();

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event for when IsBusy changes
		/// </summary>
		public event EventHandler IsBusyChanged;

		/// <summary>
		/// Event for when IsValid changes
		/// </summary>
		public event EventHandler IsValidChanged;

		/// <summary>
		/// Other viewmodels can override this if something should be done when busy
		/// </summary>
		protected void OnIsBusyChanged()
		{
			EventHandler ev = IsBusyChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler ev = PropertyChanged;
			if (ev != null)
			{
				ev(this, new PropertyChangedEventArgs(name));
			}
		}

		/// <summary>
		/// Protected method for validating the ViewModel
		/// - Fires PropertyChanged for IsValid and Errors
		/// </summary>
		protected void Validate()
		{
			OnPropertyChanged("IsValid");
			OnPropertyChanged("Errors");

			EventHandler method = IsValidChanged;
			if (method != null)
				method(this, EventArgs.Empty);
		}

		/// <summary>
		/// Other viewmodels should call this when overriding Validate, to validate each property
		/// </summary>
		/// <param name="validate">Func to determine if a value is valid</param>
		/// <param name="error">The error message to use if not valid</param>
		protected void ValidateProperty(Func<bool> validate, string error)
		{
			if (validate())
			{
				if (!Errors.Contains(error))
					Errors.Add(error);
			}
			else
			{
				Errors.Remove(error);
			}
		}
	}
#endif
}