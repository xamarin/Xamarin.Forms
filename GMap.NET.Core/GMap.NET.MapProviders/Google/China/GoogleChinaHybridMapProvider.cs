
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// GoogleChinaHybridMap provider
   /// </summary>
   public class GoogleChinaHybridMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleChinaHybridMapProvider Instance;

      GoogleChinaHybridMapProvider()
      {
         RefererUrl = string.Format("http://ditu.{0}/", ServerChina);
      }

      static GoogleChinaHybridMapProvider()
      {
         Instance = new GoogleChinaHybridMapProvider();
      }

      public string Version = "h@298";

      #region GMapProvider Members

      readonly Guid id = new Guid("B8A2A78D-1C49-45D0-8F03-9B95C83116B7");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleChinaHybridMap";
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
               overlays = new GMapProvider[] { GoogleChinaSatelliteMapProvider.Instance, this };
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

         return string.Format(UrlFormat, UrlFormatServer, GetServerNum(pos, 4), UrlFormatRequest, Version, ChinaLanguage, pos.X, sec1, pos.Y, zoom, sec2, ServerChina);
      }

      static readonly string ChinaLanguage = "zh-CN";
      static readonly string UrlFormatServer = "mt";
      static readonly string UrlFormatRequest = "vt";
      static readonly string UrlFormat = "http://{0}{1}.{10}/{2}/imgtp=png32&lyrs={3}&hl={4}&gl=cn&x={5}{6}&y={7}&z={8}&s={9}";
   }
}