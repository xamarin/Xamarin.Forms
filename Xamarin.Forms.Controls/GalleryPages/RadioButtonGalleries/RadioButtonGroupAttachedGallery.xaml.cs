using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.RadioButtonGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RadioButtonGroupAttachedGallery : ContentPage
	{
		public RadioButtonGroupAttachedGallery()
		{
			InitializeComponent();

			BindingContext = new RadioButtonGroupAttachedGalleryViewModel() { GroupName = "baz" };
		}
	}

	public class RadioButtonGroupAttachedGalleryViewModel : INotifyPropertyChanged
	{
		private string _groupName;
		private RadioButton _selection;

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string GroupName { get => _groupName; set { _groupName = value; OnPropertyChanged("GroupName"); } }

		public RadioButton Selection { get => _selection; 
			set 
			{ 
				_selection = value; 
				OnPropertyChanged("Selection"); 
			} 
		}
	}
}