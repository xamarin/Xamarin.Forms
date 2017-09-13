
namespace GMap.NET
{
   using System.Collections.Generic;

   public class GDirections
   {
      /// <summary>
      /// contains a short textual description for the route, suitable for naming and disambiguating the route from alternatives.
      /// </summary>
      public string Summary;

      /// <summary>
      /// contains a human-readable representation of the duration.
      /// </summary>
      public string Duration;

      /// <summary>
      /// contains a value of the duration.
      /// </summary>
      public uint DurationValue;

      /// <summary>
      /// contains a human-readable representation of the distance, displayed in units as used at the origin
      /// (or as overridden within the units parameter in the request), in the language specified in the request.
      /// (For example, miles and feet will be used for any origin within the United States.)
      /// </summary>
      public string Distance;

      /// <summary>
      /// contains a value of the distance.
      /// </summary>
      public uint DistanceValue;

      /// <summary>
      /// contains the latitude/longitude coordinates of the origin of this leg. Because the Directions API
      /// calculates directions between locations by using the nearest transportation option (usually a road)
      /// at the start and end points, start_location may be different than the provided origin of this leg if,
      /// for example, a road is not near the origin.
      /// </summary>
      public PointLatLng StartLocation;

      /// <summary>
      /// contains the latitude/longitude coordinates of the given destination of this leg. Because the Directions
      /// API calculates directions between locations by using the nearest transportation option (usually a road)
      /// at the start and end points, end_location may be different than the provided destination of this leg if,
      /// for example, a road is not near the destination.
      /// </summary>
      public PointLatLng EndLocation;

      /// <summary>
      /// contains the human-readable address (typically a street address) reflecting the start_location of this leg.
      /// </summary>
      public string StartAddress;

      /// <summary>
      /// contains the human-readable address (typically a street address) reflecting the end_location of this leg.
      /// </summary>
      public string EndAddress;

      /// <summary>
      /// contains the copyrights text to be displayed for this route. You must handle and display this information yourself.
      /// </summary>
      public string Copyrights;

      /// <summary>
      /// contains an array of steps denoting information about each separate step of the leg of the journey.
      /// </summary>
      public List<GDirectionStep> Steps;

      /// <summary>
      /// contains all points of the route
      /// </summary>
      public List<PointLatLng> Route;

      public override string ToString()
      {
         return Summary + " | " + Distance + " | " + Duration;
      }
   }

   public struct GDirectionStep
   {
      public string TravelMode;

      /// <summary>
      /// contains the location of the starting point of this step, as a single set of lat and lng fields.
      /// </summary>
      public PointLatLng StartLocation;

      /// <summary>
      /// contains the location of the ending point of this step, as a single set of lat and lng fields.
      /// </summary>
      public PointLatLng EndLocation;

      /// <summary>
      ///  contains the typical time required to perform the step, until the next step. This field may be undefined if the duration is unknown.
      /// </summary>
      public string Duration;

      /// <summary>
      /// contains the distance covered by this step until the next step. This field may be undefined if the distance is unknown.
      /// </summary>
      public string Distance;

      /// <summary>
      /// contains formatted instructions for this step, presented as an HTML text string.
      /// </summary>
      public string HtmlInstructions;

      /// <summary>
      /// points of the step
      /// </summary>
      public List<PointLatLng> Points;

      public override string ToString()
      {
         return TravelMode + " | " + Distance + " | " + Duration + " | " + HtmlInstructions;
      }
   }
}
