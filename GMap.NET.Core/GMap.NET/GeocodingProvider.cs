
namespace GMap.NET
{
   using System.Collections.Generic;

   /// <summary>
   /// geocoding interface
   /// </summary>
   public interface GeocodingProvider
   {
      GeoCoderStatusCode GetPoints(string keywords, out List<PointLatLng> pointList);

      PointLatLng? GetPoint(string keywords, out GeoCoderStatusCode status);

      GeoCoderStatusCode GetPoints(Placemark placemark, out List<PointLatLng> pointList);

      PointLatLng? GetPoint(Placemark placemark, out GeoCoderStatusCode status);

      // ...

      GeoCoderStatusCode GetPlacemarks(PointLatLng location, out List<Placemark> placemarkList);

      Placemark ? GetPlacemark(PointLatLng location, out GeoCoderStatusCode status);
   }
}
