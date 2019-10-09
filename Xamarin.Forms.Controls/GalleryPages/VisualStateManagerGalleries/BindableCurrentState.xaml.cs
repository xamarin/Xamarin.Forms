using System;
using System.ComponentModel;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.VisualStateManagerGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BindableCurrentState : ContentPage
	{
		public BindableCurrentState()
		{
			InitializeComponent();
			BindingContext = new TestViewModel();
		}

		void OnClicked(object sender, EventArgs e)
		{
			var model = BindingContext as TestViewModel;
			if (model.LabelState == "Normal")
			{
				model.LabelState = "Invalid";
			}
			else
			{
				model.LabelState = "Normal";
			}
		}

		void OnClicked_Manual(object sender, EventArgs e)
		{
			if (VisualStateManager.GetCurrentState(label) == "Normal")
			{
				VisualStateManager.GoToState(label, "Invalid");
			}
			else
			{
				VisualStateManager.GoToState(label, "Normal");
			}
		}

		class TestViewModel : INotifyPropertyChanged
		{
			string _currentState;

			public string LabelState
			{
				get => _currentState;
				set
				{
					_currentState = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelState)));
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}
	}
}