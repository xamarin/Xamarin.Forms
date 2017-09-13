
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// OpenStreetMapQuestHybrid provider - http://wiki.openstreetmap.org/wiki/MapQuest
   /// </summary>
   public class OpenStreetMapQuestHybridProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenStreetMapQuestHybridProvider Instance;

      OpenStreetMapQuestHybridProvider()
      {
         Copyright = string.Format("© MapQuest - Map data ©{0} MapQuest, OpenStreetMap", DateTime.Today.Year);
      }

      static OpenStreetMapQuestHybridProvider()
      {
         Instance = new OpenStreetMapQuestHybridProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("95E05027-F846-4429-AB7A-9445ABEEFA2A");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenStreetMapQuestHybrid";
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
               overlays = new GMapProvider[] { OpenStreetMapQuestSatteliteProvider.Instance, this };
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
         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://otile{0}.mqcdn.com/tiles/1.0.0/hyb/{1}/{2}/{3}.png";
   }
}
