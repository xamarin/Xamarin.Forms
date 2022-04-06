using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.RadioButtonGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RadioButtonGroupBindingGallery : ContentPage
	{
		RadioButtonGroupBindingModel _viewModel;

		public RadioButtonGroupBindingGallery()
		{
			InitializeComponent();
			_viewModel = new RadioButtonGroupBindingModel() { GroupName = "group1" };
			BindingContext = _viewModel;
		}

		private void Button_Clicked(object sender, System.EventArgs e)
		{
			_viewModel.Selection = "B";
			_viewModel.Selection2 = "D";
			_viewModel.SelectionBool = !_viewModel.SelectionBool;
		}
	}

	public class RadioButtonGroupBindingModel : INotifyPropertyChanged
	{
		private string _groupName;
		private string _selection = "A";
		private string _selection2 = "C";
		private int _selectionInt = 1;
		private bool _selectionBool = false;
		private object _selectionObject = "False";
		private Pill _selectionEnum = Pill.Blue;

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
				OnPropertyChanged(nameof(GroupName));
			}
		}

		public string Selection
		{
			get => _selection;
			set
			{
				_selection = value;
				OnPropertyChanged(nameof(Selection));
			}
		}


		public string Selection2
		{
			get => _selection2;
			set
			{
				_selection2 = value;
				OnPropertyChanged(nameof(Selection2));
			}
		}


		public int SelectionInt
		{
			get => _selectionInt;
			set
			{
				_selectionInt = value;
				OnPropertyChanged(nameof(_selectionInt));
			}
		}


		public bool SelectionBool
		{
			get => _selectionBool;
			set
			{
				_selectionBool = value;
				OnPropertyChanged(nameof(_selectionBool));
			}
		}

		public object SelectionObject
		{
			get => _selectionObject;
			set
			{
				_selectionObject = value;
				OnPropertyChanged(nameof(_selectionObject));
			}
		}

		public Pill SelectionEnum
		{
			get => _selectionEnum;
			set
			{
				_selectionEnum = value;
				OnPropertyChanged(nameof(_selectionEnum));
			}
		}

	}

	public enum Pill
	{
		Red,
		Blue
	}
}