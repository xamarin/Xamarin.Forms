using System;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IsFocusedBindableGalleryPage : ContentPage
	{
		public IsFocusedBindableGalleryPage()
		{
			InitializeComponent();
			BindingContext = new EntryIsFocusedViewModel();
		}

		void VisualElement_OnFocused(object sender, FocusEventArgs e)
		{
			DisplayLabel.Text = "Focused";
		}

		void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
		{
			DisplayLabel.Text = "Unfocused";
		}

		void Button_Focused_OnClicked(object sender, EventArgs e)
		{
			(BindingContext as EntryIsFocusedViewModel).IsFocused = true;
		}
		
		void Button_Unfocused_OnClicked(object sender, EventArgs e)
		{
			(BindingContext as EntryIsFocusedViewModel).IsFocused = false;
		}
	}

	class EntryIsFocusedViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		bool _isFocused;

		public bool IsFocused
		{
			get => _isFocused;
			set
			{
				_isFocused = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFocused)));
			}
		}


	}
}