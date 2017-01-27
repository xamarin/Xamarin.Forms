using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	public class ViewModel : INotifyPropertyChanged
	{
		bool _shouldBeEnabled;
		bool _shouldBeToggled;

		public bool ShouldBeEnabled
		{
			get { return _shouldBeEnabled; }
			set
			{
				_shouldBeEnabled = value;
				OnPropertyChanged();
			}
		}

		public bool ShouldBeToggled
		{
			get { return _shouldBeToggled; }
			set
			{
				_shouldBeToggled = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}

#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1747, "Binding to Switch.IsEnabled has no effect", PlatformAffected.WinPhone)]
	public partial class Issue1747 : ContentPage
	{
		readonly ViewModel _vm;

		public Issue1747()
		{
			_vm = new ViewModel();
			BindingContext = _vm;
			InitializeComponent();
		}

		public void Button_OnClick(object sender, EventArgs args)
		{
			_vm.ShouldBeToggled = !_vm.ShouldBeToggled;
			_vm.ShouldBeEnabled = !_vm.ShouldBeEnabled;
		}
	}
#endif
}