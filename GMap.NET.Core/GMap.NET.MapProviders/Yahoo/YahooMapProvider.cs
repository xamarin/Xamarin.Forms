
namespace GMap.NET.MapProviders
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.Globalization;
   using System.Xml;
   using GMap.NET.Internals;
   using GMap.NET.Projections;

   public abstract class YahooMapProviderBase : GMapProvider, GeocodingProvider
   {
      public YahooMapProviderBase()
      {
         RefererUrl = "http://maps.yahoo.com/";
         Copyright = string.Format("© Yahoo! Inc. - Map data & Imagery ©{0} NAVTEQ", DateTime.Today.Year);
      }

      public string AppId = string.Empty;
      public int MinExpectedQuality = 39;

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

      #region GeocodingProvider Members

      public GeoCoderStatusCode GetPoints(string keywords, out List<PointLatLng> pointList)
      {
          // http://where.yahooapis.com/geocode?q=lithuania,vilnius&appid=1234&flags=CG&gflags=QL&locale=LT-lt

          #region -- response --
          //<ResultSet version="1.0"><Error>0</Error><ErrorMessage>No error</ErrorMessage><Locale>LT-lt</Locale><Quality>40</Quality><Found>1</Found><Result><quality>40</quality><latitude>54.689850</latitude><longitude>25.269260</longitude><offsetlat>54.689850</offsetlat><offsetlon>25.269260</offsetlon><radius>46100</radius></Result></ResultSet>
          #endregion

          return GetLatLngFromGeocoderUrl(MakeGeocoderUrl(keywords), out pointList);
      }

      public PointLatLng? GetPoint(string keywords, out GeoCoderStatusCode status)
      {
          List<PointLatLng> pointList;
          status = GetPoints(keywords, out pointList);
          return pointList != null && pointList.Count > 0 ? pointList[0] : (PointLatLng?)null;
      }

      public GeoCoderStatusCode GetPoints(Placemark placemark, out List<PointLatLng> pointList)
      {
          // http://where.yahooapis.com/geocode?country=LT&state=Vilniaus+Apskritis&county=Vilniaus+Miesto+Savivaldybe&city=Vilnius&neighborhood=Naujamiestis&postal=01108&street=J.+Tumo-Vaizganto+Gatve&house=2&appid=1234&flags=CG&gflags=QL&locale=LT-lt

          #region -- response --
          //<ResultSet version="1.0"><Error>0</Error><ErrorMessage>No error</ErrorMessage><Locale>LT-lt</Locale><Quality>19</Quality><Found>1</Found><Result><quality>87</quality><latitude>54.690181</latitude><longitude>25.269483</longitude><offsetlat>54.690227</offsetlat><offsetlon>25.269278</offsetlon><radius>500</radius></Result></ResultSet>
          #endregion

          return GetLatLngFromGeocoderUrl(MakeGeocoderDetailedUrl(placemark), out pointList);
      }

      public PointLatLng? GetPoint(Placemark placemark, out GeoCoderStatusCode status)
      {
          List<PointLatLng> pointList;
          status = GetPoints(placemark, out pointList);
          return pointList != null && pointList.Count > 0 ? pointList[0] : (PointLatLng?)null;
      }

      public GeoCoderStatusCode GetPlacemarks(PointLatLng location, out List<Placemark> placemarkList)
      {
          // http://where.yahooapis.com/geocode?q=54.689850,25.269260&appid=1234&flags=G&gflags=QRL&locale=LT-lt

          #region -- response --
          //<ResultSet version="1.0"><Error>0</Error><ErrorMessage>No error</ErrorMessage><Locale>LT-lt</Locale><Quality>99</Quality><Found>1</Found><Result><quality>99</quality><latitude>54.689850</latitude><longitude>25.269260</longitude><offsetlat>54.689850</offsetlat><offsetlon>25.269260</offsetlon><radius>500</radius><name>54.689850,25.269260</name><line1>2 J. Tumo-Vaizganto Gatve</line1><line2>01108 Naujamiestis</line2><line3/><line4>Lietuvos Respublika</line4><house>2</house><street>J. Tumo-Vaizganto Gatve</street><xstreet/><unittype/><unit/><postal>01108</postal><level4>Naujamiestis</level4><level3>Vilnius</level3><level2>Vilniaus Miesto Savivaldybe</level2><level1>Vilniaus Apskritis</level1><level0>Lietuvos Respublika</level0><level0code>LT</level0code><level1code/><level2code/><hash/><woeid>12758362</woeid><woetype>11</woetype><uzip>01108</uzip></Result></ResultSet>
          #endregion

          return GetPlacemarksFromReverseGeocoderUrl(MakeReverseGeocoderUrl(location), out placemarkList);
      }

      public Placemark ? GetPlacemark(PointLatLng location, out GeoCoderStatusCode status)
      {
          List<Placemark> placemarkList;
          status = GetPlacemarks(location, out placemarkList);
          return placemarkList != null && placemarkList.Count > 0 ? placemarkList[0] : (Placemark?)null;
      }

      #region -- internals --

      string MakeGeocoderUrl(string keywords)
      {
         return string.Format(CultureInfo.InvariantCulture, GeocoderUrlFormat, keywords.Replace(' ', '+'), AppId, !string.IsNullOrEmpty(LanguageStr) ? "&locale=" + LanguageStr : "");
      }

      string MakeGeocoderDetailedUrl(Placemark placemark)
      {
          return string.Format(GeocoderDetailedUrlFormat,
                               PrepareUrlString(placemark.CountryName),
                               PrepareUrlString(placemark.AdministrativeAreaName),
                               PrepareUrlString(placemark.SubAdministrativeAreaName),
                               PrepareUrlString(placemark.LocalityName),
                               PrepareUrlString(placemark.DistrictName),
                               PrepareUrlString(placemark.PostalCodeNumber),
                               PrepareUrlString(placemark.ThoroughfareName),
                               PrepareUrlString(placemark.HouseNo),
                               AppId,
                               !string.IsNullOrEmpty(LanguageStr) ? "&locale=" + LanguageStr : string.Empty);
      }

      string MakeReverseGeocoderUrl(PointLatLng pt)
      {
         return string.Format(CultureInfo.InvariantCulture, ReverseGeocoderUrlFormat, pt.Lat, pt.Lng, AppId, !string.IsNullOrEmpty(LanguageStr) ? "&locale=" + LanguageStr : "");
      }

      string PrepareUrlString(string str)
      {
          if (str == null) return string.Empty;
          return str.Replace(' ', '+');
      }

      GeoCoderStatusCode GetLatLngFromGeocoderUrl(string url, out List<PointLatLng> pointList)
      {
          var status = GeoCoderStatusCode.Unknow;
          pointList = null;

          try
          {
              string geo = GMaps.Instance.UseGeocoderCache ? Cache.Instance.GetContent(url, CacheType.GeocoderCache) : string.Empty;

              bool cache = false;

              if (string.IsNullOrEmpty(geo))
              {
                  geo = GetContentUsingHttp(url);

                  if (!string.IsNullOrEmpty(geo))
                  {
                      cache = true;
                  }
              }

              if (!string.IsNullOrEmpty(geo))
              {
                  if (geo.StartsWith("<?xml") && geo.Contains("<Result"))
                  {
                      if (cache && GMaps.Instance.UseGeocoderCache)
                      {
                          Cache.Instance.SaveContent(url, CacheType.GeocoderCache, geo);
                      }

                      XmlDocument doc = new XmlDocument();
                      doc.LoadXml(geo);
                      {
                          XmlNodeList l = doc.SelectNodes("/ResultSet/Result");
                          if (l != null)
                          {
                              pointList = new List<PointLatLng>();

                              foreach (XmlNode n in l)
                              {
                                  var nn = n.SelectSingleNode("quality");
                                  if (nn != null)
                                  {
                                      var quality = int.Parse(nn.InnerText);
                                      if (quality < MinExpectedQuality) continue;

                                      nn = n.SelectSingleNode("latitude");
                                      if (nn != null)
                                      {
                                          double lat = double.Parse(nn.InnerText, CultureInfo.InvariantCulture);

                                          nn = n.SelectSingleNode("longitude");
                                          if (nn != null)
                                          {
                                              double lng = double.Parse(nn.InnerText, CultureInfo.InvariantCulture);
                                              pointList.Add(new PointLatLng(lat, lng));
                                          }
                                      }
                                  }
                              }

                              status = GeoCoderStatusCode.G_GEO_SUCCESS;
                          }
                      }
                  }
              }
          }
          catch (Exception ex)
          {
              status = GeoCoderStatusCode.ExceptionInCode;
              Debug.WriteLine("GetLatLngFromGeocoderUrl: " + ex);
          }

          return status;
      }

      GeoCoderStatusCode GetPlacemarksFromReverseGeocoderUrl(string url, out List<Placemark> placemarkList)
      {
          var status = GeoCoderStatusCode.Unknow;
          placemarkList = null;

          try
          {
              string geo = GMaps.Instance.UsePlacemarkCache ? Cache.Instance.GetContent(url, CacheType.PlacemarkCache) : string.Empty;

              bool cache = false;

              if (string.IsNullOrEmpty(geo))
              {
                  geo = GetContentUsingHttp(url);

                  if (!string.IsNullOrEmpty(geo))
                  {
                      cache = true;
                  }
              }

              if (!string.IsNullOrEmpty(geo))
              {
                  if (geo.StartsWith("<?xml") && geo.Contains("<ResultSet"))
                  {
                      if (cache && GMaps.Instance.UsePlacemarkCache)
                      {
                          Cache.Instance.SaveContent(url, CacheType.PlacemarkCache, geo);
                      }

                      XmlDocument doc = new XmlDocument();
                      doc.LoadXml(geo);
                      {
                          XmlNodeList l = doc.SelectNodes("/ResultSet/Result");
                          if (l != null)
                          {
                              placemarkList = new List<Placemark>();

                              foreach (XmlNode n in l)
                              {
                                  var vl = n.SelectSingleNode("name");
                                  if (vl == null) continue;

                                  Placemark placemark = new Placemark(vl.InnerText);

                                  vl = n.SelectSingleNode("level0");
                                  if (vl != null)
                                  {
                                      placemark.CountryName = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("level0code");
                                  if (vl != null)
                                  {
                                      placemark.CountryNameCode = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("postal");
                                  if (vl != null)
                                  {
                                      placemark.PostalCodeNumber = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("level1");
                                  if (vl != null)
                                  {
                                      placemark.AdministrativeAreaName = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("level2");
                                  if (vl != null)
                                  {
                                      placemark.SubAdministrativeAreaName = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("level3");
                                  if (vl != null)
                                  {
                                      placemark.LocalityName = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("level4");
                                  if (vl != null)
                                  {
                                      placemark.DistrictName = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("street");
                                  if (vl != null)
                                  {
                                      placemark.ThoroughfareName = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("house");
                                  if (vl != null)
                                  {
                                      placemark.HouseNo = vl.InnerText;
                                  }

                                  vl = n.SelectSingleNode("radius");
                                  if (vl != null)
                                  {
                                      placemark.Accuracy = int.Parse(vl.InnerText);
                                  }
                                  
                                  placemarkList.Add(placemark);
                              }

                              status = GeoCoderStatusCode.G_GEO_SUCCESS;
                          }
                      }
                  }
              }
          }
          catch (Exception ex)
          {
              status = GeoCoderStatusCode.ExceptionInCode;
              Debug.WriteLine("GetPlacemarkFromReverseGeocoderUrl: " + ex);
          }

          return status;
      }

      static readonly string ReverseGeocoderUrlFormat = "http://where.yahooapis.com/geocode?q={0},{1}&appid={2}&flags=G&gflags=QRL{3}";
      static readonly string GeocoderUrlFormat = "http://where.yahooapis.com/geocode?q={0}&appid={1}&flags=CG&gflags=QL{2}";
      static readonly string GeocoderDetailedUrlFormat = "http://where.yahooapis.com/geocode?country={0}&state={1}&county={2}&city={3}&neighborhood={4}&postal={5}&street={6}&house={7}&appid={8}&flags=CG&gflags=QL{9}";

      #endregion

      #endregion
   }

   /// <summary>
   /// YahooMap provider
   /// </summary>
   public class YahooMapProvider : YahooMapProviderBase
   {
      public static readonly YahooMapProvider Instance;

      YahooMapProvider()
      {
      }

      static YahooMapProvider()
      {
         Instance = new YahooMapProvider();
      }

      public string Version = "2.1";

      #region GMapProvider Members

      readonly Guid id = new Guid("65DB032C-6869-49B0-A7FC-3AE41A26AF4D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "YahooMap";
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
         // http://maps1.yimg.com/hx/tl?b=1&v=4.3&.intl=en&x=12&y=7&z=7&r=1
         // http://2.base.maps.api.here.com/maptile/2.1/maptile/newest/normal.day/11/1169/652/256/png8?lg=EN&token=TrLJuXVK62IQk0vuXFzaig%3D%3D&app_id=eAdkWGYRoc4RfxVo0Z4B
         // https://4.aerial.maps.api.here.com/maptile/2.1/maptile/newest/hybrid.day/11/1167/652/256/jpg?lg=ENG&token=TrLJuXVK62IQk0vuXFzaig%3D%3D&requestid=yahoo.prod&app_id=eAdkWGYRoc4RfxVo0Z4B
         // https://4.aerial.maps.api.here.com/maptile/2.1/maptile/newest/satellite.day/13/4671/2604/256/jpg?lg=ENG&token=TrLJuXVK62IQk0vuXFzaig%3D%3D&requestid=yahoo.prod&app_id=eAdkWGYRoc4RfxVo0Z4B

         return string.Format(UrlFormat, ((GetServerNum(pos, 2)) + 1), Version, zoom, pos.X, pos.Y, language, rnd1, rnd2);
      }

      string rnd1 = Guid.NewGuid().ToString("N").Substring(0, 28);
      string rnd2 = Guid.NewGuid().ToString("N").Substring(0, 20);

      //static readonly string UrlFormat = "http://maps{0}.yimg.com/hx/tl?v={1}&.intl={2}&x={3}&y={4}&z={5}&r=1";
      static readonly string UrlFormat = "http://{0}.base.maps.api.here.com/maptile/{1}/maptile/newest/normal.day/{2}/{3}/{4}/256/png8?lg={5}&token={6}&requestid=yahoo.prod&app_id={7}";
   }
}