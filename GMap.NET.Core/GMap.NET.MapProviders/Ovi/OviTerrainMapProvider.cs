
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// OviTerrainMap provider
   /// </summary>
   public class OviTerrainMapProvider : OviMapProviderBase
   {
      public static readonly OviTerrainMapProvider Instance;

      OviTerrainMapProvider()
      {
      }

      static OviTerrainMapProvider()
      {
         Instance = new OviTerrainMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("7267339C-445E-4E61-B8B8-82D0B7AAACC5");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OviTerrainMap";
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
         // http://d.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/terrain.day/12/2317/1277/256/png8

         return string.Format(UrlFormat, UrlServerLetters[GetServerNum(pos, 4)], zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/terrain.day/{1}/{2}/{3}/256/png8";
   }
}