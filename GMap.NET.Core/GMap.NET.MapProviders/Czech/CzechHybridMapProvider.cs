
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// CzechHybridMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechHybridMapProvider : CzechMapProviderBase
   {
      public static readonly CzechHybridMapProvider Instance;

      CzechHybridMapProvider()
      {
      }

      static CzechHybridMapProvider()
      {
         Instance = new CzechHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("7540CE5B-F634-41E9-B23E-A6E0A97526FD");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechHybridMap";
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
               overlays = new GMapProvider[] { CzechSatelliteMapProvider.Instance, this };
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
         // http://m3.mapserver.mapy.cz/hybrid-m/14-8802-5528

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/hybrid-m/{1}-{2}-{3}";
   }
}