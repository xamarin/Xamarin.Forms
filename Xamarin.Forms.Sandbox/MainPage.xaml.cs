using System.Windows.Input;

namespace Xamarin.Forms.Sandbox
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			BindingContext = new ViewModel();
		}
	}

	class ViewModel
	{
		public ICommand ButtonClickedCommand { get; }
		public ICommand ButtonClicked2Command { get; }
		public ICommand ListViewItemTappedCommand { get; }
		public object SomeParam { get; } = 1;

		public ViewModel()
		{
			ButtonClickedCommand = new Command(() =>
			{
			});

			ButtonClicked2Command = new Command(o =>
			{
			});

			ListViewItemTappedCommand = new Command<string>(o =>
			{
			});
		}
	}
}