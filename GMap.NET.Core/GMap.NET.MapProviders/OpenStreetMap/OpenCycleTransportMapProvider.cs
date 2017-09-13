
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// OpenCycleMap Transport provider - http://www.opencyclemap.org
   /// </summary>
   public class OpenCycleTransportMapProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenCycleTransportMapProvider Instance;

      OpenCycleTransportMapProvider()
      {
         RefererUrl = "http://www.opencyclemap.org/";
      }

      static OpenCycleTransportMapProvider()
      {
         Instance = new OpenCycleTransportMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("AF66DF88-AD25-43A9-8F82-56FCA49A748A");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenCycleTransportMap";
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
         string url = MakeTileImageUrl(pos, zoom, string.Empty);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         char letter = ServerLetters[GMapProvider.GetServerNum(pos, 3)];
         return string.Format(UrlFormat, letter, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.tile2.opencyclemap.org/transport/{1}/{2}/{3}.png";
   }
}
