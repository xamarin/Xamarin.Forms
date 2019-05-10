using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.Maps.UWP
{
	public class MapRenderer : ViewRenderer<Map, MapControl>
	{
		protected override async void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				var mapModel = e.OldElement;
				MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");
				((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
				((ObservableCollection<Polyline>)mapModel.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
			}

			if (e.NewElement != null)
			{
				var mapModel = e.NewElement;

				if (Control == null)
				{
					SetNativeControl(new MapControl());
					Control.MapServiceToken = FormsMaps.AuthenticationToken;
					Control.ZoomLevelChanged += async (s, a) => await UpdateVisibleRegion();
					Control.CenterChanged += async (s, a) => await UpdateVisibleRegion();
					Control.MapTapped += OnMapTapped;
				}

				MessagingCenter.Subscribe<Map, MapSpan>(this, "MapMoveToRegion", async (s, a) => await MoveToRegion(a), mapModel);

				UpdateMapType();
				UpdateHasScrollEnabled();
				UpdateHasZoomEnabled();

				((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged += OnPinCollectionChanged;
				if (mapModel.Pins.Any())
					LoadPins();

				((ObservableCollection<Polyline>)mapModel.Polylines).CollectionChanged += OnPolylineCollectionChanged;
				if (mapModel.Polylines.Any())
					LoadPolylines();

				if (Control == null) return;

				await Control.Dispatcher.RunIdleAsync(async (i) => await MoveToRegion(mapModel.LastMoveToRegion, MapAnimationKind.None));
				await UpdateIsShowingUser();
			}
		}

		protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Map.MapTypeProperty.PropertyName)
				UpdateMapType();
			else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
				await UpdateIsShowingUser();
			else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
				UpdateHasScrollEnabled();
			else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
				UpdateHasZoomEnabled();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;

				_timer?.Stop();
				_timer = null;

				MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");

				if (Element != null)
				{
					((ObservableCollection<Pin>)Element.Pins).CollectionChanged -= OnPinCollectionChanged;
					((ObservableCollection<Polyline>)Element.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
				}
			}

			base.Dispose(disposing);
		}

		bool _disposed;
		Ellipse _userPositionCircle;
		DispatcherTimer _timer;

		void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (Pin pin in e.NewItems)
						LoadPin(pin);
					break;
				case NotifyCollectionChangedAction.Move:
					// no matter
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Pin pin in e.OldItems)
						RemovePin(pin);
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (Pin pin in e.OldItems)
						RemovePin(pin);
					foreach (Pin pin in e.NewItems)
						LoadPin(pin);
					break;
				case NotifyCollectionChangedAction.Reset:
					ClearPins();
					break;
			}
		}

		void LoadPins()
		{
			foreach (var pin in Element.Pins)
				LoadPin(pin);
		}

		void ClearPins()
		{
			Control.Children.Clear();
#pragma warning disable 4014 // don't wanna block UI thread
			UpdateIsShowingUser();
#pragma warning restore
		}

		void RemovePin(Pin pinToRemove)
		{
			var pushPin = Control.Children.FirstOrDefault(c =>
			{
				var pin = (c as PushPin);
				return (pin != null && pin.DataContext.Equals(pinToRemove));
			});

			if (pushPin != null)
				Control.Children.Remove(pushPin);
		}

		void LoadPin(Pin pin)
		{
			Control.Children.Add(new PushPin(pin));
		}

		void OnPolylineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (Polyline polyline in e.NewItems)
						LoadPolyline(polyline);
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (Polyline polyline in e.OldItems)
						RemovePolyline(polyline);
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (Polyline polyline in e.OldItems)
						RemovePolyline(polyline);
					foreach (Polyline polyline in e.NewItems)
						LoadPolyline(polyline);
					break;
				case NotifyCollectionChangedAction.Reset:
					Control.MapElements.Clear();
					LoadPins();
					break;
			}
		}

		void LoadPolylines()
		{
			foreach (var polyline in Element.Polylines)
			{
				LoadPolyline(polyline);
			}
		}

		void LoadPolyline(Polyline polyline)
		{
			polyline.PropertyChanged += OnPolylinePropertyChanged;

			if (polyline.Geopath.Any())
			{
				var mapPolyline = new MapPolyline()
				{
					Path = new Geopath(polyline.Geopath.Select(position => new BasicGeoposition()
					{
						Latitude = position.Latitude,
						Longitude = position.Longitude
					})),
					StrokeColor = polyline.StrokeColor.IsDefault ? Colors.Black : polyline.StrokeColor.ToWindowsColor(),
					StrokeThickness = polyline.StrokeWidth
				};

				polyline.PolylineId = mapPolyline;

				Control.MapElements.Add(mapPolyline);
			}
		}

		void RemovePolyline(Polyline polyline)
		{
			polyline.PropertyChanged -= OnPolylinePropertyChanged;
			Control.MapElements.Remove((MapPolyline)polyline.PolylineId);
		}

		void OnPolylinePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var polyline = (Polyline)sender;
			var mapPolyline = (MapPolyline)polyline.PolylineId;

			if (mapPolyline == null || polyline.Geopath.Count == 0)
			{
				RemovePolyline(polyline);
				LoadPolyline(polyline);
				return;
			}

			if (e.PropertyName == Polyline.StrokeColorProperty.PropertyName)
			{
				mapPolyline.StrokeColor = polyline.StrokeColor.IsDefault ? Colors.Black : polyline.StrokeColor.ToWindowsColor();
			}
			else if (e.PropertyName == Polyline.StrokeWidthProperty.PropertyName)
			{
				mapPolyline.StrokeThickness = polyline.StrokeWidth;
			}
			else if (e.PropertyName == nameof(polyline.Geopath))
			{
				mapPolyline.Path = new Geopath(polyline.Geopath.Select(position => new BasicGeoposition()
				{
					Latitude = position.Latitude,
					Longitude = position.Longitude
				}));
			}
		}

		async Task UpdateIsShowingUser(bool moveToLocation = true)
		{
			if (Control == null || Element == null) return;

			if (Element.IsShowingUser)
			{
				var myGeolocator = new Geolocator();
				if (myGeolocator.LocationStatus != PositionStatus.NotAvailable &&
					myGeolocator.LocationStatus != PositionStatus.Disabled)
				{
					var userPosition = await myGeolocator.GetGeopositionAsync();
					if (userPosition?.Coordinate != null)
						LoadUserPosition(userPosition.Coordinate, moveToLocation);
				}

				if (Control == null || Element == null) return;

				if (_timer == null)
				{
					_timer = new DispatcherTimer();
					_timer.Tick += async (s, o) => await UpdateIsShowingUser(moveToLocation: false);
					_timer.Interval = TimeSpan.FromSeconds(15);
				}

				if (!_timer.IsEnabled)
					_timer.Start();
			}
			else if (_userPositionCircle != null && Control.Children.Contains(_userPositionCircle))
			{
				_timer.Stop();
				Control.Children.Remove(_userPositionCircle);
			}
		}

		async Task MoveToRegion(MapSpan span, MapAnimationKind animation = MapAnimationKind.Bow)
		{
			var nw = new BasicGeoposition
			{
				Latitude = span.Center.Latitude + span.LatitudeDegrees / 2,
				Longitude = span.Center.Longitude - span.LongitudeDegrees / 2
			};
			var se = new BasicGeoposition
			{
				Latitude = span.Center.Latitude - span.LatitudeDegrees / 2,
				Longitude = span.Center.Longitude + span.LongitudeDegrees / 2
			};
			var boundingBox = new GeoboundingBox(nw, se);
			await Control.TrySetViewBoundsAsync(boundingBox, null, animation);
		}

		async Task UpdateVisibleRegion()
		{
			if (Control == null || Element == null)
				return;
			try
			{
				Geopoint nw, se = null;
				Control.GetLocationFromOffset(new Windows.Foundation.Point(0, 0), out nw);
				Control.GetLocationFromOffset(new Windows.Foundation.Point(Control.ActualWidth, Control.ActualHeight), out se);

				if (nw != null && se != null)
				{
					var boundingBox = new GeoboundingBox(nw.Position, se.Position);
					var center = new Position(boundingBox.Center.Latitude, boundingBox.Center.Longitude);
					var latitudeDelta = Math.Abs(nw.Position.Latitude - se.Position.Latitude);
					var longitudeDelta = Math.Abs(nw.Position.Longitude - se.Position.Longitude);
					await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
					{
						Element.SetVisibleRegion(new MapSpan(center, latitudeDelta, longitudeDelta));
					});
				}
			}
			catch (Exception)
			{
				return;
			}
		}

		void LoadUserPosition(Geocoordinate userCoordinate, bool center)
		{
			if (Control == null || Element == null) return;

			var userPosition = new BasicGeoposition
			{
				Latitude = userCoordinate.Point.Position.Latitude,
				Longitude = userCoordinate.Point.Position.Longitude
			};

			var point = new Geopoint(userPosition);

			if (_userPositionCircle == null)
			{
				_userPositionCircle = new Ellipse
				{
					Stroke = new SolidColorBrush(Colors.White),
					Fill = new SolidColorBrush(Colors.Blue),
					StrokeThickness = 2,
					Height = 20,
					Width = 20,
					Opacity = 50
				};
			}

			if (Control.Children.Contains(_userPositionCircle))
				Control.Children.Remove(_userPositionCircle);

			MapControl.SetLocation(_userPositionCircle, point);
			MapControl.SetNormalizedAnchorPoint(_userPositionCircle, new Windows.Foundation.Point(0.5, 0.5));

			Control.Children.Add(_userPositionCircle);

			if (center)
			{
				Control.Center = point;
				Control.ZoomLevel = 13;
			}
		}

		void UpdateMapType()
		{
			switch (Element.MapType)
			{
				case MapType.Street:
					Control.Style = MapStyle.Road;
					break;
				case MapType.Satellite:
					Control.Style = MapStyle.Aerial;
					break;
				case MapType.Hybrid:
					Control.Style = MapStyle.AerialWithRoads;
					break;
			}
		}

		void UpdateHasZoomEnabled()
		{
			Control.ZoomInteractionMode = Element.HasZoomEnabled
				? MapInteractionMode.GestureAndControl
				: MapInteractionMode.ControlOnly;
		}

		void UpdateHasScrollEnabled()
		{
			Control.PanInteractionMode = Element.HasScrollEnabled ? MapPanInteractionMode.Auto : MapPanInteractionMode.Disabled;
		}

		void OnMapTapped(MapControl sender, MapInputEventArgs args)
		{
			Element?.SendMapClicked(new Position(args.Location.Position.Latitude, args.Location.Position.Longitude));
		}
	}
}
