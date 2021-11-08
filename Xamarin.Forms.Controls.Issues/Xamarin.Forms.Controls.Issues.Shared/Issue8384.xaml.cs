using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	// Learn more about making custom code visible in the Xamarin.Forms previewer
	// by visiting https://aka.ms/xamarinforms-previewer
	[DesignTimeVisible(false)]
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 8384,
		"[Bug] [5.0] [Android] [Bug] ListView RefreshCommand ActivityIndicator does disappear on Android if CanExecute is changed to false",
		PlatformAffected.Android)]
	public partial class Issue8384 : ContentPage
	{
		public Issue8384()
		{
#if APP
			InitializeComponent();
			BindingContext = new ViewModelIssue8384();
#endif
		}
	}

	class ViewModelIssue8384 : INotifyPropertyChanged
	{
		public class MyCommand : Command
		{
			private bool _allow;

			public MyCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
			{
				Allow = true;
			}

			public bool Allow
			{
				get
				{
					return _allow;
				}
				set
				{
					_allow = value;
					ChangeCanExecute();
				}
			}
		}

		private List<string> _items;
		private bool _isRefreshing;
		private MyCommand _refresh;

		static readonly List<string> FIRST_LIST = new List<string>() {
			"one", "two", "three"
		};

		static readonly List<string> SECOND_LIST = new List<string>() {
			"four", "five", "six"
		};

		public bool IsRefreshing
		{
			get
			{
				return _isRefreshing;
			}
			set
			{
				_isRefreshing = value;
				OnPropertyChanged("IsRefreshing");
			}
		}

		public ViewModelIssue8384()
		{
			Items = FIRST_LIST;

			Refresh = new MyCommand(Execute, CanExecute);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private async void Execute(object parameter)
		{
			IsRefreshing = true;
			Debug.WriteLine("Refresh start");
			await Task.Delay(1000).ConfigureAwait(false);

			// Side note: doing this off the main thread throws an exception
			Device.BeginInvokeOnMainThread(() => { _refresh.Allow = false; });

			await Task.Delay(3000).ConfigureAwait(false);
			Items = (Items == FIRST_LIST) ? SECOND_LIST : FIRST_LIST;

			Debug.WriteLine("Refresh end");
			IsRefreshing = false;

			Device.BeginInvokeOnMainThread(() => { _refresh.Allow = true; });
		}

		private bool CanExecute(object parameter)
		{
			return _refresh.Allow;
		}

		public List<string> Items
		{
			get
			{
				return _items;
			}

			set
			{
				_items = value;
				OnPropertyChanged("Items");
			}
		}

		public MyCommand Refresh
		{
			get
			{
				return _refresh;
			}
			set
			{
				_refresh = value;
				OnPropertyChanged("Refresh");
			}
		}
	}
}
