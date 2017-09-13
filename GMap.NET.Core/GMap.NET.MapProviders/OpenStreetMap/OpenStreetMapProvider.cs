
namespace GMap.NET.MapProviders
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Globalization;
   using System.Xml;
   using GMap.NET.Internals;
   using GMap.NET.Projections;

   public abstract class OpenStreetMapProviderBase : GMapProvider, RoutingProvider, GeocodingProvider
   {
      public OpenStreetMapProviderBase()
      {
         MaxZoom = null;
         RefererUrl = "http://www.openstreetmap.org/";
         Copyright = string.Format("© OpenStreetMap - Map data ©{0} OpenStreetMap", DateTime.Today.Year);
      }

      public readonly string ServerLetters = "abc";
      public int MinExpectedRank = 0;

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

      public override GMapProvider[] Overlays
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region GMapRoutingProvider Members

      public MapRoute GetRoute(PointLatLng start, PointLatLng end, bool avoidHighways, bool walkingMode, int Zoom)
      {
         List<PointLatLng> points = GetRoutePoints(MakeRoutingUrl(start, end, walkingMode ? TravelTypeFoot : TravelTypeMotorCar));
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
         throw new NotImplementedException("use GetRoute(PointLatLng start, PointLatLng end...");
      }

      #region -- internals --
      string MakeRoutingUrl(PointLatLng start, PointLatLng end, string travelType)
      {
         return string.Format(CultureInfo.InvariantCulture, RoutingUrlFormat, start.Lat, start.Lng, end.Lat, end.Lng, travelType);
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

            if(!string.IsNullOrEmpty(route))
            {
               XmlDocument xmldoc = new XmlDocument();
               xmldoc.LoadXml(route);
               System.Xml.XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmldoc.NameTable);
               xmlnsManager.AddNamespace("sm", "http://earth.google.com/kml/2.0");

               ///Folder/Placemark/LineString/coordinates
               var coordNode = xmldoc.SelectSingleNode("/sm:kml/sm:Document/sm:Folder/sm:Placemark/sm:LineString/sm:coordinates", xmlnsManager);

               string[] coordinates = coordNode.InnerText.Split('\n');

               if(coordinates.Length > 0)
               {
                  points = new List<PointLatLng>();

                  foreach(string coordinate in coordinates)
                  {
                     if(coordinate != string.Empty)
                     {
                        string[] XY = coordinate.Split(',');
                        if(XY.Length == 2)
                        {
                           double lat = double.Parse(XY[1], CultureInfo.InvariantCulture);
                           double lng = double.Parse(XY[0], CultureInfo.InvariantCulture);
                           points.Add(new PointLatLng(lat, lng));
                        }
                     }
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

      static readonly string RoutingUrlFormat = "http://www.yournavigation.org/api/1.0/gosmore.php?format=kml&flat={0}&flon={1}&tlat={2}&tlon={3}&v={4}&fast=1&layer=mapnik";
      static readonly string TravelTypeFoot = "foot";
      static readonly string TravelTypeMotorCar = "motorcar";

      static readonly string WalkingStr = "Walking";
      static readonly string DrivingStr = "Driving";
      #endregion

      #endregion

      #region GeocodingProvider Members

      public GeoCoderStatusCode GetPoints(string keywords, out List<PointLatLng> pointList)
      {
         // http://nominatim.openstreetmap.org/search?q=lithuania,vilnius&format=xml

         #region -- response --
         //<searchresults timestamp="Wed, 01 Feb 12 09:46:00 -0500" attribution="Data Copyright OpenStreetMap Contributors, Some Rights Reserved. CC-BY-SA 2.0." querystring="lithuania,vilnius" polygon="false" exclude_place_ids="29446018,53849547,8831058,29614806" more_url="http://open.mapquestapi.com/nominatim/v1/search?format=xml&exclude_place_ids=29446018,53849547,8831058,29614806&accept-language=en&q=lithuania%2Cvilnius">
         //<place place_id="29446018" osm_type="way" osm_id="24598347" place_rank="30" boundingbox="54.6868133544922,54.6879043579102,25.2885360717773,25.2898139953613" lat="54.6873633486028" lon="25.289199818878" display_name="National Museum of Lithuania, 1, Arsenalo g., Senamiesčio seniūnija, YAHOO-HIRES-20080313, Vilnius County, Šalčininkų rajonas, Vilniaus apskritis, 01513, Lithuania" class="tourism" type="museum" icon="http://open.mapquestapi.com/nominatim/v1/images/mapicons/tourist_museum.p.20.png"/>
         //<place place_id="53849547" osm_type="way" osm_id="55469274" place_rank="30" boundingbox="54.6896553039551,54.690486907959,25.2675743103027,25.2692089080811" lat="54.6900227236882" lon="25.2683589759401" display_name="Ministry of Foreign Affairs of the Republic of Lithuania, 2, J. Tumo Vaižganto g., Naujamiesčio seniūnija, Vilnius, Vilnius County, Vilniaus m. savivaldybė, Vilniaus apskritis, LT-01104, Lithuania" class="amenity" type="public_building"/>
         //<place place_id="8831058" osm_type="node" osm_id="836234960" place_rank="30" boundingbox="54.6670935059,54.6870973206,25.2638857269,25.2838876343" lat="54.677095" lon="25.2738876" display_name="Railway Museum of Lithuania, 15, Mindaugo g., Senamiesčio seniūnija, Vilnius, Vilnius County, Vilniaus m. savivaldybė, Vilniaus apskritis, 03215, Lithuania" class="tourism" type="museum" icon="http://open.mapquestapi.com/nominatim/v1/images/mapicons/tourist_museum.p.20.png"/>
         //<place place_id="29614806" osm_type="way" osm_id="24845629" place_rank="30" boundingbox="54.6904983520508,54.6920852661133,25.2606296539307,25.2628803253174" lat="54.6913385159005" lon="25.2617684209873" display_name="Seimas (Parliament) of the Republic of Lithuania, 53, Gedimino pr., Naujamiesčio seniūnija, Vilnius, Vilnius County, Vilniaus m. savivaldybė, Vilniaus apskritis, LT-01111, Lithuania" class="amenity" type="public_building"/>
         //</searchresults> 
         #endregion

         return GetLatLngFromGeocoderUrl(MakeGeocoderUrl(keywords), out pointList);
      }

      public PointLatLng? GetPoint(string keywords, out GeoCoderStatusCode status)
      {
         List<PointLatLng> pointList;
         status = GetPoints(keywords, out pointList);
         return pointList != null && pointList.Count > 0 ? pointList[0] : (PointLatLng?) null;
      }

      public GeoCoderStatusCode GetPoints(Placemark placemark, out List<PointLatLng> pointList)
      {
         // http://nominatim.openstreetmap.org/search?street=&city=vilnius&county=&state=&country=lithuania&postalcode=&format=xml

         #region -- response --
         //<searchresults timestamp="Thu, 29 Nov 12 08:38:23 +0000" attribution="Data © OpenStreetMap contributors, ODbL 1.0. http://www.openstreetmap.org/copyright" querystring="vilnius, lithuania" polygon="false" exclude_place_ids="98093941" more_url="http://nominatim.openstreetmap.org/search?format=xml&exclude_place_ids=98093941&accept-language=de-de,de;q=0.8,en-us;q=0.5,en;q=0.3&q=vilnius%2C+lithuania">
         //<place place_id="98093941" osm_type="relation" osm_id="1529146" place_rank="16" boundingbox="54.5693359375,54.8323097229004,25.0250644683838,25.4815216064453" lat="54.6843135" lon="25.2853984" display_name="Vilnius, Vilniaus m. savivaldybė, Distrikt Vilnius, Litauen" class="boundary" type="administrative" icon="http://nominatim.openstreetmap.org/images/mapicons/poi_boundary_administrative.p.20.png"/>
         //</searchresults> 
         #endregion

         return GetLatLngFromGeocoderUrl(MakeDetailedGeocoderUrl(placemark), out pointList);
      }

      public PointLatLng? GetPoint(Placemark placemark, out GeoCoderStatusCode status)
      {
         List<PointLatLng> pointList;
         status = GetPoints(placemark, out pointList);
         return pointList != null && pointList.Count > 0 ? pointList[0] : (PointLatLng?) null;
      }

      public GeoCoderStatusCode GetPlacemarks(PointLatLng location, out List<Placemark> placemarkList)
      {
         throw new NotImplementedException("use GetPlacemark");
      }

      public Placemark? GetPlacemark(PointLatLng location, out GeoCoderStatusCode status)
      {
         //http://nominatim.openstreetmap.org/reverse?format=xml&lat=52.5487429714954&lon=-1.81602098644987&zoom=18&addressdetails=1

         #region -- response --
         /*
         <reversegeocode timestamp="Wed, 01 Feb 12 09:51:11 -0500" attribution="Data Copyright OpenStreetMap Contributors, Some Rights Reserved. CC-BY-SA 2.0." querystring="format=xml&lat=52.5487429714954&lon=-1.81602098644987&zoom=18&addressdetails=1">
         <result place_id="2061235282" osm_type="way" osm_id="90394420" lat="52.5487800131654" lon="-1.81626922291265">
         137, Pilkington Avenue, Castle Vale, City of Birmingham, West Midlands, England, B72 1LH, United Kingdom
         </result>
         <addressparts>
         <house_number>
         137
         </house_number>
         <road>
         Pilkington Avenue
         </road>
         <suburb>
         Castle Vale
         </suburb>
         <city>
         City of Birmingham
         </city>
         <county>
         West Midlands
         </county>
         <state_district>
         West Midlands
         </state_district>
         <state>
         England
         </state>
         <postcode>
         B72 1LH
         </postcode>
         <country>
         United Kingdom
         </country>
         <country_code>
         gb
         </country_code>
         </addressparts>
         </reversegeocode>
         */

         #endregion

         return GetPlacemarkFromReverseGeocoderUrl(MakeReverseGeocoderUrl(location), out status);
      }

      #region -- internals --

      string MakeGeocoderUrl(string keywords)
      {
         return string.Format(GeocoderUrlFormat, keywords.Replace(' ', '+'));
      }

      string MakeDetailedGeocoderUrl(Placemark placemark)
      {
         var street = String.Join(" ", new[] { placemark.HouseNo, placemark.ThoroughfareName }).Trim();
         return string.Format(GeocoderDetailedUrlFormat,
                              street.Replace(' ', '+'),
                              placemark.LocalityName.Replace(' ', '+'),
                              placemark.SubAdministrativeAreaName.Replace(' ', '+'),
                              placemark.AdministrativeAreaName.Replace(' ', '+'),
                              placemark.CountryName.Replace(' ', '+'),
                              placemark.PostalCodeNumber.Replace(' ', '+'));
      }

      string MakeReverseGeocoderUrl(PointLatLng pt)
      {
         return string.Format(CultureInfo.InvariantCulture, ReverseGeocoderUrlFormat, pt.Lat, pt.Lng);
      }

      GeoCoderStatusCode GetLatLngFromGeocoderUrl(string url, out List<PointLatLng> pointList)
      {
         var status = GeoCoderStatusCode.Unknow;
         pointList = null;

         try
         {
            string geo = GMaps.Instance.UseGeocoderCache ? Cache.Instance.GetContent(url, CacheType.GeocoderCache) : string.Empty;

            bool cache = false;

            if(string.IsNullOrEmpty(geo))
            {
               geo = GetContentUsingHttp(url);

               if(!string.IsNullOrEmpty(geo))
               {
                  cache = true;
               }
            }

            if(!string.IsNullOrEmpty(geo))
            {
               if(geo.StartsWith("<?xml") && geo.Contains("<place"))
               {
                  if(cache && GMaps.Instance.UseGeocoderCache)
                  {
                     Cache.Instance.SaveContent(url, CacheType.GeocoderCache, geo);
                  }

                  XmlDocument doc = new XmlDocument();
                  doc.LoadXml(geo);
                  {
                     XmlNodeList l = doc.SelectNodes("/searchresults/place");
                     if(l != null)
                     {
                        pointList = new List<PointLatLng>();

                        foreach(XmlNode n in l)
                        {
                           var nn = n.Attributes["place_rank"];

                           int rank = 0;
#if !PocketPC
                           if (nn != null && Int32.TryParse(nn.Value, out rank))
                           {
#else
                           if(nn != null && !string.IsNullOrEmpty(nn.Value))
                           {
                              rank = int.Parse(nn.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
#endif
                              if(rank < MinExpectedRank)
                                 continue;
                           }

                           nn = n.Attributes["lat"];
                           if(nn != null)
                           {
                              double lat = double.Parse(nn.Value, CultureInfo.InvariantCulture);

                              nn = n.Attributes["lon"];
                              if(nn != null)
                              {
                                 double lng = double.Parse(nn.Value, CultureInfo.InvariantCulture);
                                 pointList.Add(new PointLatLng(lat, lng));
                              }
                           }
                        }

                        status = GeoCoderStatusCode.G_GEO_SUCCESS;
                     }
                  }
               }
            }
         }
         catch(Exception ex)
         {
            status = GeoCoderStatusCode.ExceptionInCode;
            Debug.WriteLine("GetLatLngFromGeocoderUrl: " + ex);
         }

         return status;
      }

      Placemark? GetPlacemarkFromReverseGeocoderUrl(string url, out GeoCoderStatusCode status)
      {
         status = GeoCoderStatusCode.Unknow;
         Placemark? ret = null;

         try
         {
            string geo = GMaps.Instance.UsePlacemarkCache ? Cache.Instance.GetContent(url, CacheType.PlacemarkCache) : string.Empty;

            bool cache = false;

            if(string.IsNullOrEmpty(geo))
            {
               geo = GetContentUsingHttp(url);

               if(!string.IsNullOrEmpty(geo))
               {
                  cache = true;
               }
            }

            if(!string.IsNullOrEmpty(geo))
            {
               if(geo.StartsWith("<?xml") && geo.Contains("<result"))
               {
                  if(cache && GMaps.Instance.UsePlacemarkCache)
                  {
                     Cache.Instance.SaveContent(url, CacheType.PlacemarkCache, geo);
                  }

                  XmlDocument doc = new XmlDocument();
                  doc.LoadXml(geo);
                  {
                     XmlNode r = doc.SelectSingleNode("/reversegeocode/result");
                     if(r != null)
                     {
                        var p = new Placemark(r.InnerText);

                        XmlNode ad = doc.SelectSingleNode("/reversegeocode/addressparts");
                        if(ad != null)
                        {
                           var vl = ad.SelectSingleNode("country");
                           if(vl != null)
                           {
                              p.CountryName = vl.InnerText;
                           }

                           vl = ad.SelectSingleNode("country_code");
                           if(vl != null)
                           {
                              p.CountryNameCode = vl.InnerText;
                           }

                           vl = ad.SelectSingleNode("postcode");
                           if(vl != null)
                           {
                              p.PostalCodeNumber = vl.InnerText;
                           }

                           vl = ad.SelectSingleNode("state");
                           if(vl != null)
                           {
                              p.AdministrativeAreaName = vl.InnerText;
                           }

                           vl = ad.SelectSingleNode("region");
                           if(vl != null)
                           {
                              p.SubAdministrativeAreaName = vl.InnerText;
                           }

                           vl = ad.SelectSingleNode("suburb");
                           if(vl != null)
                           {
                              p.LocalityName = vl.InnerText;
                           }

                           vl = ad.SelectSingleNode("road");
                           if(vl != null)
                           {
                              p.ThoroughfareName = vl.InnerText;
                           }
                        }

                        ret = p;

                        status = GeoCoderStatusCode.G_GEO_SUCCESS;
                     }
                  }
               }
            }
         }
         catch(Exception ex)
         {
            ret = null;
            status = GeoCoderStatusCode.ExceptionInCode;
            Debug.WriteLine("GetPlacemarkFromReverseGeocoderUrl: " + ex);
         }

         return ret;
      }

      static readonly string ReverseGeocoderUrlFormat = "http://nominatim.openstreetmap.org/reverse?format=xml&lat={0}&lon={1}&zoom=18&addressdetails=1";
      static readonly string GeocoderUrlFormat = "http://nominatim.openstreetmap.org/search?q={0}&format=xml";
      static readonly string GeocoderDetailedUrlFormat = "http://nominatim.openstreetmap.org/search?street={0}&city={1}&county={2}&state={3}&country={4}&postalcode={5}&format=xml";

      #endregion

      #endregion
   }

   /// <summary>
   /// OpenStreetMap provider - http://www.openstreetmap.org/
   /// </summary>
   public class OpenStreetMapProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenStreetMapProvider Instance;

      OpenStreetMapProvider()
      {
      }

      static OpenStreetMapProvider()
      {
         Instance = new OpenStreetMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("0521335C-92EC-47A8-98A5-6FD333DDA9C0");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenStreetMap";
      public override string Name
      {
         get
         {
            return name;
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
         string url = MakeTileImageUrl(pos, zoom, string.Empty);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         char letter = ServerLetters[GetServerNum(pos, 3)];
         var url =  string.Format(UrlFormat, letter, zoom, pos.X, pos.Y);

         return url;
      }

      static readonly string UrlFormat = "http://{0}.tile.openstreetmap.org/{1}/{2}/{3}.png";
   }
}
