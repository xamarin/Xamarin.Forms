using System.Collections.Concurrent;
using MapKit;

#if __MOBILE__
namespace Microsoft.Maui.Controls.Compatibility.Maps.iOS
#else
namespace Microsoft.Maui.Controls.Compatibility.Maps.MacOS
#endif
{
	// A static pool of MKMapView instances we can reuse 
	internal class MapPool
	{
		static MapPool s_instance;
		public static MapPool Instance => s_instance ?? (s_instance = new MapPool());

		internal readonly ConcurrentQueue<MKMapView> Maps = new ConcurrentQueue<MKMapView>();

		public static void Add(MKMapView mapView)
		{
			Instance.Maps.Enqueue(mapView);
		}

		public static MKMapView Get()
		{
			MKMapView mapView;
			return Instance.Maps.TryDequeue(out mapView) ? mapView : null;
		}
	}
}