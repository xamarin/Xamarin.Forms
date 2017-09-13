
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// GoogleSatelliteMap provider
   /// </summary>
   public class GoogleSatelliteMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleSatelliteMapProvider Instance;

      GoogleSatelliteMapProvider()
      {
      }

      static GoogleSatelliteMapProvider()
      {
         Instance = new GoogleSatelliteMapProvider();
      }

      public string Version = "192";

      #region GMapProvider Members

      readonly Guid id = new Guid("9CB89D76-67E9-47CF-8137-B9EE9FC46388");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleSatelliteMap";
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
         string sec1 = string.Empty; // after &x=...
         string sec2 = string.Empty; // after &zoom=...
         GetSecureWords(pos, out sec1, out sec2);

         return string.Format(UrlFormat, UrlFormatServer, GetServerNum(pos, 4), UrlFormatRequest, Version, language, pos.X, sec1, pos.Y, zoom, sec2, Server);
      }

      static readonly string UrlFormatServer = "khm";
      static readonly string UrlFormatRequest = "kh";
      static readonly string UrlFormat = "http://{0}{1}.{10}/{2}/v={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
   }
}