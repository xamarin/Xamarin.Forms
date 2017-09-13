
namespace GMap.NET
{
   /// <summary>
   /// GeoCoder StatusCode
   /// </summary>
   public enum GeoCoderStatusCode : int
   {
      /// <summary>
      /// unknow response
      /// </summary>
      Unknow = -1,

      /// <summary>
      /// No errors occurred; the address was successfully parsed and its geocode has been returned.
      /// </summary>
      G_GEO_SUCCESS = 200,

      /// <summary>
      /// A directions request could not be successfully parsed.
      /// For example, the request may have been rejected if it contained more than the maximum number of waypoints allowed.
      /// </summary>  
      G_GEO_BAD_REQUEST = 400,

      /// <summary>
      /// A geocoding or directions request could not be successfully processed, yet the exact reason for the failure is not known.
      /// </summary>
      G_GEO_SERVER_ERROR = 500,

      /// <summary>
      /// The HTTP q parameter was either missing or had no value.
      /// For geocoding requests, this means that an empty address was specified as input. For directions requests, this means that no query was specified in the input.
      /// </summary>
      G_GEO_MISSING_QUERY = 601,

      /// <summary>
      /// Synonym for G_GEO_MISSING_QUERY.
      /// </summary>
      G_GEO_MISSING_ADDRESS = 601,

      /// <summary>
      ///  No corresponding geographic location could be found for the specified address.
      ///  This may be due to the fact that the address is relatively new, or it may be incorrect.
      /// </summary>
      G_GEO_UNKNOWN_ADDRESS = 602,

      /// <summary>
      /// The geocode for the given address or the route for the given directions query cannot be returned due to legal or contractual reasons.
      /// </summary>
      G_GEO_UNAVAILABLE_ADDRESS = 603,

      /// <summary>
      /// The GDirections object could not compute directions between the points mentioned in the query.
      /// This is usually because there is no route available between the two points, or because we do not have data for routing in that region.
      /// </summary>
      G_GEO_UNKNOWN_DIRECTIONS = 604,

      /// <summary>
      /// The given key is either invalid or does not match the domain for which it was given.
      /// </summary>
      G_GEO_BAD_KEY = 610,

      /// <summary>
      /// The given key has gone over the requests limit in the 24 hour period or has submitted too many requests in too short a period of time.
      /// If you're sending multiple requests in parallel or in a tight loop, use a timer or pause in your code to make sure you don't send the requests too quickly.
      /// </summary>
      G_GEO_TOO_MANY_QUERIES = 620,

      /// <summary>
      /// indicates that exception occured during execution
      /// </summary>
      ExceptionInCode,
   }

   /// <summary>
   /// Direction StatusCode
   /// </summary>
   public enum DirectionsStatusCode : int
   {
      /// <summary>
      /// indicates the response contains a valid result.
      /// </summary>
      OK = 0,

      /// <summary>
      /// indicates at least one of the locations specified in the requests's origin, destination, or waypoints could not be geocoded.
      /// </summary>
      NOT_FOUND,

      /// <summary>
      /// indicates no route could be found between the origin and destination.
      /// </summary>
      ZERO_RESULTS,

      /// <summary>
      ///  indicates that too many waypointss were provided in the request The maximum allowed waypoints is 8, plus the origin, and destination.
      /// </summary>
      MAX_WAYPOINTS_EXCEEDED,

      /// <summary>
      /// indicates that the provided request was invalid.
      /// </summary>
      INVALID_REQUEST,

      /// <summary>
      /// indicates the service has received too many requests from your application within the allowed time period.
      /// </summary>
      OVER_QUERY_LIMIT,

      /// <summary>
      /// indicates that the service denied use of the directions service by your application.
      /// </summary>
      REQUEST_DENIED,

      /// <summary>
      /// indicates a directions request could not be processed due to a server error. The request may succeed if you try again.
      /// </summary>
      UNKNOWN_ERROR,

      /// <summary>
      /// indicates that exception occured during execution
      /// </summary>
      ExceptionInCode,
   }
}
