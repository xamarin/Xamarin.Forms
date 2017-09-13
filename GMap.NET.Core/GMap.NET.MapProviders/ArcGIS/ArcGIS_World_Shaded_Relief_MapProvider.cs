
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// ArcGIS_World_Shaded_Relief_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_World_Shaded_Relief_MapProvider : ArcGISMapMercatorProviderBase
   {
      public static readonly ArcGIS_World_Shaded_Relief_MapProvider Instance;

      ArcGIS_World_Shaded_Relief_MapProvider()
      {
      }

      static ArcGIS_World_Shaded_Relief_MapProvider()
      {
         Instance = new ArcGIS_World_Shaded_Relief_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("2E821FEF-8EA1-458A-BC82-4F699F4DEE79");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_World_Shaded_Relief_Map";
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
         // http://services.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/0/0/0jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/{0}/{1}/{2}";
   }
}