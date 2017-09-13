
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// GoogleHybridMap provider
   /// </summary>
   public class GoogleHybridMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleHybridMapProvider Instance;

      GoogleHybridMapProvider()
      {
      }

      static GoogleHybridMapProvider()
      {
         Instance = new GoogleHybridMapProvider();
      }

      public string Version = "h@333000000";

      #region GMapProvider Members

      readonly Guid id = new Guid("B076C255-6D12-4466-AAE0-4A73D20A7E6A");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleHybridMap";
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
               overlays = new GMapProvider[] { GoogleSatelliteMapProvider.Instance, this };
            }
            return overlays;
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