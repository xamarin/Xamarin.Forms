using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapElementsGallery : ContentPage
	{
		Polyline _currentPolyline;
		Random _random = new Random();

		public MapElementsGallery()
		{
			InitializeComponent();
			AddNewPolyline();
		}

		void MapClicked(object sender, MapClickedEventArgs e)
		{
			_currentPolyline.Geopath.Add(e.Position);
		}

		void AddPolylineClicked(object sender, EventArgs e)
		{
			AddNewPolyline();
		}

		void RemovePolylineClicked(object sender, EventArgs e)
		{
			Map.MapElements.Remove(_currentPolyline);
			_currentPolyline = Map.MapElements.OfType<Polyline>().LastOrDefault();

			if (_currentPolyline == null)
			{
				AddNewPolyline();
			}
		}

		void AddNewPolyline()
		{
			Map.MapElements.Add(_currentPolyline = new Polyline());
		}

		void ChangeColorClicked(object sender, EventArgs e)
		{
			_currentPolyline.StrokeColor = new Color(_random.NextDouble(), _random.NextDouble(), _random.NextDouble());
		}

		void ChangeWidthClicked(object sender, EventArgs e)
		{
			_currentPolyline.StrokeWidth = _random.Next(1, 50);
		}
	}
}