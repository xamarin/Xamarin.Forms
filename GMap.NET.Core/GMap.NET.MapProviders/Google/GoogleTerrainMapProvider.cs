
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// GoogleTerrainMap provider
   /// </summary>
   public class GoogleTerrainMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleTerrainMapProvider Instance;

      GoogleTerrainMapProvider()
      {
      }

      static GoogleTerrainMapProvider()
      {
         Instance = new GoogleTerrainMapProvider();
      }

      public string Version = "t@132,r@333000000";

      #region GMapProvider Members

      readonly Guid id = new Guid("A42EDF2E-63C5-4967-9DBF-4EFB3AF7BC11");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleTerrainMap";
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

      static readonly string UrlFormatServer = "mt";
      static readonly string UrlFormatRequest = "vt";
      static readonly string UrlFormat = "http://{0}{1}.{10}/maps/{2}/lyrs={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
   }
}