using System;

namespace Xamarin.Forms.Controls.GalleryPages.GradientGalleries
{
	public partial class CssGradientsPlayground : ContentPage
	{
		public CssGradientsPlayground()
		{
			InitializeComponent();
			BindingContext = new CssGradientsPlaygroundViewModel();
		}
	}

	public class CssGradientsPlaygroundViewModel : BindableObject
	{
		string _css;
		string _error;
		GradientBrush _backgroundBrush;

		public CssGradientsPlaygroundViewModel()
		{
			Css = "linear-gradient(90deg, rgb(255, 0, 0) 0%,rgb(255, 153, 51) 60%)";
		}

		public string Css
		{
			get => _css;
			set
			{
				_css = value;
				UpdateGradientBrush();
				OnPropertyChanged();
			}
		}

		public string Error
		{
			get => _error;
			set
			{
				_error = value;
				OnPropertyChanged();
			}
		}

		public GradientBrush BackgroundBrush
		{
			get => _backgroundBrush;
			set
			{
				_backgroundBrush = value;
				OnPropertyChanged();
			}
		}

		void UpdateGradientBrush()
		{
			try
			{
				var brushTypeConverter = new BrushTypeConverter();
				var gradient = brushTypeConverter.ConvertFromInvariantString(Css);

				BackgroundBrush = (GradientBrush)gradient;
				Error = string.Empty;
			}
			catch(Exception ex)
			{
				Error = ex.Message;
			}
		}
	}
}