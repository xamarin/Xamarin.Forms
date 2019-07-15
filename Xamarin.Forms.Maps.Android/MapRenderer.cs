using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Java.Lang;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Math = System.Math;
using NativePolyline = Android.Gms.Maps.Model.Polyline;

namespace Xamarin.Forms.Maps.Android
{
	public class MapRenderer : ViewRenderer<Map, MapView>, GoogleMap.IOnCameraMoveListener, IOnMapReadyCallback
	{
		const string MoveMessageName = "MapMoveToRegion";

		static Bundle s_bundle;

		bool _disposed;

		bool _init = true;

		List<Marker> _markers;
		List<NativePolyline> _polylines;

		public MapRenderer(Context context) : base(context)
		{
			AutoPackage = false;
		}

		[Obsolete("This constructor is obsolete as of version 2.5. Please use MapRenderer(Context) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MapRenderer()
		{
			AutoPackage = false;
		}

		protected Map Map => Element;

		protected GoogleMap NativeMap;

		internal static Bundle Bundle
		{
			set { s_bundle = value; }
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			return new SizeRequest(new Size(Context.ToPixels(40), Context.ToPixels(40)));
		}

		protected override MapView CreateNativeControl()
		{
			return new MapView(Context);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			_disposed = true;

			if (disposing)
			{
				if (Element != null)
				{
					MessagingCenter.Unsubscribe<Map, MapSpan>(this, MoveMessageName);

					((ObservableCollection<Pin>)Element.Pins).CollectionChanged -= OnPinCollectionChanged;
					foreach (Pin pin in Element.Pins)
					{
						pin.PropertyChanged -= PinOnPropertyChanged;
					}

					((ObservableCollection<Polyline>)Element.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
					foreach (Polyline polyline in Element.Polylines)
					{
						polyline.PropertyChanged -= PolylineOnPropertyChanged;
					}
				}

				if (NativeMap != null)
				{
					NativeMap.MyLocationEnabled = false;
					NativeMap.SetOnCameraMoveListener(null);
					NativeMap.InfoWindowClick -= MapOnMarkerClick;
					NativeMap.MapClick -= OnMapClick;
					NativeMap.Dispose();
					NativeMap = null;
				}

				Control?.OnDestroy();
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			base.OnElementChanged(e);

			MapView oldMapView = Control;

			MapView mapView = CreateNativeControl();
			mapView.OnCreate(s_bundle);
			mapView.OnResume();
			SetNativeControl(mapView);

			if (e.OldElement != null)
			{
				Map oldMapModel = e.OldElement;

				((ObservableCollection<Pin>)oldMapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
				foreach (Pin pin in oldMapModel.Pins)
				{
					pin.PropertyChanged -= PinOnPropertyChanged;
				}

				((ObservableCollection<Polyline>)oldMapModel.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
				foreach (Polyline polyline in oldMapModel.Polylines)
				{
					polyline.PropertyChanged -= PolylineOnPropertyChanged;
				}

				MessagingCenter.Unsubscribe<Map, MapSpan>(this, MoveMessageName);

				if (NativeMap != null)
				{
					NativeMap.SetOnCameraMoveListener(null);
					NativeMap.InfoWindowClick -= MapOnMarkerClick;
					NativeMap.MapClick -= OnMapClick;
					NativeMap = null;
				}

				oldMapView.Dispose();
			}

			Control.GetMapAsync(this);

			MessagingCenter.Subscribe<Map, MapSpan>(this, MoveMessageName, OnMoveToRegionMessage, Map);

			((INotifyCollectionChanged)Map.Pins).CollectionChanged += OnPinCollectionChanged;
			((INotifyCollectionChanged)Map.Polylines).CollectionChanged += OnPolylineCollectionChanged;
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Map.MapTypeProperty.PropertyName)
			{
				SetMapType();
				return;
			}

			GoogleMap gmap = NativeMap;
			if (gmap == null)
			{
				return;
			}

			if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
			{
				gmap.MyLocationEnabled = gmap.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
			}
			else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
			{
				gmap.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
			}
			else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
			{
				gmap.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
				gmap.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			base.OnLayout(changed, l, t, r, b);

			if (_init)
			{
				if (NativeMap != null)
				{
					MoveToRegion(Element.LastMoveToRegion, false);
					OnPinCollectionChanged(Element.Pins, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					OnPolylineCollectionChanged(Element.Polylines, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					_init = false;
				}
			}
			else if (changed)
			{
				if (NativeMap != null)
				{
					UpdateVisibleRegion(NativeMap.CameraPosition.Target);
				}

				if(Element.MoveToLastRegionOnLayoutChange)
					MoveToRegion(Element.LastMoveToRegion, false);
			}
		}

		protected virtual void OnMapReady(GoogleMap map)
		{
			if (map == null)
			{
				return;
			}

			map.SetOnCameraMoveListener(this);
			map.InfoWindowClick += MapOnMarkerClick;
			map.MapClick += OnMapClick;

			map.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
			map.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
			map.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
			map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
			SetMapType();
		}

		protected virtual MarkerOptions CreateMarker(Pin pin)
		{
			var opts = new MarkerOptions();
			opts.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
			opts.SetTitle(pin.Label);
			opts.SetSnippet(pin.Address);

			return opts;
		}

		void AddPins(IList pins)
		{
			GoogleMap map = NativeMap;
			if (map == null)
			{
				return;
			}

			if (_markers == null)
			{
				_markers = new List<Marker>();
			}

			_markers.AddRange(pins.Cast<Pin>().Select(p =>
			{
				Pin pin = p;
				var opts = CreateMarker(pin);
				var marker = map.AddMarker(opts);

				pin.PropertyChanged += PinOnPropertyChanged;

				// associate pin with marker for later lookup in event handlers
				pin.MarkerId = marker.Id;
				return marker;
			}));
		}

		void PinOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Pin pin = (Pin)sender;
			Marker marker = GetMarkerForPin(pin);

			if (marker == null)
			{
				return;
			}

			if (e.PropertyName == Pin.LabelProperty.PropertyName)
			{
				marker.Title = pin.Label;
			}
			else if (e.PropertyName == Pin.AddressProperty.PropertyName)
			{
				marker.Snippet = pin.Address;
			}
			else if (e.PropertyName == Pin.PositionProperty.PropertyName)
			{
				marker.Position = new LatLng(pin.Position.Latitude, pin.Position.Longitude);
			}
		}

		protected Marker GetMarkerForPin(Pin pin)
		{
			return _markers?.Find(m => m.Id == (string)pin.MarkerId);
		}

		void MapOnMarkerClick(object sender, GoogleMap.InfoWindowClickEventArgs eventArgs)
		{
			// clicked marker
			var marker = eventArgs.Marker;

			// lookup pin
			Pin targetPin = null;
			for (var i = 0; i < Map.Pins.Count; i++)
			{
				Pin pin = Map.Pins[i];
				if ((string)pin.MarkerId != marker.Id)
				{
					continue;
				}

				targetPin = pin;
				break;
			}

			// only consider event handled if a handler is present.
			// Else allow default behavior of displaying an info window.
			targetPin?.SendTap();
		}

		void OnMapClick(object sender, GoogleMap.MapClickEventArgs e)
		{
			Map.SendMapClicked(new Position(e.Point.Latitude, e.Point.Longitude));
		}

		void MoveToRegion(MapSpan span, bool animate)
		{
			GoogleMap map = NativeMap;
			if (map == null)
			{
				return;
			}

			span = span.ClampLatitude(85, -85);
			var ne = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2,
				span.Center.Longitude + span.LongitudeDegrees / 2);
			var sw = new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2,
				span.Center.Longitude - span.LongitudeDegrees / 2);
			CameraUpdate update = CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), 0);

			try
			{
				if (animate)
				{
					map.AnimateCamera(update);
				}
				else
				{
					map.MoveCamera(update);
				}
			}
			catch (IllegalStateException exc)
			{
				System.Diagnostics.Debug.WriteLine("MoveToRegion exception: " + exc);
				Log.Warning("Xamarin.Forms MapRenderer", $"MoveToRegion exception: {exc}");
			}
		}

		void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			switch (notifyCollectionChangedEventArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddPins(notifyCollectionChangedEventArgs.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					RemovePins(notifyCollectionChangedEventArgs.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					RemovePins(notifyCollectionChangedEventArgs.OldItems);
					AddPins(notifyCollectionChangedEventArgs.NewItems);
					break;
				case NotifyCollectionChangedAction.Reset:
					_markers?.ForEach(m => m.Remove());
					_markers = null;
					AddPins((IList)Element.Pins);
					break;
				case NotifyCollectionChangedAction.Move:
					//do nothing
					break;
			}
		}

		void OnMoveToRegionMessage(Map s, MapSpan a)
		{
			MoveToRegion(a, true);
		}

		void RemovePins(IList pins)
		{
			GoogleMap map = NativeMap;
			if (map == null)
			{
				return;
			}
			if (_markers == null)
			{
				return;
			}

			foreach (Pin p in pins)
			{
				p.PropertyChanged -= PinOnPropertyChanged;
				var marker = GetMarkerForPin(p);

				if (marker == null)
				{
					continue;
				}
				marker.Remove();
				_markers.Remove(marker);
			}
		}

		void SetMapType()
		{
			GoogleMap map = NativeMap;
			if (map == null)
			{
				return;
			}

			switch (Map.MapType)
			{
				case MapType.Street:
					map.MapType = GoogleMap.MapTypeNormal;
					break;
				case MapType.Satellite:
					map.MapType = GoogleMap.MapTypeSatellite;
					break;
				case MapType.Hybrid:
					map.MapType = GoogleMap.MapTypeHybrid;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		void UpdateVisibleRegion(LatLng pos)
		{
			GoogleMap map = NativeMap;
			if (map == null)
			{
				return;
			}
			Projection projection = map.Projection;
			int width = Control.Width;
			int height = Control.Height;
			LatLng ul = projection.FromScreenLocation(new global::Android.Graphics.Point(0, 0));
			LatLng ur = projection.FromScreenLocation(new global::Android.Graphics.Point(width, 0));
			LatLng ll = projection.FromScreenLocation(new global::Android.Graphics.Point(0, height));
			LatLng lr = projection.FromScreenLocation(new global::Android.Graphics.Point(width, height));
			double dlat = Math.Max(Math.Abs(ul.Latitude - lr.Latitude), Math.Abs(ur.Latitude - ll.Latitude));
			double dlong = Math.Max(Math.Abs(ul.Longitude - lr.Longitude), Math.Abs(ur.Longitude - ll.Longitude));
			Element.SetVisibleRegion(new MapSpan(new Position(pos.Latitude, pos.Longitude), dlat, dlong));
		}

		protected NativePolyline GetNativePolyline(Polyline polyline)
		{
			return _polylines?.Find(p => p.Id == (string)polyline.PolylineId);
		}

		protected Polyline GetPolylineFromNative(NativePolyline polyline)
		{
			Polyline targetPolyline = null;

			for (int i = 0; i < Map.Polylines.Count; i++)
			{
				var formsPolyline = Map.Polylines[i];
				if ((string)formsPolyline.PolylineId == polyline.Id)
				{
					targetPolyline = formsPolyline;
					break;
				}
			}

			return targetPolyline;
		}

		protected virtual PolylineOptions CreateNativePolyline(Polyline polyline)
		{
			var opts = new PolylineOptions();

			opts.InvokeColor(polyline.StrokeColor.ToAndroid(Color.Black));
			opts.InvokeWidth(polyline.StrokeWidth);

			foreach (var position in polyline.Geopath)
			{
				opts.Points.Add(new LatLng(position.Latitude, position.Longitude));
			}

			return opts;
		}

		void PolylineOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var formsPolyline = (Polyline)sender;
			var nativePolyline = GetNativePolyline(formsPolyline);

			if (nativePolyline == null)
			{
				return;
			}

			if (e.PropertyName == Polyline.StrokeColorProperty.PropertyName)
			{
				nativePolyline.Color = formsPolyline.StrokeColor.ToAndroid(Color.Black);
			}
			else if (e.PropertyName == Polyline.StrokeWidthProperty.PropertyName)
			{
				nativePolyline.Width = formsPolyline.StrokeWidth;
			}
			else if (e.PropertyName == nameof(Polyline.Geopath))
			{
				nativePolyline.Points = formsPolyline.Geopath.Select(position => new LatLng(position.Latitude, position.Longitude)).ToList();
			}
		}

		void OnPolylineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddPolylines(e.NewItems.Cast<Polyline>());
					break;
				case NotifyCollectionChangedAction.Remove:
					RemovePolylines(e.OldItems.Cast<Polyline>());
					break;
				case NotifyCollectionChangedAction.Replace:
					RemovePolylines(e.OldItems.Cast<Polyline>());
					AddPolylines(e.NewItems.Cast<Polyline>());
					break;
				case NotifyCollectionChangedAction.Reset:
					_polylines?.ForEach(p => p.Remove());
					_polylines = null;
					AddPolylines(Map.Polylines);
					break;
			}
		}

		void AddPolylines(IEnumerable<Polyline> polylines)
		{
			var map = NativeMap;
			if (map == null)
			{
				return;
			}

			if (_polylines == null)
			{
				_polylines = new List<NativePolyline>();
			}

			_polylines.AddRange(polylines.Select(p =>
			{
				p.PropertyChanged += PolylineOnPropertyChanged;

				var options = CreateNativePolyline(p);
				var nativePolyline = map.AddPolyline(options);

				p.PolylineId = nativePolyline.Id;

				return nativePolyline;
			}));
		}

		void RemovePolylines(IEnumerable<Polyline> polylines)
		{
			var map = NativeMap;

			if (map == null)
			{
				return;
			}

			if (_markers == null)
			{
				return;
			}

			foreach (var polyline in polylines)
			{
				polyline.PropertyChanged -= PolylineOnPropertyChanged;

				var native = GetNativePolyline(polyline);

				if (native != null)
				{
					native.Remove();
					_polylines.Remove(native);
				}
			}
		}

		void IOnMapReadyCallback.OnMapReady(GoogleMap map)
		{
			NativeMap = map;
			OnMapReady(map);
		}

		void GoogleMap.IOnCameraMoveListener.OnCameraMove()
		{
			UpdateVisibleRegion(NativeMap.CameraPosition.Target);
		}
	}
}
