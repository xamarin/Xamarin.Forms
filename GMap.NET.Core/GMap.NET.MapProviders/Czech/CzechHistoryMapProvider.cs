
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// CzechHistoryMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechHistoryMapProvider : CzechMapProviderBase
   {
      public static readonly CzechHistoryMapProvider Instance;

      CzechHistoryMapProvider()
      {
         MaxZoom = 15;
      }

      static CzechHistoryMapProvider()
      {
         Instance = new CzechHistoryMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("CD44C19D-5EED-4623-B367-FB39FDC55B8F");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechHistoryMap";
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
               overlays = new GMapProvider[] { this, CzechHybridMapProvider.Instance };
            }
            return overlays;
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
         // http://m3.mapserver.mapy.cz/army2-m/14-8802-5528

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/army2-m/{1}-{2}-{3}";
   }
}