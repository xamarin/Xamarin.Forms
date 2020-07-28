using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.RadioButtonGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RadioButtonGroupBindingGallery : ContentPage
	{
		public RadioButtonGroupBindingGallery()
		{
			InitializeComponent();
			BindingContext = new RadioButtonGroupBindingModel() { GroupName = "group1" };
		}
	}

	public class RadioButtonGroupBindingModel : INotifyPropertyChanged
	{
		private string _groupName;
		private RadioButton _selection;

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string GroupName 
		{ 
			get => _groupName; 
			set 
			{ 
				_groupName = value; 
				OnPropertyChanged("GroupName"); 
			} 
		}

		public RadioButton Selection
		{
			get => _selection;
			set
			{
				_selection = value;
				OnPropertyChanged("Selection");
			}
		}
	}
}