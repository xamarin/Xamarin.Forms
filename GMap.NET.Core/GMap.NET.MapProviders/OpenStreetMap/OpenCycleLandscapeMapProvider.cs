
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// OpenCycleMap Landscape provider - http://www.opencyclemap.org
   /// </summary>
   public class OpenCycleLandscapeMapProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenCycleLandscapeMapProvider Instance;

      OpenCycleLandscapeMapProvider()
      {
         RefererUrl = "http://www.opencyclemap.org/";
      }

      static OpenCycleLandscapeMapProvider()
      {
         Instance = new OpenCycleLandscapeMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("BDBAA939-6597-4D87-8F4F-261C49E35F56");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenCycleLandscapeMap";
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
                                        

      static readonly string UrlFormat = "http://{0}.tile3.opencyclemap.org/landscape/{1}/{2}/{3}.png";
   }
}
