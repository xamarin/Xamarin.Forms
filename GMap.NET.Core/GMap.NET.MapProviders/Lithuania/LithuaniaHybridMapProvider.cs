
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// LithuaniaHybridMap provider
   /// </summary>
   public class LithuaniaHybridMapProvider : LithuaniaMapProviderBase
   {
      public static readonly LithuaniaHybridMapProvider Instance;

      LithuaniaHybridMapProvider()
      {
      }

      static LithuaniaHybridMapProvider()
      {
         Instance = new LithuaniaHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("279AB0E0-4704-4AA6-86AD-87D13B1F8975");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "LithuaniaHybridMap";
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
               overlays = new GMapProvider[] { LithuaniaOrtoFotoMapProvider.Instance, this };
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
         // http://dc5.maps.lt/cache/mapslt_ortofoto_overlay/map/_alllayers/L09/R000016b1/C000020e1.jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      internal static readonly string UrlFormat = "http://dc5.maps.lt/cache/mapslt_ortofoto_overlay/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.png";
   }
}