using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using AndroidSpecific = Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using iOSSpecific = Xamarin.Forms.PlatformConfiguration.iOSSpecific;
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
		double _buttonBorderWidth = -1;
		ButtonImagePosition _buttonImagePosition = ButtonImagePosition.Left;

		public ButtonLayoutGalleryPage()
			: this(VisualMarker.MatchParent)
		{
		}

		public ButtonLayoutGalleryPage(IVisual visual)
		{
			InitializeComponent();
			Visual = visual;

			// buttons are transparent on default iOS, so we have to give them something
			if (Device.RuntimePlatform == Device.iOS)
			{
				if (Visual != VisualMarker.Material)
				{
					SetBackground(Content);

					void SetBackground(View view)
					{
						if (view is Button button && !button.IsSet(Button.BackgroundColorProperty))
							view.BackgroundColor = Color.LightGray;

						if (view is Layout layout)
						{
							foreach (var child in layout.Children)
							{
								if (child is View childView)
									SetBackground(childView);
							}
						}
					}
				}
			}

			BindingContext = this;
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
			new string[] { "<none>", "bank.png", "oasissmall.jpg", "cover1.jpg" };

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

		public string[] ButtonFlags =>
			new[] { "<none>", "True", "False" };

		public double ButtonBorderWidth
		{
			get => _buttonBorderWidth;
			set
			{
				_buttonBorderWidth = value;
				OnPropertyChanged();

				if (value != -1d)
				{
					autosizedButton.BorderWidth = value;
					autosizedButton.BorderColor = Color.Red;
					stretchedButton.BorderWidth = value;
					stretchedButton.BorderColor = Color.Red;
				}
				else
				{
					autosizedButton.ClearValue(Button.BorderWidthProperty);
					autosizedButton.ClearValue(Button.BorderColorProperty);
					stretchedButton.ClearValue(Button.BorderWidthProperty);
					stretchedButton.ClearValue(Button.BorderColorProperty);
				}
			}
		}

		void OnButtonDefaultShadowChanged(object sender, EventArgs e)
		{
			if (sender is Picker picker)
			{
				if (picker.SelectedItem is string item && bool.TryParse(item, out var value))
				{
					autosizedButton.On<Android>().SetUseDefaultShadow(value).SetUseDefaultPadding(value);
					stretchedButton.On<Android>().SetUseDefaultShadow(value).SetUseDefaultPadding(value);
				}
				else
				{
					autosizedButton.ClearValue(AndroidSpecific.Button.UseDefaultShadowProperty);
					autosizedButton.ClearValue(AndroidSpecific.Button.UseDefaultPaddingProperty);
					stretchedButton.ClearValue(AndroidSpecific.Button.UseDefaultShadowProperty);
					stretchedButton.ClearValue(AndroidSpecific.Button.UseDefaultPaddingProperty);
				}
			}
		}
	}
}
