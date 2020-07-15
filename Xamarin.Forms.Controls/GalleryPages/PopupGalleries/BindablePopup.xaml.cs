using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BindablePopup : Popup
	{
		public BindablePopup()
		{
			Size = new Size(700, 1000);
			InitializeComponent();
		}

		Color _defaultBackground = Color.White;
		void BackgroundColor_Clicked(object sender, System.EventArgs e)
		{
			if (Color == _defaultBackground)
				Color = Color.Red;
			else
				Color = _defaultBackground;
		}

		int _currentHorizontalOptions = 1;
		void HorizontalPosition_Clicked(object sender, System.EventArgs e)
		{
			if (_currentHorizontalOptions == 0)
			{
				HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, true);
				_currentHorizontalOptions++;
			}
			else if (_currentHorizontalOptions == 1)
			{
				HorizontalOptions = new LayoutOptions(LayoutAlignment.Center, true);
				_currentHorizontalOptions++;
			}
			else if (_currentHorizontalOptions == 2)
			{
				HorizontalOptions = new LayoutOptions(LayoutAlignment.End, true);
				_currentHorizontalOptions = 0;
			}
		}

		int _currentVerticalOptions = 1;
		void VerticalPosition_Clicked(object sender, System.EventArgs e)
		{
			if (_currentVerticalOptions == 0)
			{
				VerticalOptions = new LayoutOptions(LayoutAlignment.Start, true);
				_currentVerticalOptions++;
			}
			else if (_currentVerticalOptions == 1)
			{
				VerticalOptions = new LayoutOptions(LayoutAlignment.Center, true);
				_currentVerticalOptions++;
			}
			else if (_currentVerticalOptions == 2)
			{
				VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
				_currentVerticalOptions = 0;
			}
		}

		Size _defaultSize = new Size(700, 1000);
		void Size_Clicked(object sender, System.EventArgs e)
		{
			if (_defaultSize.Height == Size.Height && _defaultSize.Width == Size.Width)
				Size = new Size(800, 1100);
			else
				Size = _defaultSize;
		}

		void Close_Clicked(object sender, System.EventArgs e)
		{
			Dismiss(null);
		}
	}
}