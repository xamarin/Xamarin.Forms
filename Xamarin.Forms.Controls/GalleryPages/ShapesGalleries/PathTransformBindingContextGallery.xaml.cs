using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.ShapesGalleries
{
	[Preserve(AllMembers = true)]
	public partial class PathTransformBindingContextGallery : ContentPage
	{
		public PathTransformBindingContextGallery()
		{
			InitializeComponent();
		}
	}

	public class PathTransformBindingContextViewModel : BindableObject
	{
		double _rotation;
		double _centerX;
		double _centerY;
		double _scaleX;
		double _scaleY;
		double _skewX;
		double _skewY;
		double _translateX;
		double _translateY;

		public PathTransformBindingContextViewModel()
		{
			Rotation = 0;
			CenterX = 0;
			CenterY = 0;
			ScaleX = 1;
			ScaleY = 1;
			SkewX = 0;
			SkewY = 0;
			TranslateX = 0;
			TranslateY = 0;
		}

		public double Rotation
		{
			get { return _rotation; }
			set
			{
				_rotation = value;
				OnPropertyChanged();
			}
		}

		public double CenterX
		{
			get { return _centerX; }
			set
			{
				_centerX = value;
				OnPropertyChanged();
			}
		}

		public double CenterY
		{
			get { return _centerY; }
			set
			{
				_centerY = value;
				OnPropertyChanged();
			}
		}

		public double ScaleX
		{
			get { return _scaleX; }
			set
			{
				_scaleX = value;
				OnPropertyChanged();
			}
		}

		public double ScaleY
		{
			get { return _scaleY; }
			set
			{
				_scaleY = value;
				OnPropertyChanged();
			}
		}

		public double SkewX
		{
			get { return _skewX; }
			set
			{
				_skewX = value;
				OnPropertyChanged();
			}
		}

		public double SkewY
		{
			get { return _skewY; }
			set
			{
				_skewY = value;
				OnPropertyChanged();
			}
		}

		public double TranslateX
		{
			get { return _translateX; }
			set
			{
				_translateX = value;
				OnPropertyChanged();
			}
		}
		public double TranslateY
		{
			get { return _translateY; }
			set
			{
				_translateY = value;
				OnPropertyChanged();
			}
		}
	}
}