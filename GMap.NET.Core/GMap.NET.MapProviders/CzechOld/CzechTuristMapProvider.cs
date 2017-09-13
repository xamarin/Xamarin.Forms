
namespace GMap.NET.MapProviders
{
   using System;

   /// <summary>
   /// CzechTuristMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechTuristMapProviderOld : CzechMapProviderBaseOld
   {
      public static readonly CzechTuristMapProviderOld Instance;

      CzechTuristMapProviderOld()
      {
      }

      static CzechTuristMapProviderOld()
      {
         Instance = new CzechTuristMapProviderOld();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("B923C81D-880C-42EB-88AB-AF8FE42B564D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechTuristOldMap";
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
         // http://m1.mapserver.mapy.cz/turist/3_8000000_8000000

         long xx = pos.X << (28 - zoom);
         long yy = ((((long)Math.Pow(2.0, (double)zoom)) - 1) - pos.Y) << (28 - zoom);

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/turist/{1}_{2:x7}_{3:x7}";
   }
}