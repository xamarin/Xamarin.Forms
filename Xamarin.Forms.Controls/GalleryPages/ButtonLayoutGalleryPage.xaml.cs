using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ButtonImagePosition = Xamarin.Forms.Button.ButtonContentLayout.ImagePosition;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ButtonLayoutGalleryPage : ContentPage
	{
		string _buttonText = "Text";
		string _buttonImage = "bank.png";

		Thickness _buttonPadding = default(Thickness);

		double _buttonImageSpacing = 10;
		ButtonImagePosition _buttonImagePosition = ButtonImagePosition.Left;

		public ButtonLayoutGalleryPage()
		{
			InitializeComponent();

			BindingContext = this;
		}

		public ButtonLayoutGalleryPage(IVisual visual)
			: this()
		{
			Visual = visual;
		}

		public string ButtonText
		{
			get => _buttonText;
			set
			{
				_buttonText = value;
				OnPropertyChanged();
			}
		}

		public string[] ButtonImages =>
			new string[] { "<none>", "bank.png", "cover1.jpg" };

		public string ButtonImage
		{
			get => _buttonImage;
			set
			{
				_buttonImage = value;
				OnPropertyChanged();
			}
		}

		public Thickness ButtonPadding
		{
			get => _buttonPadding;
			set
			{
				_buttonPadding = value;
				OnPropertyChanged();
			}
		}

		public double ButtonImageSpacing
		{
			get => _buttonImageSpacing;
			set
			{
				_buttonImageSpacing = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ButtonImageLayout));
			}
		}

		public ButtonImagePosition[] ButtonImagePositions =>
			(ButtonImagePosition[])Enum.GetValues(typeof(ButtonImagePosition));

		public ButtonImagePosition ButtonImagePosition
		{
			get => _buttonImagePosition;
			set
			{
				_buttonImagePosition = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ButtonImageLayout));
			}
		}

		public Button.ButtonContentLayout ButtonImageLayout =>
			new Button.ButtonContentLayout(ButtonImagePosition, ButtonImageSpacing);
	}
}
