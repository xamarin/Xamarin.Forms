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
		enum SelectedElementType
		{
			Polyline,
			Polygon,
			Circle
		}

		SelectedElementType _selectedType;

		Polyline _polyline;
		Polygon _polygon;
		Circle _circle;

		Random _random = new Random();

		public MapElementsGallery()
		{
			InitializeComponent();

			Map.MoveToRegion(
				MapSpan.FromCenterAndRadius(
					new Position(39.828152, -98.569817),
					Distance.FromMiles(1681)));

			_polyline = new Polyline
			{
				Geopath =
				{
					new Position(47.641944, -122.127222),
					new Position(37.411625, -122.071327),
					new Position(35.138901, -80.922623)
				}
			};

			_polygon = new Polygon
			{
				StrokeColor = Color.FromHex("#002868"),
				FillColor = Color.FromHex("#88BF0A30"),
				Geopath =
				{
					new Position(37, -102.05),
					new Position(37, -109.05),
					new Position(41, -109.05),
					new Position(41, -102.05)
				}
			};

			_circle = new Circle
			{
				Center = new Position(42.352364, -71.067177),
				Radius = Distance.FromMiles(100.0),
				StrokeColor = Color.FromRgb(31, 174, 206),
				FillColor = Color.FromRgba(31, 174, 206, 127)
			};

			Map.MapElements.Add(_polyline);
			Map.MapElements.Add(_polygon);
			Map.MapElements.Add(_circle);

			ElementPicker.SelectedIndex = 0;
		}

		void MapClicked(object sender, MapClickedEventArgs e)
		{
			switch (_selectedType)
			{
				case SelectedElementType.Polyline:
					_polyline.Geopath.Add(e.Position);
					break;
				case SelectedElementType.Polygon:
					_polygon.Geopath.Add(e.Position);
					break;
				case SelectedElementType.Circle:
					if (_circle.Center == default(Position))
					{
						_circle.Center = e.Position;
					}
					else
					{
						_circle.Radius = CalculateDistance(_circle.Center, e.Position);
					}
					break;
			}
		}

		Distance CalculateDistance(Position position1, Position position2)
		{
			const double EarthRadiusKm = 6371;

			var latitude1 = DegreeToRadian(position1.Latitude);
			var longitude1 = DegreeToRadian(position1.Longitude);

			var latitude2 = DegreeToRadian(position2.Latitude);
			var longitude2 = DegreeToRadian(position2.Longitude);

			var distance = Math.Sin((latitude2 - latitude1) / 2.0);
			distance *= distance;

			var intermediate = Math.Sin((longitude2 - longitude1) / 2.0);
			intermediate *= intermediate;

			distance = distance + Math.Cos(latitude1) * Math.Cos(latitude2) * intermediate;
			distance = 2 * EarthRadiusKm * Math.Atan2(Math.Sqrt(distance), Math.Sqrt(1 - distance));

			return Distance.FromKilometers(distance);
		}

		double DegreeToRadian(double degree)
		{
			return degree * Math.PI / 180.0;
		}

		void PickerSelectionChanged(object sender, EventArgs e)
		{
			Enum.TryParse((string)ElementPicker.SelectedItem, out _selectedType);
		}

		void AddClicked(object sender, EventArgs e)
		{
			switch (_selectedType)
			{
				case SelectedElementType.Polyline:
					Map.MapElements.Add(_polyline = new Polyline());
					break;
				case SelectedElementType.Polygon:
					Map.MapElements.Add(_polygon = new Polygon());
					break;
				case SelectedElementType.Circle:
					Map.MapElements.Add(_circle = new Circle());
					break;
			}
		}

		void RemoveClicked(object sender, EventArgs e)
		{
			switch (_selectedType)
			{
				case SelectedElementType.Polyline:
					Map.MapElements.Remove(_polyline);
					_polyline = Map.MapElements.OfType<Polyline>().LastOrDefault();

					if (_polyline == null)
						Map.MapElements.Add(_polyline = new Polyline());

					break;
				case SelectedElementType.Polygon:
					Map.MapElements.Remove(_polygon);
					_polygon = Map.MapElements.OfType<Polygon>().LastOrDefault();

					if (_polygon == null)
						Map.MapElements.Add(_polygon = new Polygon());

					break;
				case SelectedElementType.Circle:
					Map.MapElements.Remove(_circle);
					_circle = Map.MapElements.OfType<Circle>().LastOrDefault();

					if (_circle == null)
						Map.MapElements.Add(_circle = new Circle());

					break;
			}
		}

		void ChangeColorClicked(object sender, EventArgs e)
		{
			var newColor = new Color(_random.NextDouble(), _random.NextDouble(), _random.NextDouble());
			switch (_selectedType)
			{
				case SelectedElementType.Polyline:
					_polyline.StrokeColor = newColor;
					break;
				case SelectedElementType.Polygon:
					_polygon.StrokeColor = newColor;
					break;
				case SelectedElementType.Circle:
					_circle.StrokeColor = newColor;
					break;
			}
		}

		void ChangeWidthClicked(object sender, EventArgs e)
		{
			var newWidth = _random.Next(1, 50);
			switch (_selectedType)
			{
				case SelectedElementType.Polyline:
					_polyline.StrokeWidth = newWidth;
					break;
				case SelectedElementType.Polygon:
					_polygon.StrokeWidth = newWidth;
					break;
				case SelectedElementType.Circle:
					_circle.StrokeWidth = newWidth;
					break;
			}
		}

		void ChangeFillClicked(object sender, EventArgs e)
		{
			var newColor = new Color(_random.NextDouble(), _random.NextDouble(), _random.NextDouble(), _random.NextDouble());
			switch (_selectedType)
			{
				case SelectedElementType.Polygon:
					_polygon.FillColor = newColor;
					break;
				case SelectedElementType.Circle:
					_circle.FillColor = newColor;
					break;
			}
		}
	}
}