
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;

   /// <summary>
   /// ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_Map provider, 
   /// http://www.darb.ae/ArcGIS/rest/services/BaseMaps/Q2_2011_NAVTQ_Eng_V5/MapServer
   /// </summary>
   public class ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_MapProvider : GMapProvider
   {
      public static readonly ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_MapProvider Instance;

      ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_MapProvider()
      {
         MaxZoom = 12;
         Area = RectLatLng.FromLTRB(49.8846923723311, 28.0188609585523, 58.2247031977662, 21.154115956732);
         Copyright = string.Format("©{0} ESRI - Map data ©{0} ArcGIS", DateTime.Today.Year);
      }

      static ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_MapProvider()
      {
         Instance = new ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("E03CFEDF-9277-49B3-9912-D805347F934B");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_DarbAE_Q2_2011_NAVTQ_Eng_V5_MapProvider";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return PlateCarreeProjectionDarbAe.Instance;
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
         string url = MakeTileImageUrl(pos, zoom);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom)
      {
         // http://www.darb.ae/ArcGIS/rest/services/BaseMaps/Q2_2011_NAVTQ_Eng_V5/MapServer/tile/0/121/144

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://www.darb.ae/ArcGIS/rest/services/BaseMaps/Q2_2011_NAVTQ_Eng_V5/MapServer/tile/{0}/{1}/{2}";
   }
}