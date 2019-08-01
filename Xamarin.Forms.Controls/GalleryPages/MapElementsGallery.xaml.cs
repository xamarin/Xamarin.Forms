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
            Polygon
		}

		SelectedElementType _selectedType;
		MapElement _currentElement;
		Random _random = new Random();

		public MapElementsGallery()
		{
			InitializeComponent();
			ElementPicker.SelectedIndex = 0;
		}

		void MapClicked(object sender, MapClickedEventArgs e)
		{
			switch (_currentElement)
			{
				case Polyline polyline:
                    polyline.Geopath.Add(e.Position);
                    break;
				case Polygon polygon:
                    polygon.Geopath.Add(e.Position);
                    break;
			}
		}

		void PickerSelectionChanged(object sender, EventArgs e)
		{
			Enum.TryParse((string)ElementPicker.SelectedItem, out _selectedType);
            AddClicked(sender, e);
		}

		void AddClicked(object sender, EventArgs e)
		{
			switch (_selectedType)
			{
				case SelectedElementType.Polyline:
                    _currentElement = new Polyline();
                    break;
				case SelectedElementType.Polygon:
                    _currentElement = new Polygon();
                    break;
			}

            Map.MapElements.Add(_currentElement);
		}

		void RemoveClicked(object sender, EventArgs e)
		{
			Map.MapElements.Remove(_currentElement);
			_currentElement = Map.MapElements.LastOrDefault();

			if (_currentElement == null)
			{
				AddClicked(sender, e);
			}
		}

		void ChangeColorClicked(object sender, EventArgs e)
		{
			_currentElement.StrokeColor = new Color(_random.NextDouble(), _random.NextDouble(), _random.NextDouble());
		}

		void ChangeWidthClicked(object sender, EventArgs e)
		{
			_currentElement.StrokeWidth = _random.Next(1, 50);
		}

		void ChangeFillClicked(object sender, EventArgs e)
		{
			if (_currentElement is Polygon polygon)
			{
                polygon.FillColor = new Color(_random.NextDouble(), _random.NextDouble(), _random.NextDouble(), _random.NextDouble());
			}
		}
	}
}