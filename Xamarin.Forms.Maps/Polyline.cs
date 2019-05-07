using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Xamarin.Forms.Maps
{
	public class Polyline : BindableObject
	{
		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
			nameof(StrokeColor),
			typeof(Color), 
			typeof(Polyline),
			Color.Default);

		public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(
			nameof(StrokeWidth), 
			typeof(float), 
			typeof(Polyline), 
			5f);

		public Color StrokeColor
		{
			get => (Color)GetValue(StrokeColorProperty);
			set => SetValue(StrokeColorProperty, value);
		}

		public float StrokeWidth
		{
			get => (float)GetValue(StrokeWidthProperty);
			set => SetValue(StrokeWidthProperty, value);
		}

		public IList<Position> Geopath { get; } = new ObservableCollection<Position>();

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object PolylineId { get; set; }
	}
}
