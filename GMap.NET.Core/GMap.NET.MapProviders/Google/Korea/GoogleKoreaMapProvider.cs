
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// GoogleKoreaMap provider
   /// </summary>
   public class GoogleKoreaMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleKoreaMapProvider Instance;

      GoogleKoreaMapProvider()
      {
         Area = new RectLatLng(38.6597777307125, 125.738525390625, 4.02099609375, 4.42072406219614);
      }

      static GoogleKoreaMapProvider()
      {
         Instance = new GoogleKoreaMapProvider();
      }

      public string Version = "kr1.12";

      #region GMapProvider Members

      readonly Guid id = new Guid("0079D360-CB1B-4986-93D5-AD299C8E20E6");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleKoreaMap";
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

         return string.Format(UrlFormat, UrlFormatServer, GetServerNum(pos, 4), UrlFormatRequest, Version, language, pos.X, sec1, pos.Y, zoom, sec2, ServerKorea);
      }

      static readonly string UrlFormatServer = "mt";
      static readonly string UrlFormatRequest = "mt";
      static readonly string UrlFormat = "http://{0}{1}.{10}/{2}/v={3}&hl={4}&x={5}{6}&y={7}&z={8}&s={9}";
   }
}