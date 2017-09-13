
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// GoogleKoreaSatelliteMap provider
   /// </summary>
   public class GoogleKoreaSatelliteMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleKoreaSatelliteMapProvider Instance;

      GoogleKoreaSatelliteMapProvider()
      {
      }

      static GoogleKoreaSatelliteMapProvider()
      {
         Instance = new GoogleKoreaSatelliteMapProvider();
      }

      public string Version = "170";

      #region GMapProvider Members

      readonly Guid id = new Guid("70370941-D70C-4123-BE4A-AEE6754047F5");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleKoreaSatelliteMap";
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
         string sec1 = string.Empty;
         string sec2 = string.Empty;
         GetSecureWords(pos, out sec1, out sec2);

         return string.Format(UrlFormat, UrlFormatServer, GetServerNum(pos, 4), UrlFormatRequest, Version, language, pos.X, sec1, pos.Y, zoom, sec2, ServerKoreaKr);
      }

      static readonly string UrlFormatServer = "khm";
      static readonly string UrlFormatRequest = "kh";
      static readonly string UrlFormat = "http://{0}{1}.{10}/{2}/v={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
   }
}