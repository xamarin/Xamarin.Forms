
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// OpenSeaMapHybrid provider - http://openseamap.org
   /// </summary>
   public class OpenSeaMapHybridProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenSeaMapHybridProvider Instance;

      OpenSeaMapHybridProvider()
      {
         RefererUrl = "http://openseamap.org/";
      }

      static OpenSeaMapHybridProvider()
      {
         Instance = new OpenSeaMapHybridProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("FAACDE73-4B90-4AE6-BB4A-ADE4F3545592");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenSeaMapHybrid";
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
               overlays = new GMapProvider[] { OpenStreetMapProvider.Instance, this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, string.Empty);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         return string.Format(UrlFormat, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://tiles.openseamap.org/seamark/{0}/{1}/{2}.png";
   }
}
