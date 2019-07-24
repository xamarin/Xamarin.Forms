#if APP
using System;
using System.ComponentModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 6947, "DatePicker SelectedDate", PlatformAffected.All)]
	public partial class Issue6947 : ContentPage
	{
		public Issue6947()
		{
			InitializeComponent();
			BindingContext = new Issue6947ViewModel();
		}

		void ClearBtn_Clicked(object sender, EventArgs e)
		{
			((Issue6947ViewModel)BindingContext).SelectedDate = null;
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue6947ViewModel : INotifyPropertyChanged
	{
		DateTime? _selectedDate = DateTime.Today;

		public event PropertyChangedEventHandler PropertyChanged;

		public DateTime? SelectedDate
		{
			get => _selectedDate;
			set
			{
				if (_selectedDate != value)
				{
					_selectedDate = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDate)));
				}
			}
		}
	}
}
#endif