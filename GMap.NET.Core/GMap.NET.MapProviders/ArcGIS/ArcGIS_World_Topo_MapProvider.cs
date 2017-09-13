
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// ArcGIS_World_Topo_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_World_Topo_MapProvider : ArcGISMapMercatorProviderBase
   {
      public static readonly ArcGIS_World_Topo_MapProvider Instance;

      ArcGIS_World_Topo_MapProvider()
      {
      }

      static ArcGIS_World_Topo_MapProvider()
      {
         Instance = new ArcGIS_World_Topo_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("E0354A49-7447-4C9A-814F-A68565ED834B");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_World_Topo_Map";
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
         // http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/0/0/0jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{0}/{1}/{2}";
   }
}