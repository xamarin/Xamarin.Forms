
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;
   using System.Globalization;
   using GMap.NET.Internals;
   using System.Collections.Generic;
   using System.Xml;
   using System.Diagnostics;

   public abstract class CloudMadeMapProviderBase : GMapProvider, RoutingProvider, DirectionsProvider
   {
      public readonly string ServerLetters = "abc";
      public readonly string DoubleResolutionString = "@2x";

      public bool DoubleResolution = true;
      public string Key;
      public int StyleID;

      public string Version = "0.3";

      public CloudMadeMapProviderBase()
      {
         MaxZoom = null;
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }
      #endregion

      #region RoutingProvider Members

      public MapRoute GetRoute(PointLatLng start, PointLatLng end, bool avoidHighways, bool walkingMode, int Zoom)
      {
         List<PointLatLng> points = GetRoutePoints(MakeRoutingUrl(start, end, walkingMode ? TravelTypeFoot : TravelTypeMotorCar, LanguageStr, "km"));
         MapRoute route = points != null ? new MapRoute(points, walkingMode ? WalkingStr : DrivingStr) : null;
         return route;
      }

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <param name="avoidHighways"></param>
      /// <param name="walkingMode"></param>
      /// <param name="Zoom"></param>
      /// <returns></returns>
      public MapRoute GetRoute(string start, string end, bool avoidHighways, bool walkingMode, int Zoom)
      {
         throw new NotImplementedException();
      }

      #region -- internals --

      string MakeRoutingUrl(PointLatLng start, PointLatLng end, string travelType, string language, string units)
      {
         // http://developers.cloudmade.com/projects/routing-http-api/examples/
         // http://routes.cloudmade.com/YOUR-API-KEY-GOES-HERE/api/0.3/start_point,[[transit_point1,...,transit_pointN]],end_point/route_type[/route_type_modifier].output_format[?lang=(en|de)][&units=(km|miles)]
         return string.Format(CultureInfo.InvariantCulture, UrlFormat, Key, Version, start.Lat, start.Lng, end.Lat, end.Lng, travelType, language, units);
      }

      List<PointLatLng> GetRoutePoints(string url)
      {
         List<PointLatLng> points = null;
         try
         {
            string route = GMaps.Instance.UseRouteCache ? Cache.Instance.GetContent(url, CacheType.RouteCache) : string.Empty;
            if(string.IsNullOrEmpty(route))
            {
               route = GetContentUsingHttp(url);
               if(!string.IsNullOrEmpty(route))
               {
                  if(GMaps.Instance.UseRouteCache)
                  {
                     Cache.Instance.SaveContent(url, CacheType.RouteCache, route);
                  }
               }
            }

            #region -- gpx response --
            //<?xml version="1.0" encoding="UTF-8"?>
            //<gpx creator="" version="1.1" xmlns="http://www.topografix.com/GPX/1/1"
            //    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //    xsi:schemaLocation="http://www.topografix.com/GPX/1/1 gpx.xsd ">
            //    <extensions>
            //        <distance>293</distance>
            //        <time>34</time>
            //        <start>Perckhoevelaan</start>
            //        <end>Goudenregenlaan</end>
            //    </extensions>
            //    <wpt lat="51.17702" lon="4.39630" />
            //    <wpt lat="51.17656" lon="4.39655" />
            //    <wpt lat="51.17639" lon="4.39670" />
            //    <wpt lat="51.17612" lon="4.39696" />
            //    <wpt lat="51.17640" lon="4.39767" />
            //    <wpt lat="51.17668" lon="4.39828" />
            //    <wpt lat="51.17628" lon="4.39874" />
            //    <wpt lat="51.17618" lon="4.39888" />
            //    <rte>
            //        <rtept lat="51.17702" lon="4.39630">
            //            <desc>Head south on Perckhoevelaan, 0.1 km</desc>
            //            <extensions>
            //                <distance>111</distance>
            //                <time>13</time>
            //                <offset>0</offset>
            //                <distance-text>0.1 km</distance-text>
            //                <direction>S</direction>
            //                <azimuth>160.6</azimuth>
            //            </extensions>
            //        </rtept>
            //        <rtept lat="51.17612" lon="4.39696">
            //            <desc>Turn left at Laarstraat, 0.1 km</desc>
            //            <extensions>
            //                <distance>112</distance>
            //                <time>13</time>
            //                <offset>3</offset>
            //                <distance-text>0.1 km</distance-text>
            //                <direction>NE</direction>
            //                <azimuth>58.1</azimuth>
            //                <turn>TL</turn>
            //                <turn-angle>269.0</turn-angle>
            //            </extensions>
            //        </rtept>
            //        <rtept lat="51.17668" lon="4.39828">
            //            <desc>Turn right at Goudenregenlaan, 70 m</desc>
            //            <extensions>
            //                <distance>70</distance>
            //                <time>8</time>
            //                <offset>5</offset>
            //                <distance-text>70 m</distance-text>
            //                <direction>SE</direction>
            //                <azimuth>143.4</azimuth>
            //                <turn>TR</turn>
            //                <turn-angle>89.8</turn-angle>
            //            </extensions>
            //        </rtept>
            //    </rte>
            //</gpx> 
            #endregion

            if(!string.IsNullOrEmpty(route))
            {
               XmlDocument xmldoc = new XmlDocument();
               xmldoc.LoadXml(route);
               System.Xml.XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
               xmlnsManager.AddNamespace("sm", "http://www.topografix.com/GPX/1/1");

               XmlNodeList wpts = xmldoc.SelectNodes("/sm:gpx/sm:wpt", xmlnsManager);
               if(wpts != null && wpts.Count > 0)
               {
                  points = new List<PointLatLng>();
                  foreach(XmlNode w in wpts)
                  {
                     double lat = double.Parse(w.Attributes["lat"].InnerText, CultureInfo.InvariantCulture);
                     double lng = double.Parse(w.Attributes["lon"].InnerText, CultureInfo.InvariantCulture);
                     points.Add(new PointLatLng(lat, lng));
                  }
               }
            }
         }
         catch(Exception ex)
         {
            Debug.WriteLine("GetRoutePoints: " + ex);
         }

         return points;
      }

      static readonly string UrlFormat = "http://routes.cloudmade.com/{0}/api/{1}/{2},{3},{4},{5}/{6}.gpx?lang={7}&units={8}";
      static readonly string TravelTypeFoot = "foot";
      static readonly string TravelTypeMotorCar = "car";
      static readonly string WalkingStr = "Walking";
      static readonly string DrivingStr = "Driving";

      #endregion

      #endregion

      #region DirectionsProvider Members

      public DirectionsStatusCode GetDirections(out GDirections direction, PointLatLng start, PointLatLng end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
      {
         return GetDirectionsUrl(MakeRoutingUrl(start, end, walkingMode ? TravelTypeFoot : TravelTypeMotorCar, LanguageStr, metric ? "km" : "miles"), out direction);
      }

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="direction"></param>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <param name="avoidHighways"></param>
      /// <param name="avoidTolls"></param>
      /// <param name="walkingMode"></param>
      /// <param name="sensor"></param>
      /// <param name="metric"></param>
      /// <returns></returns>
      public DirectionsStatusCode GetDirections(out GDirections direction, string start, string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="status"></param>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <param name="avoidHighways"></param>
      /// <param name="avoidTolls"></param>
      /// <param name="walkingMode"></param>
      /// <param name="sensor"></param>
      /// <param name="metric"></param>
      /// <returns></returns>
      public IEnumerable<GDirections> GetDirections(out DirectionsStatusCode status, string start, string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="status"></param>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <param name="avoidHighways"></param>
      /// <param name="avoidTolls"></param>
      /// <param name="walkingMode"></param>
      /// <param name="sensor"></param>
      /// <param name="metric"></param>
      /// <returns></returns>
      public IEnumerable<GDirections> GetDirections(out DirectionsStatusCode status, PointLatLng start, PointLatLng end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="direction"></param>
      /// <param name="start"></param>
      /// <param name="wayPoints"></param>
      /// <param name="avoidHighways"></param>
      /// <param name="avoidTolls"></param>
      /// <param name="walkingMode"></param>
      /// <param name="sensor"></param>
      /// <param name="metric"></param>
      /// <returns></returns>
      public DirectionsStatusCode GetDirections(out GDirections direction, PointLatLng start, IEnumerable<PointLatLng> wayPoints, PointLatLng end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
      {
          throw new NotImplementedException();
      }

      /// <summary>
      /// NotImplemented
      /// </summary>
      /// <param name="direction"></param>
      /// <param name="start"></param>
      /// <param name="wayPoints"></param>
      /// <param name="avoidHighways"></param>
      /// <param name="avoidTolls"></param>
      /// <param name="walkingMode"></param>
      /// <param name="sensor"></param>
      /// <param name="metric"></param>
      /// <returns></returns>
      public DirectionsStatusCode GetDirections(out GDirections direction, string start, IEnumerable<string> wayPoints, string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric)
      {
          throw new NotImplementedException();
      }

      #region -- internals --

      DirectionsStatusCode GetDirectionsUrl(string url, out GDirections direction)
      {
         DirectionsStatusCode ret = DirectionsStatusCode.UNKNOWN_ERROR;
         direction = null;

         try
         {
            string route = GMaps.Instance.UseRouteCache ? Cache.Instance.GetContent(url, CacheType.DirectionsCache) : string.Empty;
            if(string.IsNullOrEmpty(route))
            {
               route = GetContentUsingHttp(url);
               if(!string.IsNullOrEmpty(route))
               {
                  if(GMaps.Instance.UseRouteCache)
                  {
                     Cache.Instance.SaveContent(url, CacheType.DirectionsCache, route);
                  }
               }
            }

            #region -- gpx response --
            //<?xml version="1.0" encoding="UTF-8"?>
            //<gpx creator="" version="1.1" xmlns="http://www.topografix.com/GPX/1/1"
            //    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
            //    xsi:schemaLocation="http://www.topografix.com/GPX/1/1 gpx.xsd ">
            //    <extensions>
            //        <distance>293</distance>
            //        <time>34</time>
            //        <start>Perckhoevelaan</start>
            //        <end>Goudenregenlaan</end>
            //    </extensions>
            //    <wpt lat="51.17702" lon="4.39630" />
            //    <wpt lat="51.17656" lon="4.39655" />
            //    <wpt lat="51.17639" lon="4.39670" />
            //    <wpt lat="51.17612" lon="4.39696" />
            //    <wpt lat="51.17640" lon="4.39767" />
            //    <wpt lat="51.17668" lon="4.39828" />
            //    <wpt lat="51.17628" lon="4.39874" />
            //    <wpt lat="51.17618" lon="4.39888" />
            //    <rte>
            //        <rtept lat="51.17702" lon="4.39630">
            //            <desc>Head south on Perckhoevelaan, 0.1 km</desc>
            //            <extensions>
            //                <distance>111</distance>
            //                <time>13</time>
            //                <offset>0</offset>
            //                <distance-text>0.1 km</distance-text>
            //                <direction>S</direction>
            //                <azimuth>160.6</azimuth>
            //            </extensions>
            //        </rtept>
            //        <rtept lat="51.17612" lon="4.39696">
            //            <desc>Turn left at Laarstraat, 0.1 km</desc>
            //            <extensions>
            //                <distance>112</distance>
            //                <time>13</time>
            //                <offset>3</offset>
            //                <distance-text>0.1 km</distance-text>
            //                <direction>NE</direction>
            //                <azimuth>58.1</azimuth>
            //                <turn>TL</turn>
            //                <turn-angle>269.0</turn-angle>
            //            </extensions>
            //        </rtept>
            //        <rtept lat="51.17668" lon="4.39828">
            //            <desc>Turn right at Goudenregenlaan, 70 m</desc>
            //            <extensions>
            //                <distance>70</distance>
            //                <time>8</time>
            //                <offset>5</offset>
            //                <distance-text>70 m</distance-text>
            //                <direction>SE</direction>
            //                <azimuth>143.4</azimuth>
            //                <turn>TR</turn>
            //                <turn-angle>89.8</turn-angle>
            //            </extensions>
            //        </rtept>
            //    </rte>
            //</gpx> 
            #endregion

            if(!string.IsNullOrEmpty(route))
            {
               XmlDocument xmldoc = new XmlDocument();
               xmldoc.LoadXml(route);
               System.Xml.XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
               xmlnsManager.AddNamespace("sm", "http://www.topografix.com/GPX/1/1");

               XmlNodeList wpts = xmldoc.SelectNodes("/sm:gpx/sm:wpt", xmlnsManager);
               if(wpts != null && wpts.Count > 0)
               {
                  ret = DirectionsStatusCode.OK;

                  direction = new GDirections();
                  direction.Route = new List<PointLatLng>();

                  foreach(XmlNode w in wpts)
                  {
                     double lat = double.Parse(w.Attributes["lat"].InnerText, CultureInfo.InvariantCulture);
                     double lng = double.Parse(w.Attributes["lon"].InnerText, CultureInfo.InvariantCulture);
                     direction.Route.Add(new PointLatLng(lat, lng));
                  }

                  if(direction.Route.Count > 0)
                  {
                     direction.StartLocation = direction.Route[0];
                     direction.EndLocation = direction.Route[direction.Route.Count - 1];
                  }

                  XmlNode n = xmldoc.SelectSingleNode("/sm:gpx/sm:metadata/sm:copyright/sm:license", xmlnsManager);
                  if(n != null)
                  {
                     direction.Copyrights = n.InnerText;
                  }

                  n = xmldoc.SelectSingleNode("/sm:gpx/sm:extensions/sm:distance", xmlnsManager);
                  if(n != null)
                  {
                     direction.Distance = n.InnerText + "m";
                  }

                  n = xmldoc.SelectSingleNode("/sm:gpx/sm:extensions/sm:time", xmlnsManager);
                  if(n != null)
                  {
                     direction.Duration = n.InnerText + "s";
                  }

                  n = xmldoc.SelectSingleNode("/sm:gpx/sm:extensions/sm:start", xmlnsManager);
                  if(n != null)
                  {
                     direction.StartAddress = n.InnerText;
                  }

                  n = xmldoc.SelectSingleNode("/sm:gpx/sm:extensions/sm:end", xmlnsManager);
                  if(n != null)
                  {
                     direction.EndAddress = n.InnerText;
                  }

                  wpts = xmldoc.SelectNodes("/sm:gpx/sm:rte/sm:rtept", xmlnsManager);
                  if(wpts != null && wpts.Count > 0)
                  {
                     direction.Steps = new List<GDirectionStep>();

                     foreach(XmlNode w in wpts)
                     {
                        GDirectionStep step = new GDirectionStep();

                        double lat = double.Parse(w.Attributes["lat"].InnerText, CultureInfo.InvariantCulture);
                        double lng = double.Parse(w.Attributes["lon"].InnerText, CultureInfo.InvariantCulture);

                        step.StartLocation = new PointLatLng(lat, lng);

                        XmlNode nn = w.SelectSingleNode("sm:desc", xmlnsManager);
                        if(nn != null)
                        {
                           step.HtmlInstructions = nn.InnerText;
                        }

                        nn = w.SelectSingleNode("sm:extensions/sm:distance-text", xmlnsManager);
                        if(nn != null)
                        {
                           step.Distance = nn.InnerText;
                        }

                        nn = w.SelectSingleNode("sm:extensions/sm:time", xmlnsManager);
                        if(nn != null)
                        {
                           step.Duration = nn.InnerText + "s";
                        }

                        direction.Steps.Add(step);
                     }
                  }
               }
            }
         }
         catch(Exception ex)
         {
            ret = DirectionsStatusCode.ExceptionInCode;
            direction = null;
            Debug.WriteLine("GetDirectionsUrl: " + ex);
         }

         return ret;
      }

      #endregion

      #endregion      
   }

   /// <summary>
   /// CloudMadeMap demo provider, http://maps.cloudmade.com/
   /// </summary>
   public class CloudMadeMapProvider : CloudMadeMapProviderBase
   {
      public static readonly CloudMadeMapProvider Instance;

      CloudMadeMapProvider()
      {
         Key = "5937c2bd907f4f4a92d8980a7c666ac0"; // demo key of CloudMade
         StyleID = 45363; // grab your style here http://maps.cloudmade.com/?styleId=45363
      }

      static CloudMadeMapProvider()
      {
         Instance = new CloudMadeMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("00403A36-725F-4BC4-934F-BFC1C164D003");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CloudMade, Demo";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         return string.Format(UrlFormat, ServerLetters[GetServerNum(pos, 3)], Key, StyleID, (DoubleResolution ? DoubleResolutionString : string.Empty), zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.tile.cloudmade.com/{1}/{2}{3}/256/{4}/{5}/{6}.png";
   }
}