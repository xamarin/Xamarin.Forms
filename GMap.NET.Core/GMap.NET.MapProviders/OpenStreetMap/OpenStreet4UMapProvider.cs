
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// OpenStreet4UMap provider
   /// http://www.4umaps.eu
   /// 
   /// 4UMaps are topographic outdoor maps based on OpenStreetmap data.
   /// The map contains everything you need for any kind of back country activity like hiking,
   /// mountain biking, cycling, climbing etc. 4UMaps has elevation lines, hill shading,
   /// peak height and name, streets, ways, tracks and trails, as well as springs, supermarkets,
   /// restaurants, hotels, shelters etc.
   /// </summary>
   public class OpenStreet4UMapProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenStreet4UMapProvider Instance;

      OpenStreet4UMapProvider()
      {
         RefererUrl = "http://www.4umaps.eu/map.htm";
         Copyright = string.Format("© 4UMaps.eu, © OpenStreetMap - Map data ©{0} OpenStreetMap", DateTime.Today.Year);
      }

      static OpenStreet4UMapProvider()
      {
         Instance = new OpenStreet4UMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("3E3D919E-9814-4978-B430-6AAB2C1E41B2");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenStreet4UMap";
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
         return string.Format(UrlFormat, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://4umaps.eu/{0}/{1}/{2}.png";
   }
}
